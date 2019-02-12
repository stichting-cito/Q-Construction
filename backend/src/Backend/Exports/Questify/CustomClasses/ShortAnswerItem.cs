using System;
using System.Collections.Generic;
using System.Linq;
using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses;

namespace Citolab.QConstruction.Backend.Exports.Questify.CustomClasses
{
    public class ShortAnswerItem : QuestifyItemBase
    {
        public ShortAnswerItem(Item item) : base(item)
        {
            var key = new string(item.Key.Where(char.IsDigit).ToArray());
            Keys = new List<string> { key };
            KeyValues = Keys.FirstOrDefault();
            GeneralText = item.Key.StripHtml().Replace(" ", string.Empty).Any(c => !char.IsDigit(c)) ? item.Key : string.Empty;
        }
        
        public override string Template => "BO.TextEntry.SC";
        public override string RazorView => "ShortAnswer";
    }
}
