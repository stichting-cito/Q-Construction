namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     A screener can give feedback on a screenings item.
    /// </summary>
    public class ScreeningItem : Citolab.Repository.Model
    {
        /// <summary>
        ///     Used to categorize the screening items in the frontend
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///     Display value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Screeningsitem is relevant for this itemtype. If empty then All
        /// </summary>
        public ItemType? ItemType { get; set; }
    }
}