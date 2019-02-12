using System;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Wasted items
    /// </summary>
    public class WastedItem
    {
        /// <summary>
        ///     ItemId
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        ///     Removed by author of test expert
        /// </summary>
        public bool RemovedByAuthor { get; set; }

        /// <summary>
        ///     Deleted after x rounds
        /// </summary>
        public int Rounds { get; set; }
    }
}