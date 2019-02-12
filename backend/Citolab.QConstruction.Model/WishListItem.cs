using System;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Item of the wishlist
    /// </summary>
    public class WishlistItem : Citolab.Repository.Model
    {
        private DateTime _deadline;
        /// <summary>
        ///     Reference to wishlist
        /// </summary>
        public Guid WishlistId { get; set; }

        /// <summary>
        ///     DomainId
        /// </summary>
        public Guid DomainId { get; set; }

        /// <summary>
        ///     Learning objective
        /// </summary>
        public Guid LearningObjectiveId { get; set; }

        /// <summary>
        ///     Learning objective code
        /// </summary>
        public string LearningObjectiveCode { get; set; }

        /// <summary>
        ///     LearningObjective Title
        /// </summary>
        public string LearningObjectiveTitle { get; set; }

        /// <summary>
        ///     Number of items
        /// </summary>
        public int NumberOfItems { get; set; }

        /// <summary>
        ///     Number of items that should be created
        /// </summary>
        public int Todo { get; set; }

        /// <summary>
        ///     Numver of items per status
        /// </summary>
        public ItemStatusCount[] ItemStatusCount { get; set; }

        /// <summary>
        ///     Deadline when all the items should be ready
        /// </summary>
        //[BsonDateTimeOptions(DateOnly = true)]
       
        public DateTime Deadline
        {
            get => _deadline.Date;
            set => _deadline = value.Date;
        }
        /// <summary>
        ///     Items
        /// </summary>
        public Guid[] ItemIds { get; set; }
    }
}