using System;
using System.IO;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Item
    /// </summary>
    public class Item : Citolab.Repository.Model
    {
        /// <summary>
        ///     Title of the item
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The item type.
        /// </summary>
        public ItemType? ItemType { get; set; }

        /// <summary>
        ///     Identifier of the learning objective to which this item belongs.
        /// </summary>
        public Guid LearningObjectiveId { get; set; }

        /// <summary>
        ///     Reference to the wishlist where this item belongs to.
        /// </summary>
        public Guid WishListId { get; set; }

        /// <summary>
        ///     Status of the item.
        /// </summary>
        public ItemStatus ItemStatus { get; set; }

        /// <summary>
        ///     Version number.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        ///     Body text of the item.
        /// </summary>
        public string BodyText { get; set; }

        /// <summary>
        ///     Options for this item, in case of item type MC.
        /// </summary>
        public SimpleChoice[] SimpleChoices { get; set; }

        /// <summary>
        ///     Key for the item.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Description of the interactions in the item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Notes and construction instructions for the item, in case
        ///     it's a hotspot or graphic gap match.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        ///     Title of the domain
        /// </summary>
        public string DomainTitle { get; set; }

        /// <summary>
        ///     Title of the wishlist
        /// </summary>
        public string WishListTitle { get; set; }

        /// <summary>
        ///     Title of the learningObjective
        /// </summary>
        public string LearningObjectiveTitle { get; set; }

        /// <summary>
        ///     Learningobjective code
        /// </summary>
        public string LearningObjectiveCode { get; set; }

        /// <summary>
        ///     Latest screening of the item
        /// </summary>
        public Guid? LatestScreeningId { get; set; }

        /// <summary>
        ///     Author of the latest screening.
        /// </summary>
        public Guid? LatestScreeningAuthorId { get; set; }

        /// <summary>
        ///     Deadline of the item
        /// </summary>
        public DateTime? Deadline { get; set; }

        public string UniqueCode { get; set; }
    }
}