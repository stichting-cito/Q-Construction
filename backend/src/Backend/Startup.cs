using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Backend.Exports;
using Citolab.QConstruction.Backend.Exports.QTI;
using Citolab.QConstruction.Backend.Filters;
using Citolab.QConstruction.Backend.Helpers;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Item;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses.Authorization;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using Citolab.Repository.Extensions;
using Citolab.Repository.Mongo.Extensions;
using Citolab.Repository.Options;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace Citolab.QConstruction.Backend
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true) //load local 
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
                if (env.IsDevelopment())
                {
                    builder.AddApplicationInsightsSettings(developerMode: true);
                }
            try
            {
                //try to deleted folder
                var wwwRootPath = !string.IsNullOrWhiteSpace(env.WebRootPath)
                    ? env.WebRootPath
                    : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var folderName = Path.Combine(wwwRootPath, "Temp");
                DeleteDirectory(folderName);
            }
            catch
            {
                // do nothing
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static void DeleteDirectory(string d)
        {
            string[] files = Directory.GetFiles(d);
            string[] dirs = Directory.GetDirectories(d);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(d, true);
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var corsAllowedOrigins = Configuration.GetValue<string>("AppSettings:corsAllowedOrigins")?.Split(',');
            var corsPolicy = new CorsPolicyBuilder(corsAllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .Build();
            services.AddCors(x => x.AddPolicy("allowall", corsPolicy));
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Audience = Configuration["Auth0:ClientId"];
                    o.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                    o.SaveToken = true;
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                            {
                                if (context.Request.QueryString.HasValue)
                                {
                                    var token = context.Request.QueryString.Value
                                        .Split('&')
                                        .SingleOrDefault(x => x.Contains("authorization"))
                                        ?.Split('=')[1];

                                    if (!string.IsNullOrWhiteSpace(token))
                                    {
                                        context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                                    }
                                }
                            }
                            return Task.FromResult(0);
                        }
                    };
                });
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddResponseCompression();
            services.AddLogging();
            services.AddMemoryCache();
            services.AddScoped<ILoggedInUserProvider, Auth0LoggedInUserProvider>();
            services.AddMvcCore()
                .AddMvcOptions(opts =>
                {
                    opts.CacheProfiles.Add("Cache1Hour", new CacheProfile
                    {
                        Duration = 3600,
                        VaryByHeader = "Accept"
                    });
                    opts.CacheProfiles.Add("NoCache", new CacheProfile
                    {
                        Duration = 0,
                        VaryByHeader = "Accept",
                        NoStore = true
                    });
                    opts.Filters.Add(typeof(AddUserIdFilter));
                    opts.Filters.Add(typeof(LogActionFilter));
                    opts.Filters.Add(typeof(DomainExceptionFilter));
                    opts.Filters.Add(new ValidateModelStateAttribute());
                })
                .AddJsonOptions(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    // Force Camel Case to JSON
                })
                .AddRazorViewEngine()
                .AddJsonFormatters()
                .AddApiExplorer();
            services.Configure<RazorViewEngineOptions>(
                options => { options.ViewLocationExpanders.Add(new CustomViewLocationExpander()); });
            services.AddScoped<ViewRender, ViewRender>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin",
                    policy => policy.Requirements.Add(new TypeRequirement(UserType.Admin)));
                options.AddPolicy("Manager",
                    policy => policy.Requirements.Add(new TypeRequirement(UserType.RestrictedManager, UserType.Manager, UserType.Admin)));
                options.AddPolicy("Toetsdeskundige",
                    policy => policy.Requirements.Add(new RoleRequirement(UserType.Toetsdeskundige.ToString())));
                options.AddPolicy("Constructeur",
                    policy => policy.Requirements.Add(new RoleRequirement(UserType.Constructeur.ToString())));
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build();
            });
            services.AddSingleton<IAuthorizationHandler, TypeHandler>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "QConstruction", Version = "v1" });
            });
            services.AddScoped<ILoggedInUserProvider, Auth0LoggedInUserProvider>();
            switch (Configuration.GetValue<string>("AppSettings:repository").ToLower())
            {
                case "memory":
                    {
                        services.AddInMemoryRepository();
                        break;
                    }
                case "mongo":
                    {
                        services.AddMongoRepository("QConstruction", Configuration.GetConnectionString("MongoDB"));
                        break;
                    }
                default:
                    throw new Exception("Unknown repository specified in AppSettings:repository");
            }
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            services.AddMediatR(typeof(AddItemCommand).GetTypeInfo().Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            ILoggerFactory loggerFactory, IRepositoryFactory repositoryFactory, 
            IMediator mediator, IMemoryCache memoryCache,
            IHostingEnvironment env)
        {
            Constants.ManagementClientId = Configuration["auth0:ManagementClientId"];
            Constants.ClientSecret = Configuration["auth0:ClientSecret"];
            Constants.Auth0Domain = Configuration["auth0:Domain"];
            Constants.FontendClientId = Configuration["auth0:ClientId"];
            app.UseCors("allowall");
            
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers["Cache-Control"] =
                        "public,max-age=" + durationInSeconds;
                }
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseAuthentication();
            app.UseResponseCompression();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QConstruction V1"));
        }
    }
}