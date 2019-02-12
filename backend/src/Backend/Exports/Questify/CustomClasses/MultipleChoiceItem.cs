using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using DocumentFormat.OpenXml.Office2010.Word;

namespace Citolab.QConstruction.Backend.Exports.Questify.CustomClasses
{
    public class MultipleChoiceItem : QuestifyItemBase
    {
        public MultipleChoiceItem(Item item) : base(item)
        {
            AlternativeCount = item.SimpleChoices.Count();
            Alternatives = item.SimpleChoices.Select(s => s.Title).ToList();
            RawScore = item.SimpleChoices.Any(i => i.IsKey) ? 1 : 0;
            Keys = item.SimpleChoices.Where(i => i.IsKey)
                .Select(i => ((char) (item.SimpleChoices.ToList().IndexOf(i) + 65)).ToString())
                .ToList();
            KeyValues = string.Join("|", Keys.ToArray());
        }
        public List<string> Alternatives { get; set; }
        public override string Template => "BO.Choice.SC";
        public override string RazorView => "Choice";
    }
}
