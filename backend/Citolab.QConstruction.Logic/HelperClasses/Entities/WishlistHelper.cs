using System;
using System.Collections.Generic;
using System.Linq;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace Citolab.QConstruction.Logic.HelperClasses.Entities
{
    /// <summary>
    ///     Helper function to update the wishlist if an item is changed
    /// </summary>
    public static class WishlistHelper
    {
        /// <summary>
        ///     UpdateWishListWhenItemChanged
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="item"></param>
        /// <param name="beforeSave"></param>
        /// <param name="repositoryFactory"></param>
        public static void UpdateWishListWhenItemChanged(Guid itemId, Item item, Item beforeSave,
            IRepositoryFactory repositoryFactory)
        {
            if (beforeSave != null && item != null && item.ItemStatus == beforeSave.ItemStatus) return;
            var wishlistItemRepository = repositoryFactory.GetRepository<WishlistItem>();
            var wishlistRepository = repositoryFactory.GetRepository<Wishlist>();
            var wishListItem = item == null
                ? wishlistItemRepository.AsQueryable().FirstOrDefault(wi => wi.ItemIds.Any(i => i == itemId))
                : wishlistItemRepository.AsQueryable()
                    .FirstOrDefault(
                        wi => wi.LearningObjectiveId == item.LearningObjectiveId && wi.WishlistId == item.WishListId);
            if (wishListItem == null) return;
            var wishlistId = item?.WishListId ?? beforeSave.WishListId;
            var learningObjectiveId = item?.LearningObjectiveId ?? beforeSave.LearningObjectiveId;
            UpdateStatus(wishlistId, learningObjectiveId, item, beforeSave, wishlistItemRepository, wishlistRepository);
            LearningObjectiveHelper.UpdateWishListInfoInLearningObject(wishListItem.WishlistId, repositoryFactory);
            UpdateStatsPerWishlist(item, wishListItem, beforeSave?.ItemStatus, repositoryFactory);
        }

        /// <summary>
        ///     Updates the wishlist stats
        /// </summary>
        /// <param name="repositoryFactor"></param>
        /// <param name="wishlistId"></param>
        public static async void CreateNewWishListStats(IRepositoryFactory repositoryFactor, Guid wishlistId)
        {
            var statsPerWishlistRepository = repositoryFactor.GetRepository<StatsPerWishlist>();
            var wishlistItemRepository = repositoryFactor.GetRepository<WishlistItem>();
            var wItems = wishlistItemRepository.AsQueryable().Where(wi => wi.WishlistId == wishlistId).ToList();
            var stats = await statsPerWishlistRepository.GetAsync(wishlistId) ??
                        statsPerWishlistRepository.AddAsync(new StatsPerWishlist {Id = wishlistId}).Result;
            var totalItems = wItems.Sum(wi => wi.NumberOfItems);
            var domainStats = new List<DomainStats>();
            var deadlines = new List<DateCount>();
            wItems.ForEach(w =>
            {
                var deadline = deadlines.FirstOrDefault(d => d.Date == w.Deadline.Date);
                if (deadline == null)
                {
                    deadline = new DateCount {Count = 0, Date = w.Deadline.Date};
                    deadlines.Add(deadline);
                }
                deadline.Count += w.NumberOfItems;
            });

            wItems.Select(wi => wi.DomainId).Distinct().ToList().ForEach(dId =>
            {
                var domainRepository = repositoryFactor.GetRepository<Domain>();
                var domain = domainRepository.GetAsync(dId).Result;
                domainStats.Add(new DomainStats
                {
                    DomainId = dId,
                    DomainName = domain.Title,
                    ItemsAcceptedCount = 0,
                    ItemsRejectedCount = 0,
                    TotalItemCount =
                        wishlistItemRepository
                            .AsQueryable()
                            .Where(wi => wi.WishlistId == wishlistId && wi.DomainId == domain.Id)
                            .Sum(wi => wi.NumberOfItems),
                    IterationCount = 0,
                    MeanReviewIterations = 0,
                    PercentageAccepted = 0
                });
            });
            stats.ItemsAcceptedCount = 0;
            stats.ItemsInReviewCount = 0;
            stats.ItemsRejectedCount = 0;
            stats.PercentageAccepted = 0;
            stats.PercentageMortality = 0;
            stats.MeanReviewIterations = 0;
            stats.ItemTargetCount = totalItems;
            stats.ItemsTodoCount = totalItems;
            stats.ItemDeadlinesWithCounts = deadlines;
            stats.StatsPerDomain = domainStats;
            await statsPerWishlistRepository.UpdateAsync(stats);
        }

        private static async void UpdateStatsPerWishlist(Item item, WishlistItem wishlistItem,
            ItemStatus? previousItemStatus, IRepositoryFactory repositoryFactory)
        {
            if (item == null)
            {
                throw new ArgumentException("Item should not be null.", nameof(item));
            }
            if (default(Guid) == item.CreatedByUserId)
            {
                throw new ArgumentException("UserId in CreatedByUserId should not be null.",
                    nameof(item.CreatedByUserId));
            }
            var statsPerWishlistRepository = repositoryFactory.GetRepository<StatsPerWishlist>();
            var learningObjectiveRepository = repositoryFactory.GetRepository<LearningObjective>();
            var domainRepository = repositoryFactory.GetRepository<Domain>();
            var userRepository = repositoryFactory.GetRepository<User>();
            var wishlistItemRepository = repositoryFactory.GetRepository<WishlistItem>();
            var screeningRepository = repositoryFactory.GetRepository<Screening>();
            var screeningItemRepository = repositoryFactory.GetRepository<ScreeningItem>();

            var learningObjective = learningObjectiveRepository.GetAsync(wishlistItem.LearningObjectiveId).Result;
            var domain = domainRepository.GetAsync(learningObjective.DomainId).Result;
            var users = new List<User> {userRepository.GetAsync(item.CreatedByUserId).Result};
            if (item.LatestScreeningId.HasValue &&
                (item.ItemStatus == ItemStatus.Accepted || item.ItemStatus == ItemStatus.Rejected ||
                 item.ItemStatus == ItemStatus.NeedsWork))
            {
                var screening = screeningRepository.GetAsync(item.LatestScreeningId.Value).Result;
                users.Add(userRepository.GetAsync(screening.CreatedByUserId).Result);
            }
            var stats = await statsPerWishlistRepository.GetAsync(item.WishListId);
            if (stats == null)
            {
                CreateNewWishListStats(repositoryFactory, item.WishListId);
                stats = await statsPerWishlistRepository.GetAsync(item.WishListId);
            }

            var domainStats = stats.StatsPerDomain.SingleOrDefault(ds => ds.DomainId == learningObjective.DomainId);
            if (domainStats == null)
            {
                stats.StatsPerDomain.Add(new DomainStats
                {
                    DomainId = domain.Id,
                    DomainName = domain.Title,
                    ItemsAcceptedCount = 0,
                    ItemsRejectedCount = 0,
                    TotalItemCount =
                        wishlistItemRepository.AsQueryable()
                            .Where(wi => wi.WishlistId == item.WishListId && wi.DomainId == domain.Id)
                            .Sum(wi => wi.NumberOfItems),
                    IterationCount = 0,
                    MeanReviewIterations = 0,
                    PercentageAccepted = 0
                });
            }
            var usersStats = users.Select(u => GetUserStats(stats, u)).ToList();

            //update stats
            switch (item.ItemStatus)
            {
                case ItemStatus.Draft:
                    // Item is not counted when in draft mode.
                    break;
                case ItemStatus.ReadyForReview:
                    if (previousItemStatus == ItemStatus.Draft) // item enters review cycle
                    {
                        stats.ItemsInReviewCount++;
                        stats.ItemsTodoCount--;
                    }
                    break;
                case ItemStatus.InReview:
                    break;
                case ItemStatus.NeedsWork:
                    if (item.LatestScreeningId.HasValue)
                    {
                        var screening = screeningRepository.GetAsync(item.LatestScreeningId.Value).Result;
                        UpdateStatsOnReviewIteration(stats, domainStats, screening, screeningItemRepository, usersStats);
                    }
                    break;
                case ItemStatus.Accepted:

                    UpdateStatsOnAccepted(wishlistItem, stats, domainStats, usersStats);
                    break;
                case ItemStatus.Rejected:
                    var r =
                        screeningRepository.AsQueryable()
                            .Count(
                                s =>
                                    s.ItemId == item.Id && s.Status == ScreeningStatus.Final && s.FeedbackList != null &&
                                    s.FeedbackList.Length > 0);
                    UpdateStatsOnRejected(stats, domainStats, usersStats, r, item.Id);
                    break;
                case ItemStatus.Deleted:
                    var rounds =
                        screeningRepository.AsQueryable()
                            .Count(
                                s =>
                                    s.ItemId == item.Id && s.Status == ScreeningStatus.Final && s.FeedbackList != null &&
                                    s.FeedbackList.Length > 0);
                    UpdateStatsOnDeleted(previousItemStatus, stats, domainStats, usersStats, rounds, item.Id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await statsPerWishlistRepository.UpdateAsync(stats);
        }

        private static UserStats GetUserStats(StatsPerWishlist stats, User user)
        {
            var userStats = stats.StatsPerUser.SingleOrDefault(us => us.UserId == user.Id && user.UserType.HasValue);
            if (userStats != null) return userStats;
            userStats = new UserStats
            {
                UserId = user.Id,
                Picture = user.Picture,
                UserName = user.Name,
                // ReSharper disable once PossibleInvalidOperationException
                UserType = user.UserType.Value,
                ItemsAcceptedCount = 0,
                ItemsRejectedCount = 0,
                MeanReviewIterations = 0,
                IterationCount = 0,
                PercentageRejected = 0
            };
            stats.StatsPerUser.Add(userStats);
            return userStats;
        }

        private static void UpdateCalculatedStats(StatsPerWishlist stats)
        {
            stats.PercentageAccepted =
                (int) Math.Round((decimal) stats.ItemsAcceptedCount / stats.ItemTargetCount * 100);
            if (stats.ItemsRejectedCount > 0)
            {
                stats.PercentageMortality =
                    (int)
                    Math.Round((decimal) stats.ItemsRejectedCount /
                               (stats.ItemsAcceptedCount + stats.ItemsRejectedCount) * 100);
            }
            if (stats.ItemsRejectedCount > 0 || stats.ItemsAcceptedCount > 0)
            {
                stats.MeanReviewIterations =
                    Math.Round((decimal) stats.IterationCount / (stats.ItemsAcceptedCount + stats.ItemsRejectedCount), 2);
            }
        }

        private static void UpdateCalculatedDomainStats(DomainStats domainStats)
        {
            domainStats.PercentageAccepted =
                (int) Math.Round((decimal) domainStats.ItemsAcceptedCount / domainStats.TotalItemCount * 100);
            if (domainStats.ItemsRejectedCount > 0 || domainStats.ItemsAcceptedCount > 0)
            {
                domainStats.MeanReviewIterations =
                    Math.Round(
                        (decimal) domainStats.IterationCount /
                        (domainStats.ItemsAcceptedCount + domainStats.ItemsRejectedCount), 2);
            }
        }

        private static void UpdateCalculatedUserStats(UserStats userStats)
        {
            if (userStats.ItemsRejectedCount > 0)
            {
                userStats.PercentageRejected =
                    Math.Round((decimal) userStats.ItemsRejectedCount /
                               (userStats.ItemsAcceptedCount + userStats.ItemsRejectedCount) * 100);
            }
            if (userStats.ItemsRejectedCount > 0 || userStats.ItemsAcceptedCount > 0)
            {
                userStats.MeanReviewIterations =
                    Math.Round(
                        (decimal) userStats.IterationCount /
                        (userStats.ItemsAcceptedCount + userStats.ItemsRejectedCount), 2);
            }
        }

        private static void UpdateStatsOnDeleted(ItemStatus? previousItemStatus, StatsPerWishlist stats,
            DomainStats domainStats,
            List<UserStats> usersStats, int roundCount, Guid itemId)
        {
            if (previousItemStatus.HasValue && previousItemStatus != ItemStatus.Draft)
            {
                stats.ItemsInReviewCount--;
                stats.ItemsTodoCount++;
            }
            if (previousItemStatus == ItemStatus.NeedsWork)
            {
                stats.ScreeningRoundsOfWastedItems.Add(new WastedItem
                {
                    ItemId = itemId,
                    RemovedByAuthor = true,
                    Rounds = roundCount
                });
                stats.IterationCount++;
                stats.ItemsRejectedCount++;
                UpdateCalculatedStats(stats);

                domainStats.IterationCount++;
                domainStats.ItemsRejectedCount++;
                UpdateCalculatedDomainStats(domainStats);
                foreach (var userStats in usersStats)
                {
                    userStats.IterationCount++;
                    userStats.ItemsRejectedCount++;
                    UpdateCalculatedUserStats(userStats);
                }
            }
        }

        private static void UpdateStatsOnRejected(StatsPerWishlist stats, DomainStats domainStats,
            List<UserStats> usersStats, int roundCount, Guid itemId)
        {
            stats.IterationCount++;
            stats.ItemsInReviewCount--;
            stats.ItemsRejectedCount++;
            stats.ItemsTodoCount++;
            stats.ScreeningRoundsOfWastedItems.Add(new WastedItem
            {
                ItemId = itemId,
                RemovedByAuthor = false,
                Rounds = roundCount
            });
            UpdateCalculatedStats(stats);

            domainStats.ItemsRejectedCount++;
            domainStats.IterationCount++;
            UpdateCalculatedDomainStats(domainStats);

            foreach (var userStats in usersStats)
            {
                userStats.ItemsRejectedCount++;
                userStats.IterationCount++;
                UpdateCalculatedUserStats(userStats);
            }
        }


        private static void UpdateStatsOnAccepted(WishlistItem wishlistItem, StatsPerWishlist stats,
            DomainStats domainStats,
            List<UserStats> usersStats)
        {
            stats.IterationCount++;
            stats.ItemsInReviewCount--;
            stats.ItemsAcceptedCount++;
            UpdateCalculatedStats(stats);

            var s = stats.ItemDeadlinesWithCounts?.FirstOrDefault(d => d.Date == wishlistItem.Deadline.Date);
            if (s != null) s.Count--;
            var deadline = stats.ItemsAcceptedPerDayCumulative.FirstOrDefault(d => d.Date == DateTime.Today.Date);
            if (deadline == null)
            {
                deadline = new DateCount {Count = 0, Date = DateTime.Today.Date};
                stats.ItemsAcceptedPerDayCumulative.Add(deadline);
            }
            deadline.Count++;
            domainStats.IterationCount++;
            domainStats.ItemsAcceptedCount++;
            UpdateCalculatedDomainStats(domainStats);

            foreach (var userStats in usersStats)
            {
                userStats.IterationCount++;
                userStats.ItemsAcceptedCount++;
                UpdateCalculatedUserStats(userStats);
            }
        }

        private static void UpdateStatsOnReviewIteration(StatsPerWishlist stats, DomainStats domainStats,
            Screening screening, IRepository<ScreeningItem> screeningItemRepository, List<UserStats> usersStats)
        {
            stats.IterationCount++;
            domainStats.IterationCount++;
            usersStats.ForEach(us => us.IterationCount++);
            if (screening.FeedbackList != null)
            {
                foreach (var feedback in screening.FeedbackList)
                {
                    var screeningItem = screeningItemRepository.GetAsync(feedback.ScreeningItemId).Result;

                    var domainScreeningItemStat =
                        domainStats.ScreeningItemStatsList.SingleOrDefault(
                            s => s.ScreeningItemId == feedback.ScreeningItemId);
                    if (domainScreeningItemStat == null)
                    {
                        domainScreeningItemStat = new ScreeningItemStats
                        {
                            ScreeningItemId = screeningItem.Id,
                            ScreeningItemName = screeningItem.Value,
                            UseCount = 0
                        };
                        domainStats.ScreeningItemStatsList.Add(domainScreeningItemStat);
                    }
                    domainScreeningItemStat.UseCount++;

                    foreach (var userStats in usersStats)
                    {
                        var userScreeningItemStat =
                            userStats.ScreeningItemStatsList.SingleOrDefault(s => s.ScreeningItemId == screeningItem.Id);
                        if (userScreeningItemStat == null)
                        {
                            userScreeningItemStat = new ScreeningItemStats
                            {
                                ScreeningItemId = screeningItem.Id,
                                ScreeningItemName = screeningItem.Value,
                                UseCount = 0
                            };
                            userStats.ScreeningItemStatsList.Add(userScreeningItemStat);
                        }
                        userScreeningItemStat.UseCount++;
                    }
                    // overall screening item counts
                    var screeningItemStat =
                        stats.ScreeningItemStats.SingleOrDefault(s => s.ScreeningItemId == screeningItem.Id);
                    if (screeningItemStat == null)
                    {
                        screeningItemStat = new ScreeningItemStats
                        {
                            ScreeningItemId = screeningItem.Id,
                            ScreeningItemName = screeningItem.Value,
                            UseCount = 0
                        };
                        stats.ScreeningItemStats.Add(screeningItemStat);
                    }
                    screeningItemStat.UseCount++;
                }
            }

            UpdateCalculatedStats(stats);
            UpdateCalculatedDomainStats(domainStats);
            foreach (var userStats in usersStats)
            {
                UpdateCalculatedUserStats(userStats);
            }
        }

        private static void UpdateStatus(Guid wishListId, Guid learningObjective, Item saved, Item before,
            IRepository<WishlistItem> wishListItemRepository, IRepository<Wishlist> wishListRepository)
        {
            var wishListItem =
                wishListItemRepository.AsQueryable()
                    .FirstOrDefault(s => s.WishlistId == wishListId && s.LearningObjectiveId == learningObjective);
            if (wishListItem == null) return;
            var accepted = 0;
            var items = wishListItem.ItemIds?.ToList() ?? new List<Guid>();

            var states = wishListItem.ItemStatusCount?.ToList() ?? new List<ItemStatusCount>();
            if (saved != null)
            {
                if (before == null) items.Add(saved.Id);

                accepted = saved.ItemStatus == ItemStatus.Accepted ? 1 : 0;
                var statusCountIncremented = states.FirstOrDefault(s => s.ItemStatus == saved.ItemStatus);
                if (statusCountIncremented == null)
                    states.Add(new ItemStatusCount(wishListId, saved.ItemStatus, 1));
                else
                {
                    statusCountIncremented.Count += 1;
                }
            }
            if (before != null)
            {
                if (saved == null || saved.IsDeleted || saved.ItemStatus == ItemStatus.Deleted || saved.ItemStatus == ItemStatus.Rejected)
                {
                    var i = items.FirstOrDefault(item => item == before.Id);
                    if (i != default(Guid))
                    {
                        items.Remove(i);
                    }
                }
                accepted = before.ItemStatus == ItemStatus.Accepted && saved != null &&
                           saved.ItemStatus != ItemStatus.Accepted
                    ? -1
                    : accepted;
                var statusCountDecremented = states.FirstOrDefault(s => s.ItemStatus == before.ItemStatus);
                if (statusCountDecremented != null) statusCountDecremented.Count = statusCountDecremented.Count - 1;
            }
            wishListItem.ItemIds = items.ToArray();
            wishListItem.ItemStatusCount = states.ToArray();
            wishListItem.Todo = wishListItem.Todo - accepted;

            var wishList = wishListRepository.GetAsync(wishListId).Result;
            var itemInWishList = wishList.WishListItems.FirstOrDefault(wi => wi.Id == wishListItem.Id);
            var index = itemInWishList == null ? null : wishList.WishListItems?.IndexOf(itemInWishList);
            if (index.HasValue) wishList.WishListItems.RemoveAt(index.Value);
            if (wishList.WishListItems == null) wishList.WishListItems = new List<WishlistItem>();
            wishList.WishListItems.Add(wishListItem);
            wishListItemRepository.UpdateAsync(wishListItem).Wait();
            wishListRepository.UpdateAsync(wishList).Wait();
        }
    }
}