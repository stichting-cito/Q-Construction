using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;

namespace Citolab.QConstruction.Backend.Exports.Questify.CustomClasses
{
    public class GraphicalGapMatchItem : HotspotItem
    {
        public GraphicalGapMatchItem(Item item) : base(item)
        {

        }
        public override string Template => "BO.GraphicGapMatch.SC";
        public override string RazorView => "GraphicalGapMatch";
    }
}
