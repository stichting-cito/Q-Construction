using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace FixTasks.Tasks
{
    public class FixLearningObjectives : IFix
    {

        public void DoFix(IRepositoryFactory repositoryFactory, Guid wishlistId)
        {
            var itemRepos = repositoryFactory.GetRepository<Item>();
            var wishlistRepository = repositoryFactory.GetRepository<Wishlist>();
            var learningObjectiveRepository = repositoryFactory.GetRepository<LearningObjective>();
            var itemSummaryRepos = repositoryFactory.GetRepository<Item>();
            var items = itemRepos.AsQueryable().Where(i => i.WishListId == wishlistId).ToList();
            var itemsummaries = itemSummaryRepos.AsQueryable().Where(i => i.WishListId == wishlistId).ToList();
            var wishlistItems = repositoryFactory.GetRepository<WishlistItem>()
                .AsQueryable()
                .Where(wi => wi.WishlistId == wishlistId)
                .ToList();
            var wishlist = wishlistRepository.GetAsync(wishlistId).Result;
            var learningObjectiveIds = items.Select(wi => wi.LearningObjectiveId).Distinct().ToList();
            var learningObjectives = learningObjectiveRepository.AsQueryable()
                .Where(l => learningObjectiveIds.Contains(l.Id)).ToList();
            var titles = learningObjectives.Select(l => l.Code).OrderBy(s => s).ToList();
            var los = wishlistItems.Select(wi => wi.LearningObjectiveCode).OrderBy(s => s).ToList();
            foreach (var learningLearningObjectiveId in learningObjectiveIds)
            {
                var currectLo = learningObjectiveRepository.GetAsync(learningLearningObjectiveId).Result;
                if (currectLo.WishlistId == null)
                {
                    currectLo.WishlistId = wishlistId;
                    learningObjectiveRepository.UpdateAsync(currectLo).Wait();
                }
                else if (currectLo.WishlistId != wishlistId)
                {
                    var oldId = currectLo.Id;
                    var newLearningObjective = currectLo.Clone();
                    newLearningObjective.Id = new Guid();
                    newLearningObjective.WishlistId = wishlistId;
                    newLearningObjective.Deadline = new DateTime(2017, 9, 1);
                    newLearningObjective = learningObjectiveRepository.AddAsync(newLearningObjective).Result;
                    foreach (var itemToModify in wishlistItems.Where(wi => wi.LearningObjectiveId == oldId).ToList())
                    {
                        itemToModify.LearningObjectiveId = newLearningObjective.Id;
                        repositoryFactory.GetRepository<WishlistItem>().UpdateAsync(itemToModify);
                    }
                    foreach (var itemToModify in items.Where(wi => wi.LearningObjectiveId == oldId).ToList())
                    {
                        itemToModify.LearningObjectiveId = newLearningObjective.Id;
                        itemRepos.UpdateAsync(itemToModify);
                    }
                    foreach (var itemToModify in itemsummaries.Where(wi => wi.LearningObjectiveId == oldId).ToList())
                    {
                        itemToModify.LearningObjectiveId = newLearningObjective.Id;
                        itemSummaryRepos.UpdateAsync(itemToModify);
                    }
                    foreach (var itemToModify in wishlist.WishListItems.Where(wi => wi.LearningObjectiveId == oldId)
                        .ToList())
                    {
                        itemToModify.LearningObjectiveId = newLearningObjective.Id;
                    }
                    wishlistRepository.UpdateAsync(wishlist).Wait();
                }
                else
                {
                    var wishListItems = wishlistItems.Where(wi => wi.LearningObjectiveId == currectLo.Id);
                    if (wishListItems.Count() > 1)
                    {
                        // something should be fixed.
                    }
                    var wishListItem = wishListItems.FirstOrDefault();
                    if (wishListItem != null && currectLo.Total != wishListItem.NumberOfItems)
                    {
                        currectLo.Total = wishListItem.NumberOfItems;
                        learningObjectiveRepository.UpdateAsync(currectLo).Wait();
                    }
                    else if (wishListItem == null)
                    {
                        // weird..
                    }

                }

            }
            foreach (var wi in wishlistItems)
            {
                var currectLo = learningObjectiveRepository.GetAsync(wi.LearningObjectiveId).Result;
                if (currectLo.WishlistId != wishlistId)
                {
                    throw new Exception();
                }
                if (currectLo.Total != wi.NumberOfItems)
                {
                    currectLo.Total = wi.NumberOfItems;
                    learningObjectiveRepository.UpdateAsync(currectLo);
                }
            }

        }

    }
}
