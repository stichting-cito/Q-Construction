using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace FixTasks.Tasks
{
    public class FixItemSummaryCode : IFix
    {
        public void DoFix(IRepositoryFactory repositoryFactory, Guid wishlistId)
        {
            var itemSummaryRepository = repositoryFactory.GetRepository<ItemSummary>();
            var itemRepository = repositoryFactory.GetRepository<Item>();
            var itemSummaries = itemSummaryRepository.AsQueryable().Where(i => string.IsNullOrEmpty(i.UniqueCode)).ToList();
            var items = itemRepository.AsQueryable().ToList();
            itemSummaries.ForEach(item =>
                item.UniqueCode = items.FirstOrDefault(i => i.Id == item.Id)?.UniqueCode);
            itemSummaries.ForEach(item => itemSummaryRepository.UpdateAsync(item));


        }
    }
}
