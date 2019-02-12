using System;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     User type.
    /// </summary>
    [Flags]
    public enum UserType
    {
        /// <summary>
        ///     Constructeur
        /// </summary>
        Constructeur = 1,

        /// <summary>
        ///     Toetsdeskundige
        /// </summary>
        Toetsdeskundige = 2,

        /// <summary>
        /// Manager that can view stats of a wishlist but cannot create new wishlist and has no rights for user management
        /// </summary>
        RestrictedManager = 3,
        /// <summary>
        ///     Manager
        /// </summary>
        Manager = 4,
        /// <summary>
        ///  Admin: Same as manager except that an admin has no restriction on wishlists.
        /// </summary>
        Admin = 5
    }
}