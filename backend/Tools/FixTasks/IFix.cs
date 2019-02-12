using System;
using System.Collections.Generic;
using System.Text;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace FixTasks
{
    public interface IFix
    {
        void DoFix(IRepositoryFactory repositoryFactory, Guid wishlistId);
    }
}
