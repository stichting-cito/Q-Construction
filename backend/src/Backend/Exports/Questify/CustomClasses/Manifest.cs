using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QConstruction.Backend.Exports.QTI;
using DocumentFormat.OpenXml.Math;

namespace Citolab.QConstruction.Backend.Exports.Questify.CustomClasses
{
    public class Manifest: XDocument
    {
        private readonly string _path;
        private readonly ViewRender _renderer;
        public Manifest(string path, ViewRender renderer) : 
            base(path.EndsWith("manifest.xml") ?
            QuestifyHelper.FilePathToXDocument(path) :
            QuestifyHelper.FilePathToXDocument(Path.Combine(path, "manifest.xml")))
        {
            _path = path;
            _renderer = renderer;
        }

        public void AddItem(QuestifyItemBase item)
        {
            var itemText = _renderer.Render("ManifestItem", item);
            var itemXml = XElement.Parse(itemText);
            var lastNode = this.Document?.DescendantNodes()
                .OfType<XElement>()
                .LastOrDefault(e => e.Name == "ResourceEntry");
            lastNode?.AddAfterSelf(itemXml);
        }

        public void AddImage(string fileName, string name)
        {
            var imageText = _renderer.Render("ManifestImage", new ImageDetails(fileName, name));
            var imageXml = XElement.Parse(imageText);
            var lastNode = this.Document?.DescendantNodes()
                .OfType<XElement>()
                .LastOrDefault(e => e.Name == "ResourceEntry");
            lastNode?.AddAfterSelf(imageXml);
        }

        public void Save()
        {
            File.WriteAllText(Path.Combine(_path, "manifest.xml"), this.ToString());
        }
    }
}
