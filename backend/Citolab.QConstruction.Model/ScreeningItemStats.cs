using System;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Screening item statistics.
    /// </summary>
    public class ScreeningItemStats
    {
        /// <summary>
        ///     Id of the screening item.
        /// </summary>
        public Guid ScreeningItemId { get; set; }

        /// <summary>
        ///     Name of the screening item.
        /// </summary>
        public string ScreeningItemName { get; set; }

        /// <summary>
        ///     Number of times this screening item is used.
        /// </summary>
        public int UseCount { get; set; }
    }
}