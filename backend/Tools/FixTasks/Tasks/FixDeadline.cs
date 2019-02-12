using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace FixTasks.Tasks
{
    public class FixDeadline: IFix
    {
        public void DoFix(IRepositoryFactory repositoryFactory, Guid wishlistId)
        {
            var learningObjectiveRepository = repositoryFactory.GetRepository<LearningObjective>();
            var wishlistRepository = repositoryFactory.GetRepository<Wishlist>();
            var wishlistItemRepository = repositoryFactory.GetRepository<WishlistItem>();
            var itemRepository = repositoryFactory.GetRepository<Item>();
            var itemSummaryRepository = repositoryFactory.GetRepository<ItemSummary>();
            var newDeadline = new DateTime(2017, 9, 1);
            var items = itemRepository.AsQueryable().Where(i => i.WishListId == wishlistId).ToList();
            var itemsummary = itemSummaryRepository.AsQueryable().Where(i => i.WishListId == wishlistId).ToList();
            items.ForEach(item =>
            {
                item.Deadline = newDeadline;
                itemRepository.UpdateAsync(item).Wait();
            });
            itemsummary.ForEach(item =>
            {
                item.Deadline = newDeadline;
                itemSummaryRepository.UpdateAsync(item).Wait();
            });
            var wishlist = wishlistRepository.GetAsync(wishlistId).Result;
            foreach (var wishlistWishListItem in wishlist.WishListItems)
            {
                wishlistWishListItem.Deadline = newDeadline;
                var learningObjective = learningObjectiveRepository.GetAsync(wishlistWishListItem.LearningObjectiveId).Result;
                learningObjective.Deadline = newDeadline;
                learningObjectiveRepository.UpdateAsync(learningObjective);
            }
            wishlistRepository.UpdateAsync(wishlist);
            var wishlistItems = wishlistItemRepository.AsQueryable().Where(wi => wi.WishlistId == wishlistId).ToList();
            wishlistItems.ForEach(item =>
            {
                item.Deadline = newDeadline;
                wishlistItemRepository.UpdateAsync(item).Wait();
            });
            var learningObjectives = learningObjectiveRepository.AsQueryable().Where(l => l.WishlistId == wishlistId).ToList();
            foreach (var learningObjective in learningObjectives)
            {
                learningObjective.Deadline = newDeadline;
                learningObjectiveRepository.UpdateAsync(learningObjective).Wait();
            }

    }
    }
}
