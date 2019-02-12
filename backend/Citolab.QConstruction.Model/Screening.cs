using Citolab.Repository.Helpers;
using System;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Review for an item.
    /// </summary>
    public class Screening : Citolab.Repository.Model
    {
        /// <summary>
        ///     Identifier of the item this review was entered for.
        /// </summary>
        [EnsureIndex]
        public Guid ItemId { get; set; }

        /// <summary>
        ///     Indicates if its a draft or final
        /// </summary>
        public ScreeningStatus Status { get; set; }

        /// <summary>
        ///     Version number of the item this review is based on.
        /// </summary>
        public int BasedOnVersion { get; set; }

        /// <summary>
        ///     Next status the item should get.
        /// </summary>
        public ItemStatus? NextItemStatus { get; set; }

        /// <summary>
        ///     List of feedbacks in this review.
        /// </summary>
        public Feedback[] FeedbackList { get; set; }
    }
}