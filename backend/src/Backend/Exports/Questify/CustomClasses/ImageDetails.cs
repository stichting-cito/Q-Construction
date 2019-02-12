using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Citolab.QConstruction.Backend.Exports.Questify.CustomClasses
{
    public class ImageDetails
    {
        public ImageDetails(string path, string name)
        {
            using (var image = Image.FromFile(path))
            {
                Width = image.Width;
                Height = image.Height;
            }
            Id = name;
            //Src = name;
        }
        //public string Src { get; set; }
        public string Id { get; set; }
        public int Width { get; }
        public int Height { get; }
    }
}
