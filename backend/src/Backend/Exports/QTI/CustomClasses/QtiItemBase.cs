using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses;

namespace Citolab.QConstruction.Backend.Exports.QTI.CustomClasses
{
    public class QtiItemBase
    {
        public QtiItemBase(Item item)
        {
            var itemId = item.UniqueCode ?? item.Id.ToString();
            Id = $"ITM-{itemId}";
            Title = item.BodyText.TruncateAndPlainText(64);
            BodyText = item.BodyText;
        }


        /// <summary>
        ///     Id
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Title
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     BodyText
        /// </summary>
        public string BodyText { get; }
    }
}