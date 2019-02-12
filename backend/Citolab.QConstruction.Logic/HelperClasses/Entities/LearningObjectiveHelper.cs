using System;
using System.Linq;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace Citolab.QConstruction.Logic.HelperClasses.Entities
{
    /// <summary>
    ///     Helper for updating fields in learningObjectives when other entities change
    /// </summary>
    public static class LearningObjectiveHelper
    {
        //private static object _lock = new object();
        /// <summary>
        ///     Sets the domain info to the learningrepository
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="learningObjectiveRepository"></param>
        public static void SetDomainTitle(Domain domain, IRepository<LearningObjective> learningObjectiveRepository)
        {
            var learningObjectives = learningObjectiveRepository.AsQueryable().Where(l => l.DomainId == domain.Id);
            foreach (var learningObjective in learningObjectives)
            {
                learningObjective.DomainTitle = domain.Title;
                learningObjectiveRepository.UpdateAsync(learningObjective).Wait();
            }
        }

        /// <summary>
        ///     Updates the learningObjective if the wishlist changes
        /// </summary>
        /// <param name="wishlistId"></param>
        /// <param name="repositoryFactory"></param>
        public static void UpdateWishListInfoInLearningObject(Guid wishlistId, IRepositoryFactory repositoryFactory)
        {
            //lock (_lock)
            //{
            var wishlistItemRepository = repositoryFactory.GetRepository<WishlistItem>();
            var learningObjectiveRepository = repositoryFactory.GetRepository<LearningObjective>();
            var learningObjectiveIds =
                wishlistItemRepository.AsQueryable()
                    .Where(wi => wi.WishlistId == wishlistId)
                    .Select(wi => wi.LearningObjectiveId).ToList()
                    .Distinct().ToList();
            foreach (var learningObjectId in learningObjectiveIds)
            {
                var learningObjective =
                    learningObjectiveRepository.GetAsync(learningObjectId).Result;
                if (learningObjective == null) continue;
                var wishListItemsAll = wishlistItemRepository.AsQueryable().Where(wi => wi.WishlistId == wishlistId).ToList();
                var wishListItems = wishListItemsAll.Where(l => l.LearningObjectiveId == learningObjectId).ToList();
                var shortestDeadline = wishListItems.Where(wi => wi.Deadline > DateTime.Now.Date &&
                                                           wi.Todo > 0).OrderByDescending(wi => wi.Deadline).FirstOrDefault();

                if (shortestDeadline != null)
                {
                    learningObjective.Deadline = shortestDeadline.Deadline;
                }
                learningObjective.Total = wishListItems.Sum(wi => wi.NumberOfItems);
                learningObjective.CreatedCount =
                    wishListItems.Where(wi => wi.ItemIds != null).ToList().Sum(wi => wi.ItemIds.Length);
                learningObjective.Todo = learningObjective.Total - learningObjective.CreatedCount;
                learningObjectiveRepository.UpdateAsync(learningObjective).Wait();
            }
            //}
        }
    }
}