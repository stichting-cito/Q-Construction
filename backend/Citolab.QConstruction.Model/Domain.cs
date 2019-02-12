using System;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Domain
    /// </summary>
    public class Domain : Repository.Model
    {
        /// <summary>
        ///     Reference to parent domain
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        ///     Title of the domains
        /// </summary>
        public string Title { get; set; }
    }
}