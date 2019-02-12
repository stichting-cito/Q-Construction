using System;
using System.IO;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.Repository;
using Citolab.Repository.Mongo.Extensions;
using FixTasks.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Seed
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Seeding Q-Construction..");
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(environment))
                throw new Exception("Environment not found in ASPNETCORE_ENVIRONMENT");

            Console.WriteLine("Environment: {0}", environment);
            if (environment.ToLower() == "production")
            {
                Console.Write("Environment to be seeded is Production, ARE YOU SURE? (y/n)");
                var sureKey = Console.ReadKey();
                if (sureKey.KeyChar != 'y' && sureKey.KeyChar != 'Y')
                {
                    Console.WriteLine("Aborted. Exiting..");
                    return;
                }
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true) //load local 
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Constants.ManagementClientId = Configuration["auth0:ManagementClientId"];
            Constants.ClientSecret = Configuration["auth0:ClientSecret"];
            Constants.Auth0Domain = Configuration["auth0:domain"];

            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
            var mongoDbConnectionString = Configuration.GetConnectionString("MongoDB");
            // Setting up DI
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddMediatR();
            services.AddMongoRepository("QConstruction", mongoDbConnectionString);
            services.AddLogging(l =>
            {
                l.AddConfiguration(Configuration.GetSection("Logging"));
                l.AddConsole();
            });
            // TODO: Run task

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var serviceProvider = services.BuildServiceProvider();
            var memCache = serviceProvider.GetService<IMemoryCache>();
            var reposFac = serviceProvider.GetService<IRepositoryFactory>();
            new SyncAdmin().DoFix(reposFac, memCache);
            Console.WriteLine("Finished fixing. Press a key to exit.");
            Console.ReadLine();
        }
    }
}
