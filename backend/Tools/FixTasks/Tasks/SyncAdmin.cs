using System;
using System.Collections.Generic;
using System.Text;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace FixTasks.Tasks
{
    public class SyncAdmin
    {
        public void DoFix(IRepositoryFactory repositoryFactory, IMemoryCache memoryCache)
        {
            GeneralHelper.SyncAuth0Users(repositoryFactory, memoryCache);
        }
    }
}
