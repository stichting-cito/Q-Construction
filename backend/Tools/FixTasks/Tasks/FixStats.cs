using System;
using System.Collections.Generic;
using System.Linq;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace FixTasks.Tasks
{
    public class FixStats : IFix
    {
        public void DoFix(IRepositoryFactory repositoryFactory, Guid wishlistId)
        {
            var learningObjectiveRepository = repositoryFactory.GetRepository<LearningObjective>();
            var itemRepository = repositoryFactory.GetRepository<Item>();
            var wishlistRepository = repositoryFactory.GetRepository<Wishlist>();
            var userRepos = repositoryFactory.GetRepository<User>();
            var users = userRepos.AsQueryable().ToList();
            var wishlist = wishlistRepository.GetAsync(wishlistId).Result;
            var items = itemRepository.AsQueryable().Where(i => i.WishListId == wishlistId).ToList();
            var as12 = items.Where(i => i.Created > new DateTime(2017, 7, 24)).ToList();
            var userStatsRepos = repositoryFactory.GetRepository<UserItemStatusCount>();
            var userIds = users.Select(u => u.Id).ToList();
            var userStats = userStatsRepos.AsQueryable().Where(u => userIds.Contains(u.UserId));
            var statsRepos = repositoryFactory.GetRepository<StatsPerWishlist>();
            var wishlistItems = repositoryFactory.GetRepository<WishlistItem>()
                .AsQueryable()
                .Where(wi => wi.WishlistId == wishlistId)
                .ToList();
            var wishlistStats = statsRepos.GetAsync(wishlistId).Result;
            var learningObjectives = learningObjectiveRepository.AsQueryable().Where(l => l.WishlistId == wishlistId).ToList();
            wishlistStats.ItemsAcceptedCount = items.Count(i => i.ItemStatus == ItemStatus.Accepted);
            wishlistStats.ItemsInReviewCount = items
                .Count(i => i.ItemStatus == ItemStatus.ReadyForReview || i.ItemStatus == ItemStatus.InReview || i.ItemStatus == ItemStatus.NeedsWork);
            wishlistStats.ItemsTodoCount = wishlistStats.ItemTargetCount - wishlistStats.ItemsAcceptedCount;
            wishlistStats.ItemsRejectedCount = items.Count(i => i.ItemStatus == ItemStatus.Rejected);
            UpdateCalculatedStats(wishlistStats);
            foreach (var user in users)
            {
                var stats = userStats.Where(u => u.UserId == user.Id && u.WishlistId == wishlistId).ToList();
                foreach (var stat in stats)
                {
                    var count = items.Count(i => i.CreatedByUserId == user.Id && i.ItemStatus == stat.ItemStatus);
                    if (stat.Count != count)
                    {
                        stat.Count = count;
                        var _ = userStatsRepos.UpdateAsync(stat).Result;
                    }
                }
            }
            var wlIsChanged = false;
            var wishlistItemRepository = repositoryFactory.GetRepository<WishlistItem>();
            foreach (var wi in wishlistItems)
            {
                var todo = wi.NumberOfItems - items.Count(i => i.LearningObjectiveId == wi.LearningObjectiveId && i.ItemStatus != ItemStatus.Deleted && i.ItemStatus != ItemStatus.Rejected);
                var wiInWishlist = wishlist.WishListItems.FirstOrDefault(w => w.Id == wi.Id);
                if (wiInWishlist?.Todo != todo)
                {
                    wiInWishlist.Todo = todo;
                    wlIsChanged = true;
                }
                if (todo != wi.Todo)
                {
                    wi.Todo = todo;
                    wishlistItemRepository.UpdateAsync(wi).Wait();
                }
                var itemItems = items.Where(i => i.LearningObjectiveId == wi.LearningObjectiveId && i.ItemStatus != ItemStatus.Deleted && i.ItemStatus != ItemStatus.Rejected)
                        .Select(i => i.Id).ToArray();
                if (!wi.ItemIds.OrderBy(t => t).SequenceEqual(itemItems.OrderBy(t => t)))
                {
                    wi.ItemIds = itemItems;
                    wiInWishlist.ItemIds = itemItems;
                    wishlistItemRepository.UpdateAsync(wi);
                    wlIsChanged = true;
                }

            }
            if (wlIsChanged) wishlistRepository.UpdateAsync(wishlist).Wait();
            foreach (var statsperDomain in wishlistStats.StatsPerDomain)
            {
                var learningObjectivesIdsPerDomain =
                    learningObjectives.Where(l => l.DomainId == statsperDomain.DomainId).Select(l => l.Id).ToList();
                var itemsPerDomain = items.Where(i => learningObjectivesIdsPerDomain.Contains(i.LearningObjectiveId)).ToList();
                statsperDomain.ItemsAcceptedCount = itemsPerDomain
                    .Count(i => i.ItemStatus == ItemStatus.Accepted);
                statsperDomain.ItemsRejectedCount = itemsPerDomain
                    .Count(i => i.ItemStatus == ItemStatus.Rejected);
                statsperDomain.PercentageAccepted = (int)Math.Round((decimal)statsperDomain.ItemsAcceptedCount / statsperDomain.TotalItemCount * 100);
            }
            var datesCount = new List<DateCount>();
            var lIds = wishlistStats.StatsPerDomain.Select(s => s.DomainId).ToList();
            var learningObjectivesInWishlist = learningObjectives.Where(l => lIds.Contains(l.DomainId)).ToList();
            foreach (var d in learningObjectivesInWishlist.Select(l => l.Deadline).Distinct().ToList())
            {
                if (d.HasValue)
                {
                    var learningObjectivesWithThisDeadline = learningObjectivesInWishlist.Where(l => l.Deadline == d).Select(l => l.Id).ToList();
                    var numberOfAccepted =
                        items.Count(i => learningObjectivesWithThisDeadline.Contains(i.LearningObjectiveId));
                    var total = wishlistItems.Where(
                        wi => learningObjectivesWithThisDeadline.Contains(wi.LearningObjectiveId)).Sum(wi => wi.NumberOfItems);
                    var count = (total - numberOfAccepted);
                    if (count != 0)
                    {
                        datesCount.Add(new DateCount { Count = (total - numberOfAccepted), Date = d.Value });
                    }

                }

            }
            foreach (var learningObjective in learningObjectivesInWishlist)
            {
                var created = items.Count(
                    i => i.LearningObjectiveId == learningObjective.Id && i.ItemStatus != ItemStatus.Deleted && i.ItemStatus != ItemStatus.Rejected);
                if (learningObjective.Todo != learningObjective.Total - created)
                {
                    learningObjective.Todo = learningObjective.Total - created;
                    learningObjective.CreatedCount = created;
                    learningObjectiveRepository.UpdateAsync(learningObjective).Wait();
                }
            }


            wishlistStats.ItemDeadlinesWithCounts = datesCount;
            statsRepos.UpdateAsync(wishlistStats);

        }


        private static void UpdateCalculatedStats(StatsPerWishlist stats)
        {
            stats.PercentageAccepted =
                (int)Math.Round((decimal)stats.ItemsAcceptedCount / stats.ItemTargetCount * 100);
            if (stats.ItemsRejectedCount > 0)
            {
                stats.PercentageMortality =
                    (int)
                    Math.Round((decimal)stats.ItemsRejectedCount /
                               (stats.ItemsAcceptedCount + stats.ItemsRejectedCount) * 100);
            }
            if (stats.ItemsRejectedCount > 0 || stats.ItemsAcceptedCount > 0)
            {
                stats.MeanReviewIterations =
                    Math.Round((decimal)stats.IterationCount / (stats.ItemsAcceptedCount + stats.ItemsRejectedCount), 2);
            }
        }
    }
}

