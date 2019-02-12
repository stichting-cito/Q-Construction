namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Used to get metadata from Auth0 provider
    /// </summary>
    public class UserMetadata
    {
        /// <summary>
        ///     Token from Auth0
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     PictureUrl
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        ///     UserType
        /// </summary>
        public UserType UserType { get; set; }
    }
}