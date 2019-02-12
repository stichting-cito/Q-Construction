using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.Repository;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;

namespace Citolab.QConstruction.Backend.Exports.MsWord
{
    public class WordConverter
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly string _folderName;
        public WordConverter(string rootPath, IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _folderName = Path.Combine(rootPath, "Temp");
            var dirInfo = new DirectoryInfo(_folderName);
            if (!dirInfo.Exists) dirInfo.Create();
        }

        public string Process(IEnumerable<Item> items)
        {
            var fullPath = System.IO.Path.Combine(_folderName, CleanFileName($"Q-construction-export-{DateTime.Now}.docx"));
            // Create a Wordprocessing document. 
            using (WordprocessingDocument package = WordprocessingDocument.Create(fullPath, WordprocessingDocumentType.Document))
            {
                // Add a new main document part. 
                package.AddMainDocumentPart();
                // Create the Document DOM. 
                package.MainDocumentPart.Document = new Document(new Body());
                var mainPart = package.MainDocumentPart;
                foreach (var item in items)
                {
                    var choices = item.SimpleChoices.ToList();
                    var isMc = choices.Any(c => !string.IsNullOrWhiteSpace(c.Title));
                    var key = isMc ? (choices.FirstOrDefault(i => i.IsKey)?.Title) : item.Key;
                    ProcessHtml($"Unieke code: {item.UniqueCode}</p>", mainPart);
                    ProcessHtml($"Vraag: {item.BodyText}", mainPart);
                    ProcessHtml($"Sleutel: {key}", mainPart);
                    if (isMc)
                    {
                        ProcessHtml($"Alternatieven:", mainPart);
                        item.SimpleChoices.ToList().Where(c => !string.IsNullOrWhiteSpace(c.Title)).ToList()
                                                   .ForEach(s => ProcessHtml(s.Title, mainPart));
                    }
                    ProcessHtml($"Beschrijving: {item.Description}", mainPart);
                    ProcessHtml($"Notities: {item.Notes}", mainPart);
                    ProcessHtml(" ===================================================================================", mainPart);
                }

                // Save changes to the main document part. 
                package.MainDocumentPart.Document.Save();
                return fullPath;
            }
        }

        private void ProcessHtml(string html, MainDocumentPart mainPart)
        {
            html = html.Replace("</p><p>", "</p> <p>");
            if (string.IsNullOrWhiteSpace(html)) return;
            var body = mainPart.Document.Body;
            html = $"<dummytag>{html}</dummytag>";
            var doc = new HtmlDocument { OptionOutputAsXml = true };
            doc.LoadHtml(html);

            body.AddText(doc.DocumentNode.FirstChild.InnerText);
            var images = doc.GetImages(string.Empty,_repositoryFactory.GetRepository<Attachment>())?.ToList();
            images?.ForEach(i => mainPart.InsertAPicture(i.Value, i.Key));
        }



        private static string CleanFileName(string fileName)
        {
            return System.IO.Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
