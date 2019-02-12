using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;

namespace Citolab.QConstruction.Backend.Exports.Questify.CustomClasses
{
    public class HotspotItem: QuestifyItemBase
    {
        public HotspotItem(Item item) : base(item)
        {
            KeyValues = string.Empty;
            
        }
        public string Image { get; set; }

        public override string Template => "BO.Hotspot.SC";
        public override string RazorView => "Hotspot";
    }
}
