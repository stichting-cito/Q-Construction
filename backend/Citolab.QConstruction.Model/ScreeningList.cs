namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Contains a list of screeningItems
    /// </summary>
    public class ScreeningList : Citolab.Repository.Model
    {
        /// <summary>
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Screening items
        /// </summary>
        public ScreeningItem[] Items { get; set; }
    }
}