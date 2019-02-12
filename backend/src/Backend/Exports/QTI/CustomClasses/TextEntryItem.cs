using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses;

namespace Citolab.QConstruction.Backend.Exports.QTI.CustomClasses
{
    /// <summary>
    ///     TextEntry item
    /// </summary>
    public class TextEntryItem : QtiItemBase
    {
        /// <summary>
        ///     TextEntry
        /// </summary>
        /// <param name="item"></param>
        public TextEntryItem(Item item) : base(item)
        {
            Key = item.Key.StripHtml();
        }

        /// <summary>
        ///     Key
        /// </summary>
        public string Key { get; }
    }
}