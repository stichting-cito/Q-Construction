using System.Collections.Generic;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Statistics per wishlist for the manager dashboard
    /// </summary>
    public class StatsPerWishlist : Citolab.Repository.Model
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public StatsPerWishlist()
        {
            StatsPerDomain = new List<DomainStats>();
            StatsPerUser = new List<UserStats>();
            ItemsAcceptedPerDayCumulative = new List<DateCount>();
            ItemDeadlinesWithCounts = new List<DateCount>();
            ScreeningItemStats = new List<ScreeningItemStats>();
            ScreeningRoundsOfWastedItems = new List<WastedItem>();
        }

        /// <summary>
        ///     Number of items that are accepted.
        /// </summary>
        public int ItemsAcceptedCount { get; set; }

        /// <summary>
        ///     Number of items that are in the review cycle (ready-for-review, in-review or needs-work).
        /// </summary>
        public int ItemsInReviewCount { get; set; }

        /// <summary>
        ///     Number of items that are rejected (while in the review cycle)
        /// </summary>
        public int ItemsRejectedCount { get; set; }

        /// <summary>
        ///     Number of items that are left to be made.
        /// </summary>
        public int ItemsTodoCount { get; set; }

        /// <summary>
        ///     Total number of items to be made.
        /// </summary>
        public int ItemTargetCount { get; set; }

        /// <summary>
        ///     Total number of review iterations.
        /// </summary>
        public int IterationCount { get; set; }

        /// <summary>
        ///     The percentage of accepted items (rounded to the nearest integer).
        /// </summary>
        public int PercentageAccepted { get; set; } //=> (ItemsAcceptedCount / ItemTargetCount) * 100;

        /// <summary>
        ///     The percentage of rejected items (rounded to the nearest integer).
        /// </summary>
        public int PercentageMortality { get; set; }

        // => ItemsRejectedCount / (ItemsAcceptedCount + ItemsRejectedCount);

        /// <summary>
        ///     Mean number of iterations overall.
        /// </summary>
        public decimal MeanReviewIterations { get; set; }

        // => (decimal)IterationCount / (ItemsAcceptedCount + ItemsRejectedCount);

        /// <summary>
        ///     List of stats per domain (percentage accepted, mean no of iterations).
        /// </summary>
        public List<DomainStats> StatsPerDomain { get; set; }

        /// <summary>
        ///     Statistics per user.
        /// </summary>
        public List<UserStats> StatsPerUser { get; set; }

        /// <summary>
        ///     Item deadlines with counts.
        /// </summary>
        public List<DateCount> ItemDeadlinesWithCounts { get; set; }

        /// <summary>
        /// </summary>
        public List<DateCount> ItemsAcceptedPerDayCumulative { get; set; }


        /// <summary>
        ///     Screening items usage counts.
        /// </summary>
        public List<ScreeningItemStats> ScreeningItemStats { get; set; }

        /// <summary>
        ///     Number of screening of items that are deleted by the constructor of rejected
        /// </summary>
        public List<WastedItem> ScreeningRoundsOfWastedItems { get; set; }
    }
}