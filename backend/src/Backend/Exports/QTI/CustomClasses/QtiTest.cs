using System.Collections.Generic;
using Citolab.QConstruction.Model;

namespace Citolab.QConstruction.Backend.Exports.QTI.CustomClasses
{
    public class QtiTest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public IList<Item> Items { get; set; }
    }
}