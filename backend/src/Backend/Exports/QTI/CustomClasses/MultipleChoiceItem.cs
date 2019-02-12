using System.Collections.Generic;
using System.Linq;
using Citolab.QConstruction.Model;

namespace Citolab.QConstruction.Backend.Exports.QTI.CustomClasses
{
    /// <summary>
    ///     Multiple choice item
    /// </summary>
    public class MultipleChoiceItem : QtiItemBase
    {
        /// <summary>
        ///     TextEntry
        /// </summary>
        /// <param name="item"></param>
        public MultipleChoiceItem(Item item) : base(item)
        {
            //Keys = new List<char>();
            Alternatives = item.SimpleChoices.Select(c => c.Title).ToList();
            Keys = item.SimpleChoices
                    .Where(c => c.IsKey)
                    .Select(c => (char) (item.SimpleChoices.ToList().IndexOf(c) + 65))
                    .ToList();
        }

        /// <summary>
        ///     Keys
        /// </summary>
        public List<char> Keys { get; }

        /// <summary>
        ///     Alternatives
        /// </summary>
        public List<string> Alternatives { get; }
    }
}