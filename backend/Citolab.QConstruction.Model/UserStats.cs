using System;
using System.Collections.Generic;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     User statistics.
    /// </summary>
    public class UserStats
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public UserStats()
        {
            ScreeningItemStatsList = new List<ScreeningItemStats>();
        }

        /// <summary>
        ///     Id of the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     Name of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Url to picture
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        ///     Type of the user.
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        ///     Number of items accepted (for constructors and test experts).
        /// </summary>
        public int ItemsAcceptedCount { get; set; }

        /// <summary>
        ///     Number of items rejected.
        /// </summary>
        public int ItemsRejectedCount { get; set; }

        /// <summary>
        ///     Number of review iterations
        /// </summary>
        public int IterationCount { get; set; }

        /// <summary>
        ///     List of screening items this user has received/given.
        /// </summary>
        public List<ScreeningItemStats> ScreeningItemStatsList { get; set; }

        #region Constructor stats

        /// <summary>
        ///     Mean number of review iterations (for constructors).
        /// </summary>
        public decimal MeanReviewIterations { get; set; }

        // => (decimal)IterationCount / (ItemsAcceptedCount + ItemsRejectedCount);

        /// <summary>
        ///     Percentage of rejected items (for constructors).
        /// </summary>
        public decimal PercentageRejected { get; set; }

        // => (decimal)ItemsRejectedCount / (ItemsAcceptedCount + ItemsRejectedCount);

        #endregion
    }
}