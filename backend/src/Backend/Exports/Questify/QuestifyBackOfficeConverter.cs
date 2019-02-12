using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citolab.QConstruction.Backend.Exports.QTI;
using Citolab.QConstruction.Backend.Exports.QTI.CustomClasses;
using Citolab.QConstruction.Backend.Exports.Questify.CustomClasses;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using HtmlAgilityPack;
using Manifest = Citolab.QConstruction.Backend.Exports.Questify.CustomClasses.Manifest;
using MultipleChoiceItem = Citolab.QConstruction.Backend.Exports.Questify.CustomClasses.MultipleChoiceItem;

namespace Citolab.QConstruction.Backend.Exports.Questify
{
    public class QuestifyBackOfficeConverter
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly Dictionary<string, string> _images = new Dictionary<string, string>();
        private readonly string _tempPath;
        private readonly string _exportPath;

        private readonly Manifest _manifest;
        private readonly ManifestMetaData _manifestMetaData;
        private readonly ViewRender _renderer;

        public QuestifyBackOfficeConverter(string rootPath, IRepositoryFactory repositoryFactory, ViewRender renderer)
        {
            _exportPath = Path.Combine(Path.Combine(rootPath, "Temp"), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            _tempPath = Path.Combine(_exportPath, "buildExport");
            var dir = new DirectoryInfo(_tempPath);
            if (!dir.Exists) dir.Create();
            _repositoryFactory = repositoryFactory;

            var assembly = typeof(QuestifyBackOfficeConverter).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(@"Citolab.QConstruction.Backend.Exports.Views.QB_BackOffice.base.zip");
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                zip.ExtractToDirectory(_tempPath);
            }
            _renderer = renderer;
            _manifestMetaData = new ManifestMetaData(_tempPath, renderer);
            _manifest = new Manifest(_tempPath, renderer);
        }

        public string ConvertItems(IEnumerable<Item> items)
        {
            var qconstructionItem = items.ToList();
            var itemsList = qconstructionItem.Select<Item, QuestifyItemBase>(i =>
            {
                switch (i.ItemType)
                {
                    case ItemType.GraphicGapMatch:
                        return new GraphicalGapMatchItem(i);
                    case ItemType.Hotspot:
                        return new HotspotItem(i);
                    case ItemType.MC:
                        return new MultipleChoiceItem(i);
                    case ItemType.SA:
                        return new ShortAnswerItem(i);
                }
                return null;
            }).Where(i => i != null).ToList();
            foreach (var item in itemsList)
            {
                item.BodyText = ProcessHtml(item.BodyText, item);
                var mcItem = item as MultipleChoiceItem;
                for (var i = 0; i < (mcItem?.Alternatives?.Count ?? 0); i++)
                {
                    if (mcItem?.Alternatives == null) continue;
                    mcItem.Alternatives[i] = ProcessHtml(mcItem.Alternatives[i], item);
                }
                var hsItem = item as HotspotItem;
                for (var i = 0; i < (hsItem?.Keys?.Count ?? 0); i++)
                {
                    if (hsItem?.Keys == null) continue;
                    hsItem.Keys[i] = ProcessHtml(hsItem.Keys[i], item);
                    hsItem.Image = item.Images?.Count != 0 ? item.Images.FirstOrDefault().Key : "noimage.jpg";
                }
                var itemString = _renderer.Render(item.RazorView, item);
                _manifestMetaData.AddItem(item);
                _manifest.AddItem(item);
                File.WriteAllText(Path.Combine(_tempPath, item.Id), itemString);
            }
            _manifestMetaData.Save();
            _manifest.Save();
            var fileNameZip = Path.Combine(_exportPath, $"qc-to-qb-{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.export");
            ZipFile.CreateFromDirectory(_tempPath, fileNameZip,
                CompressionLevel.Optimal,
                false);
            return fileNameZip;
        }

        private string ProcessHtml(string html, QuestifyItemBase item)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            html = $"<dummytag>{html}</dummytag>";
            var doc = new HtmlDocument { OptionOutputAsXml = true, OptionOutputOriginalCase = true, OptionOutputUpperCase = true};
            doc.LoadHtml(html);
            var images = doc.GetImages(item.Id,_repositoryFactory.GetRepository<Attachment>(),
                (imageNode, name, image) =>
                {
                    var filename = Path.Combine(_tempPath, name);
                    if (!File.Exists(filename)) File.WriteAllBytes(filename, image);
                    var inlineImage = _renderer.Render("InlineImage", new ImageDetails(filename, name));
                    var inlineNode = HtmlNode.CreateNode(inlineImage);
                    Debug.Assert(imageNode?.ParentNode != null);
                    imageNode?.ParentNode?.ReplaceChild(inlineNode, imageNode);

                })?.ToList();
            images?.ForEach(i =>
            {
                var filename = Path.Combine(_tempPath, i.Key);
                if (!_images.ContainsKey(i.Key))
                {
                    if (!File.Exists(filename)) File.WriteAllBytes(filename, i.Value);
                    _images.Add(i.Key, filename);
                }
                if (!item.Images.ContainsKey(i.Key))
                {
                    _manifestMetaData.AddImage(filename, i.Key);
                    _manifest.AddImage(filename, i.Key);
                    item.Images.Add(i.Key, filename);
                }
            });
            return doc.DocumentNode.FirstChild.InnerHtml
                .Replace("inlineelement", "InlineElement")
                .Replace("layouttemplatesourcename", "layoutTemplateSourceName")
                .Replace("parameterset", "parameterSet")
                .Replace("<p>","<p id=\"c1-id-11\" xmlns=\"http://www.w3.org/1999/xhtml\">")
                .Replace("&nbsp;", " ");
        }
    }
}
