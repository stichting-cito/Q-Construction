namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     A user.
    /// </summary>
    public class User : Citolab.Repository.Model
    {
        /// <summary>
        ///     Type of the user.
        /// </summary>
        public UserType? UserType { get; set; }

        /// <summary>
        ///     Name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///  Name of the organization
        /// </summary>
        public string Organisation { get; set; }
        /// <summary>
        ///     Password, used to add Auth0 user
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Url to profile picture
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        ///     Token from other system
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        ///     Allowed Wishlists.
        /// </summary>
        public KeyValue[] AllowedWishlists { get; set; }

        /// <summary>
        ///     Id of the last selected wishlist
        /// </summary>
        public KeyValue SelectedWishlist { get; set; }
    }
}