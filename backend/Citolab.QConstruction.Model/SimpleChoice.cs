namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Option for MC/MR-type items.
    /// </summary>
    public class SimpleChoice
    {
        /// <summary>
        ///     Title text of the option.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Indicates the option is a key.
        /// </summary>
        public bool IsKey { get; set; }
    }
}