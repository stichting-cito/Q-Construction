using System;
using System.Collections.Generic;
namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Domain statistics.
    /// </summary>
    public class DomainStats
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public DomainStats()
        {
            ScreeningItemStatsList = new List<ScreeningItemStats>();
        }

        /// <summary>
        ///     Id of the domain
        /// </summary>
        public Guid DomainId { get; set; }

        /// <summary>
        ///     Name of the domain
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     Total number of items to be done.
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        ///     Number of accepted items.
        /// </summary>
        public int ItemsAcceptedCount { get; set; }

        /// <summary>
        ///     Number of rejected items.
        /// </summary>
        public int ItemsRejectedCount { get; set; }

        /// <summary>
        ///     Total number of iterations.
        /// </summary>
        public int IterationCount { get; set; }

        /// <summary>
        ///     Percentage of accepted items (rounded to the nearest integer)
        /// </summary>
        public int PercentageAccepted { get; set; } // => (ItemsAcceptedCount / TotalItemCount) * 100;

        /// <summary>
        ///     Mean number of review iterations.
        /// </summary>
        public decimal MeanReviewIterations { get; set; }

        // => (decimal)IterationCount / (ItemsAcceptedCount + ItemsRejectedCount);

        /// <summary>
        ///     List of screening items this domain has received/given.
        /// </summary>
        public List<ScreeningItemStats> ScreeningItemStatsList { get; set; }
    }
}