using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace Citolab.QConstruction.Logic.HelperClasses.Entities
{
    /// <summary>
    ///     Helper  to add extension fields
    /// </summary>
    public static class ItemHelper
    {
        /// <summary>
        ///     Add related info
        /// </summary>
        /// <param name="item"></param>
        /// <param name="repositoryFactory"></param>
        public static void AddRelatedInfo(Item item, IRepositoryFactory repositoryFactory, ILoggedInUserProvider loggedInUser)
        {
            var learningObjectiveRepository = repositoryFactory.GetRepository<LearningObjective>();
            var wishlistRepository = repositoryFactory.GetRepository<Wishlist>();
            var wishlistItemRepository = repositoryFactory.GetRepository<WishlistItem>();
            var itemRepository = repositoryFactory.GetRepository<Item>();

            if (item == null) return;
            if (!string.IsNullOrEmpty(item.LearningObjectiveTitle)) return;
            var learningObjective = learningObjectiveRepository.GetAsync(item.LearningObjectiveId).Result;
            var wishlist = wishlistRepository.GetAsync(item.WishListId).Result;
            item.CreatedByUserId = loggedInUser.GetUserId() ?? Guid.Empty;
            item.LearningObjectiveTitle = learningObjective?.Title;
            item.LearningObjectiveCode = learningObjective?.Code;
            item.DomainTitle = learningObjective?.DomainTitle;
            item.WishListTitle = wishlist?.Title;
            var wishlistItem =
                wishlistItemRepository.AsQueryable()
                    .FirstOrDefault(
                        l => l.WishlistId == item.WishListId && l.LearningObjectiveId == item.LearningObjectiveId);
            item.Deadline = wishlistItem?.Deadline;
            var lastSeq = itemRepository.AsQueryable()
                .Where(i => i.LearningObjectiveId == item.LearningObjectiveId && !string.IsNullOrEmpty(i.UniqueCode))
                .Select(i => i.UniqueCode)
                .ToList()
                .Where(c => c.IndexOf(item.LearningObjectiveCode, StringComparison.Ordinal) != -1)
                .Select(i =>
                {
                    var seq = i.Split('_').LastOrDefault();
                    return int.TryParse(seq, out var nr) ? nr : 1;
                })
                .OrderByDescending(c => c)
                .FirstOrDefault();
            lastSeq++;
            item.UniqueCode = $"{item.LearningObjectiveCode}_{lastSeq}";
        }

        /// <summary>
        ///     Update a versioned item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="repositoryFactory"></param>
        public static void UpdateVersionedItem(Item item, IRepositoryFactory repositoryFactory)
        {
            var versionedItemRepository = repositoryFactory.GetRepository<VersionedItem>();

            var versionedItem = versionedItemRepository.GetAsync(item.Id).Result;
            var isNew = versionedItem == null;
            if (isNew) versionedItem = new VersionedItem();
            versionedItem = StartVersioning(versionedItem, item);
            if (isNew) versionedItemRepository.AddAsync(versionedItem).Wait();
            if (!isNew) versionedItemRepository.UpdateAsync(versionedItem).Wait();
        }

        /// <summary>
        ///     Update item summary
        /// </summary>
        /// <param name="before"></param>
        /// <param name="saved"></param>
        /// <param name="repositoryFactory"></param>
        public static async void UpdateItemSummary(Item before, Item saved, IRepositoryFactory repositoryFactory)
        {
            var itemSummaryRepository = repositoryFactory.GetRepository<ItemSummary>();
            var userRepository = repositoryFactory.GetRepository<User>();

            var itemSummary = itemSummaryRepository.GetAsync(saved.Id).Result;
            var isNew = itemSummary == null;
            if (isNew)
            {
                var author = await userRepository.GetAsync(saved.CreatedByUserId);
                var authorName = author?.Name ?? "Unknown";
                itemSummary = new ItemSummary {Author = authorName};
            }
            itemSummary = SyncSummary(itemSummary, saved);
            if (before != null && before.ItemStatus != saved.ItemStatus)
            {
                var screeningRepository = repositoryFactory.GetRepository<Screening>();
                var screenings =
                    screeningRepository.AsQueryable().Where(s => s.ItemId == saved.Id && s.IsDeleted != true).ToList();
                if (saved.ItemStatus == ItemStatus.Accepted || saved.ItemStatus == ItemStatus.Rejected
                    || saved.ItemStatus == ItemStatus.NeedsWork || saved.ItemStatus == ItemStatus.Deleted)
                {
                    var userIds = screenings.Select(s => s.CreatedByUserId).Distinct().ToList();
                    var screeners = userRepository.AsQueryable().Where(u => userIds.Contains(u.Id)).Select(u => u.Name);
                    itemSummary.ScreeningCount = screenings.Count;
                    // ReSharper disable once CoVariantArrayConversion
                    itemSummary.Screeners = string.Join(", ", (object[]) screeners.ToArray());
                }
                if (saved.ItemStatus == ItemStatus.Accepted && saved.LatestScreeningId.HasValue)
                {
                    var latest = screenings.FirstOrDefault(s => s.Id == saved.LatestScreeningId.Value);
                    if (latest != null)
                    {
                        var user = userRepository.GetAsync(latest.CreatedByUserId).Result;
                        itemSummary.AcceptedBy = user?.Name;
                    }
                }
            }
            DebugAssert(itemSummary);
            if (isNew) await itemSummaryRepository.AddAsync(itemSummary);
            if (!isNew) await itemSummaryRepository.UpdateAsync(itemSummary);
        }

        // ReSharper disable once UnusedParameter.Local
        private static void DebugAssert(ItemSummary item)
        {
            Debug.Assert(
                !((item.ItemStatus == ItemStatus.Accepted || item.ItemStatus == ItemStatus.Rejected) &&
                  item.LatestScreeningId == null));
            Debug.Assert(
                !((item.ItemStatus == ItemStatus.Accepted || item.ItemStatus == ItemStatus.Rejected) &&
                  item.LatestScreeningAuthorId == null));
            Debug.Assert(item.Id != Guid.Empty);
        }

        /// <summary>
        ///     Sets the version of the Item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="repositoryFactory"></param>
        /// <returns></returns>
        public static Item SetVersion(Item item, IRepositoryFactory repositoryFactory)
        {
            var versionedItemRepository = repositoryFactory.GetRepository<VersionedItem>();

            var versionedItem = versionedItemRepository.GetAsync(item.Id).Result;
            if (versionedItem != null && versionedItem.ItemStatus == item.ItemStatus) return item;
            item.Version++;
            return item;
        }

        private static ItemSummary SyncSummary(ItemSummary summary, Item item)
        {
            foreach (var itemProperty in typeof(Item).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var itemValue = item.GetType().GetProperty(itemProperty.Name).GetValue(item, null);
                var itemSummaryValue = summary.GetType().GetProperty(itemProperty.Name);
                itemSummaryValue?.SetValue(summary, itemValue, null);
            }
            return summary;
        }

        private static VersionedItem StartVersioning(VersionedItem versionedItem, Item item)
        {
            if (item == null)
            {
                versionedItem.IsDeleted = true;
                return versionedItem;
            }
            versionedItem.Id = item.Id;
            //if (versionedItem.ItemStatus == item.ItemStatus) return versionedItem;
            foreach (var itemProperty in typeof(Item).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var itemValue = item.GetType().GetProperty(itemProperty.Name).GetValue(item, null);
                var itemVersionedValue = versionedItem.GetType().GetProperty(itemProperty.Name);
                itemVersionedValue?.SetValue(versionedItem, itemValue);
            }
            return versionedItem;
        }

        /// <summary>
        ///     Update the item status
        /// </summary>
        /// <param name="before"></param>
        /// <param name="saved"></param>
        /// <param name="repositoryFactory"></param>
        public static async void UpdateStatus(Item before, Item saved, IRepositoryFactory repositoryFactory)
        {
            var userItemStates = repositoryFactory.GetRepository<UserItemStatusCount>();
            if (before == null && saved == null ||
                before != null && saved != null && before.ItemStatus == saved.ItemStatus) return;
            var userId = saved?.CreatedByUserId ?? before.CreatedByUserId;
            var wishlistId = saved?.WishListId ?? before.WishListId;
            if (saved != null)
            {
                var statusCountIncremented =
                    userItemStates.AsQueryable()
                        .FirstOrDefault(
                            u => u.UserId == userId && u.WishlistId == wishlistId && u.ItemStatus == saved.ItemStatus);
                if (statusCountIncremented == null)
                    await userItemStates.AddAsync(new UserItemStatusCount(userId, saved.WishListId, saved.ItemStatus, 1));
                else
                {
                    statusCountIncremented.Count += 1;
                    await userItemStates.UpdateAsync(statusCountIncremented);
                }
            }
            if (before == null) return;
            {
                var statusCountDecremented =
                    userItemStates.AsQueryable()
                        .FirstOrDefault(
                            u => u.UserId == userId && u.WishlistId == wishlistId && u.ItemStatus == before.ItemStatus);
                if (statusCountDecremented == null) return;
                statusCountDecremented.Count = statusCountDecremented.Count - 1;
                await userItemStates.UpdateAsync(statusCountDecremented);
            }
        }
    }
}