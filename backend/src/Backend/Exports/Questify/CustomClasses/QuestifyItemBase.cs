using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses;

namespace Citolab.QConstruction.Backend.Exports.Questify.CustomClasses
{
    public abstract class QuestifyItemBase
    {

        public QuestifyItemBase(Item item)
        {
            Id = item.UniqueCode ?? item.Id.ToString();
            Title = item.BodyText.TruncateAndPlainText(64);
            Description = $"{item.Description.StripHtml()} {item.Notes?.StripHtml()}";
            BodyText = item.BodyText;
            GeneralText = string.Empty;
            Keys = new List<string> { item.Key };
            KeyValues = item.Key;
            AlternativeCount = 0;
            RawScore = string.IsNullOrWhiteSpace(item.Key) ? 0 : 1;
            Images = new Dictionary<string, string>();
        }
        public string GeneralText { get; set; }
        public string Id { get; }
        public string Title { get; }
        public string Description { get; set; }
        public string BodyText { get; set; }
        public IList<string> Keys { get; set; }
        public string KeyValues { get; set; }
        public int AlternativeCount { get; set; }
        public int RawScore { get; set; }
        public abstract string Template { get; }
        public abstract string RazorView { get; }
        public Dictionary<string, string> Images { get; set; }
    }
}
