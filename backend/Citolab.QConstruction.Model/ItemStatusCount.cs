using System;
using Citolab.Repository.Helpers;
using Newtonsoft.Json;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Count of items per status
    /// </summary>
    public class ItemStatusCount : Citolab.Repository.Model
    {
        /// <summary>
        ///     Empty constructor
        /// </summary>
        public ItemStatusCount()
        {
        }

        /// <summary>
        ///     Constructor to init with values
        /// </summary>
        /// <param name="wishListId"></param>
        /// <param name="state"></param>
        /// <param name="count"></param>
        public ItemStatusCount(Guid wishListId, ItemStatus state, int count)
        {
            WishlistId = wishListId;
            ItemStatus = state;
            Count = count;
        }

        /// <summary>
        ///     Status of the
        /// </summary>
        [JsonProperty("itemStatus")]
        public ItemStatus ItemStatus { get; private set; }

        /// <summary>
        ///     Number of items
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     Wishlist id
        /// </summary>
        [EnsureIndex]
        public Guid WishlistId { get; set; }
    }
}