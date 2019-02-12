using System;
using Citolab.Repository.Helpers;
namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Learning objective.
    /// </summary>
    public class LearningObjective : Citolab.Repository.Model
    {
        /// <summary>
        ///     Reference to the domain
        /// </summary>
        public Guid DomainId { get; set; }

        /// <summary>
        ///     Title of the learning objective.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Code of the learning objective
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Name of the domain
        /// </summary>
        public string DomainTitle { get; set; }

        /// <summary>
        ///     Name of the domain
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        ///     Reference to wishlist with shortest deadline
        /// </summary>
        [EnsureIndex]
        public Guid? WishlistId { get; set; }

        /// <summary>
        ///     Total number of items that should be created for this learningobjective
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        ///     Items to create
        /// </summary>
        public int Todo { get; set; }

        /// <summary>
        ///     Count of items already created for this learningobjective
        /// </summary>
        public int CreatedCount { get; set; }
    }
}