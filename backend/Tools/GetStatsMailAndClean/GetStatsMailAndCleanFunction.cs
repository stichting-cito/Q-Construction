using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using Citolab.Repository.Mongo.Extensions;
using CsvHelper.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;

namespace GetStatsMailAndClean
{


    public static class GetStatsMailAndCleanFunction
    {
        //[TimerTrigger("0 */1 * * * *")]
        //[TimerTrigger("0 0 0 1-7 * MON")]
        [FunctionName("GetStatsMailAndCleanEveryFirstMonday")]
        public static void Run([TimerTrigger("0 0 0 1-7 * MON")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"GetStatsMailAndCleanEveryFirstMonday Timer trigger function executed at: {DateTime.Now}");
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            var slackHookUrl = config["SlackHookUrl"];
            var mongoDbConnectionString = config["MongoDB"];
            // Setting up DI
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddMongoRepository("QConstruction", mongoDbConnectionString);
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            log.LogInformation($"Connection string: {mongoDbConnectionString} environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
            var serviceProvider = services.BuildServiceProvider();
            var repositoryFactory = serviceProvider.GetService<IRepositoryFactory>();

            var userRepos = repositoryFactory.GetRepository<User>();
            var wishlistRepos = repositoryFactory.GetRepository<Wishlist>();
            var itemRepos = repositoryFactory.GetRepository<Item>();
            var screeningRepos = repositoryFactory.GetRepository<Screening>();
            var itemSummaryRepos = repositoryFactory.GetRepository<ItemSummary>();
            var statsPerWishlistRepository = repositoryFactory.GetRepository<StatsPerWishlist>();

            var organisations = userRepos.AsQueryable()
                .Where(u => !string.IsNullOrEmpty(u.Organisation))
                .Select(u => u.Organisation)
                .ToList()
                .Distinct()
                .ToList();

            var sb = new StringBuilder();
            foreach (var organisation in organisations)
            {
                var wishlist = wishlistRepos
                    .AsQueryable()
                    .FirstOrDefault(w => w.OrganisationName == organisation);
                var stats = statsPerWishlistRepository.AsQueryable()
                        .FirstOrDefault(s => s.Id == wishlist.Id);
                if (stats == null) continue;
                sb.AppendLine($"*Organisation: {organisation}*");
                sb.AppendLine("");
                sb.AppendLine($"• Accepted items: {stats.ItemsAcceptedCount}");
                sb.AppendLine($"• In review: {stats.ItemsInReviewCount}");
                sb.AppendLine($"• Rejected: {stats.ItemsRejectedCount}");
            }
            SlackHelper.PostMessage(new Payload { Text = sb.ToString() }, slackHookUrl);

            foreach (var organisation in organisations)
            {
                var wishlist = wishlistRepos
                    .AsQueryable()
                    .FirstOrDefault(w => w.OrganisationName == organisation);
                if (wishlist != null)
                {
                    // Delete items, item summaries and screenings
                    var items = itemRepos
                        .AsQueryable()
                        .Where(i => i.WishListId == wishlist.Id)
                        .ToList();

                    items.ForEach(i =>
                    {
                        screeningRepos.AsQueryable()
                            .Where(s => s.ItemId == i.Id)
                            .ToList()
                            .ForEach(s => screeningRepos.DeleteAsync(s.Id).Wait());
                    });

                    items.ForEach(i => itemRepos.DeleteAsync(i.Id).Wait());
                    itemSummaryRepos.AsQueryable().Where(i => i.WishListId == wishlist.Id).ToList()
                        .ForEach(i => itemSummaryRepos.DeleteAsync(i.Id).Wait());
                    // Create new wishlist stats
                    WishlistHelper.CreateNewWishListStats(repositoryFactory, wishlist.Id);
                }
            }
        }



        //[FunctionName("TestFunction")]
        //public static void TestRun([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        //{
        //    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        //    var config = new ConfigurationBuilder()
        //        .SetBasePath(context.FunctionAppDirectory)
        //        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        //        .AddEnvironmentVariables()
        //        .Build();
        //    var recipients = config["Recipients"].Split(';');

        //    var mongoDbConnectionString = config.GetConnectionString("MongoDB");
        //    // Setting up DI
        //    var services = new ServiceCollection();
        //    services.AddMemoryCache();
        //    services.AddMongoRepository("QConstruction", mongoDbConnectionString);
        //    Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        //    var serviceProvider = services.BuildServiceProvider();
        //    var repositoryFactory = serviceProvider.GetService<IRepositoryFactory>();
        //    var userRepos = repositoryFactory.GetRepository<User>();
        //    var wishlistRepos = repositoryFactory.GetRepository<Wishlist>();
        //    var statsPerWishlistRepository = repositoryFactory.GetRepository<StatsPerWishlist>();

        //    var organisations = userRepos.AsQueryable()
        //        .Where(u => !string.IsNullOrEmpty(u.Organisation))
        //        .Select(u => u.Organisation)
        //        .ToList()
        //        .Distinct()
        //        .ToList();
        //    if (recipients.Any())
        //    {
        //        var sb = new StringBuilder();
        //        foreach (var organisation in organisations)
        //        {
        //            var wishlist = wishlistRepos
        //                .AsQueryable()
        //                .FirstOrDefault(w => w.OrganisationName == organisation);
        //            var stats = statsPerWishlistRepository.AsQueryable().FirstOrDefault(s => s.Id == wishlist.Id);
        //            if (stats == null) continue;
        //            sb.AppendLine("<p>");
        //            sb.AppendLine($"<h3>Organisation: {organisation} </h3>");
        //            sb.AppendLine("<ul>");
        //            sb.AppendLine($"<li>Accepted items: {stats.ItemsAcceptedCount}</li>");
        //            sb.AppendLine($"<li>In review: {stats.ItemsInReviewCount}</li>");
        //            sb.AppendLine($"<li>Rejected: {stats.ItemsRejectedCount}</li>");
        //            sb.AppendLine("</ul>");
        //            sb.AppendLine("</p>");
        //        }

        //        var client = new SendGridClient(config["AzureWebJobsSendGridApiKey"]);
        //        var body = sb.ToString();
        //        if (!string.IsNullOrEmpty(body))
        //        {
        //            var content = new Content
        //            {
        //                Type = "text/html",
        //                Value = body
        //            };
        //            var message = new SendGridMessage();
        //            message.AddTos(recipients.Select(r => new EmailAddress(r)).ToList());
        //            message.Contents = new List<Content> { content };
        //            message.SetFrom(new EmailAddress("noreply@cito.nl"));
        //            message.SetSubject("Q-construction summary");
        //            client.SendEmailAsync(message).Wait();
        //        }
        //    }


        //}
    }
}
