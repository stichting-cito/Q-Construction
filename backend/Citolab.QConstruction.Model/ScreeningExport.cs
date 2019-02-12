using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citolab.QConstruction.Model { 
    public class ScreeningExport
    {
        public string TextExpert { get; set; }
        public string Author { get; set; }
        public string Feedback { get; set; }
        public string ItemId { get; set; }
        public string ItemTitle { get; set; }
        public string LearningObjective { get; set; }
        public DateTime Created { get; set; }
    }
}
