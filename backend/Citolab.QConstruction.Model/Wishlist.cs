using System;
using System.Collections.Generic;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Construction wishlist
    /// </summary>
    public class Wishlist : Citolab.Repository.Model
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Wishlist()
        {
            WishListItems = new List<WishlistItem>();
        }

        /// <summary>
        ///     Title of the wishlist
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Organisation name
        /// </summary>
        public string OrganisationName { get; set; }

        /// <summary>
        ///     Screeningslist Id
        /// </summary>
        public Guid ScreeningsListId { get; set; }

        /// <summary>
        ///     Items to be constructed per learning objective.
        /// </summary>
        public List<WishlistItem> WishListItems { get; set; }

        /// <summary>
        /// List of itemtypes that are excluded
        /// </summary>
        public List<ItemType> DisabledItemTypes { get; set; }
    }
}