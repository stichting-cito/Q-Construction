using System;
using Citolab.Repository.Helpers;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     User status count
    /// </summary>
    public class UserItemStatusCount : ItemStatusCount
    {
        /// <summary>
        ///     Empty constructor
        /// </summary>
        public UserItemStatusCount()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="wishListId"></param>
        /// <param name="state"></param>
        /// <param name="count"></param>
        public UserItemStatusCount(Guid userId, Guid wishListId, ItemStatus state, int count)
            : base(wishListId, state, count)
        {
            UserId = userId;
        }

        /// <summary>
        ///     User Id
        /// </summary>
        [EnsureIndex]
        public Guid UserId { get; set; }
    }
}