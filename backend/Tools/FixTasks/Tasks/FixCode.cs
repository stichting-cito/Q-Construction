using Citolab.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Citolab.QConstruction.Model;

namespace FixTasks.Tasks
{
    public class FixCode: IFix
    {

        public void DoFix(IRepositoryFactory repository, Guid wishlistId)
        {
            var itemRepos = repository.GetRepository<Item>();
            var items = itemRepos.AsQueryable().ToList().OrderBy(i => i.Created).ToList();
            items.ForEach(item => item.UniqueCode = string.Empty);
            var changedItems = items.ToList();
            items.ForEach(item =>
            {
                var lastSeq = changedItems
                    .Where(i => i.LearningObjectiveId == item.LearningObjectiveId &&
                                i.UniqueCode.IndexOf(item.LearningObjectiveCode, StringComparison.Ordinal) != -1)
                    .Select(i => i.UniqueCode)
                    .ToList()
                    .Select(i =>
                    {
                        var seq = i.Split('_').LastOrDefault();
                        return int.TryParse(seq, out var nr) ? nr : 1;
                    })
                    .OrderByDescending(c => c)
                    .FirstOrDefault();
                lastSeq++;
                var itemToChange = changedItems.FirstOrDefault(c => c.Id == item.Id);
                itemToChange.UniqueCode = $"{item.LearningObjectiveCode}_{lastSeq}";
            });
            changedItems.ForEach(item => itemRepos.UpdateAsync(item));
        }

    }
}
