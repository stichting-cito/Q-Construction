using System;
using Citolab.Repository.Helpers;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     A question item.
    /// </summary>
    public class ItemSummary : Citolab.Repository.Model
    {
        /// <summary>
        ///     Title of the item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     BodyTekst
        /// </summary>
        public string BodyText { get; set; }

        /// <summary>
        ///     Type of the item.
        /// </summary>
        public ItemType ItemType { get; set; }

        /// <summary>
        ///     Reference to the wishlist.
        /// </summary>
        [EnsureIndex]
        public Guid WishListId { get; set; }

        /// <summary>
        ///     Reference to the wishList item to which this item belongs, this matches a learning objective.
        /// </summary>
        public Guid LearningObjectiveId { get; set; }

        /// <summary>
        ///     Status of the item.
        /// </summary>
        public ItemStatus ItemStatus { get; set; }

        /// <summary>
        ///     Version number.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        ///     Key for the item.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Deadline defined in wishlistItem
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        ///     Title of the learning objective
        /// </summary>
        public string LearningObjectiveTitle { get; set; }

        /// <summary>
        ///     Learning objective code
        /// </summary>
        public string LearningObjectiveCode { get; set; }

        /// <summary>
        ///     Title of the domain
        /// </summary>
        public string DomainTitle { get; set; }

        /// <summary>
        ///     Screeners
        /// </summary>
        public string Screeners { get; set; }

        /// <summary>
        ///     ScreeningsCount
        /// </summary>
        public int ScreeningCount { get; set; }

        /// <summary>
        ///     Accepted by
        /// </summary>
        public string AcceptedBy { get; set; }

        /// <summary>
        ///     Name of the creator
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Latest screening
        /// </summary>
        public Guid? LatestScreeningId { get; set; }

        /// <summary>
        ///     Author of the latest screening.
        /// </summary>
        public Guid? LatestScreeningAuthorId { get; set; }

        public string UniqueCode { get; set; }
    }
}