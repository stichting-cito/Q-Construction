using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Citolab.QConstruction.Backend.Exports.Interfaces;
using Citolab.QConstruction.Backend.Exports.QTI.CustomClasses;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.QConstruction.Model;
using Citolab.Repository;

namespace Citolab.QConstruction.Backend.Exports.QTI
{
    /// <summary>
    ///     Factory to get the right converter based on a item
    /// </summary>
    public class QtiConverter
    {
        private readonly IConvertItem _choiceInterationConverter;
        private readonly IConvertManifest _manifestConverter;
        private readonly IConvertTest _testConverter;
        private readonly string _testname;
        private readonly IRepositoryFactory _repository;
        private readonly IConvertItem _textEntryInteractionConvert;

        /// <summary>
        ///     Constructor, pass the wwwRoot directory
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="testname"></param>
        /// <param name="renderer"></param>
        /// <param name="version"></param>
        /// <param name="repository"></param>
        public QtiConverter(string rootPath, string testname, ViewRender renderer, string version, IRepositoryFactory repository)
        {
            _choiceInterationConverter = new ChoiceInteraction(renderer, version);
            _textEntryInteractionConvert = new TextEntryInteraction(renderer, version);
            _testConverter = new Test(renderer, version);
            _manifestConverter = new Manifest(renderer, version);
            _testname = testname;
            _repository = repository;
            var tempPath = Path.Combine(rootPath, "temp");
            var dir = new DirectoryInfo(tempPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            var packagePath = Path.Combine(tempPath, Path.GetRandomFileName());
            var itemPath = Path.Combine(packagePath, "items");
            var baseDir = new DirectoryInfo(packagePath);
            var itemDir = new DirectoryInfo(itemPath);
            if (!baseDir.Exists) baseDir.Create();
            if (!itemDir.Exists) itemDir.Create();
        }

        /// <summary>
        ///     Convert items and create package
        /// </summary>
        /// <param name="items"></param>
        /// <param name="tempPath"></param>
        /// <returns></returns>
        public string ConvertItems(IEnumerable<Item> items, string tempPath)
        {
            var baseTempPath = tempPath;
            tempPath = Path.Combine(baseTempPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            var dir = new DirectoryInfo(tempPath);
            if (!dir.Exists) dir.Create();
            var itemsList = items.Select(i => i.Clone()).ToList();
            var htmlToQtiConverter = new HtmlToQtiHelper(_repository, tempPath);
            var itemsDir = new DirectoryInfo(Path.Combine(tempPath, "items"));
            if (!itemsDir.Exists) itemsDir.Create();
            foreach (var item in itemsList)
            {
                var code = item.UniqueCode ?? item.Id.ToString();
                item.BodyText = htmlToQtiConverter.ConvertXhtmlToQti(code, item.BodyText);
                item.BodyText = htmlToQtiConverter.ConvertStylesToCss(item.BodyText);
                if (item.Key != null)
                {
                    item.Key = htmlToQtiConverter.ConvertXhtmlToQti(code, item.Key);
                    item.Key = htmlToQtiConverter.ConvertStylesToCss( item.Key);
                }
                for (var i = 0; i < (item.SimpleChoices?.Length ?? 0); i++)
                {
                    if (item.SimpleChoices == null) continue;
                    item.SimpleChoices[i].Title = htmlToQtiConverter.ConvertXhtmlToQti(code,
                        item.SimpleChoices[i].Title);
                    item.SimpleChoices[i].Title = htmlToQtiConverter.ConvertStylesToCss(item.SimpleChoices[i].Title);
                }
                var itemQti = item.SimpleChoices?.Length > 0
                    ? _choiceInterationConverter.Convert(item)
                    : _textEntryInteractionConvert.Convert(item);
                var fileName = Path.Combine(itemsDir.ToString(), $"ITM-{item.Id}.xml");
                File.WriteAllText(fileName, itemQti);
            }
            var test = new QtiTest { Id = $"TST-{Guid.NewGuid()}", Items = itemsList, Title = _testname };
            var testQti = _testConverter.Convert(test);
            var fileNameTest = Path.Combine(itemsDir.ToString(), $"{test.Id}.xml");
            File.WriteAllText(fileNameTest, testQti);
            var css = htmlToQtiConverter.Css;
            var images = htmlToQtiConverter.Images;

            var imageList = images?.Select(i => $"img/{i.Key}").ToList() ?? new List<string>();
            var cssList = !string.IsNullOrEmpty(css)
                ? new List<string> { "css/generated_styles.css" }
                : new List<string>();
            var manifestItems = new ManifestItems
            {
                TestId = test.Id,
                Items = itemsList,
                Dependencies = htmlToQtiConverter.Dependencies,
                Media = imageList,
                Css = cssList
            };
            var manifestQti = _manifestConverter.Convert(manifestItems);
            var fileNameManifest = Path.Combine(tempPath, "imsmanifest.xml");
            File.WriteAllText(fileNameManifest, manifestQti);
            var cssDir = new DirectoryInfo(Path.Combine(tempPath, "css"));
            if (!cssDir.Exists) cssDir.Create();
            var fileNameCss = Path.Combine(cssDir.ToString(), "generated_styles.css");
            if (!string.IsNullOrEmpty(css)) File.WriteAllText(fileNameCss, css);
            var fileNameZip = Path.Combine(baseTempPath, $"qti-package-{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.zip");
            ZipFile.CreateFromDirectory(tempPath, fileNameZip,
                        CompressionLevel.Optimal,
                        false);
            return fileNameZip;
        }
    }
}