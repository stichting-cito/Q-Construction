using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace Citolab.QConstruction.Backend.Exports.Questify
{
    public class QuestifyHelper
    {
        public static XDocument FilePathToXDocument(string filePath)
        {
            var xDoc = new XDocument();
            string s = File.ReadAllText(filePath, Encoding.UTF8);
            try
            {
                xDoc = XDocument.Parse(Convert.ToString(s));
            }
            catch
            {
                //not good? try utf16
                s = File.ReadAllText(filePath, Encoding.Unicode);
                xDoc = XDocument.Parse(Utf16ToUtf8(s));
            }
            return xDoc;
        }

        public static string Utf16ToUtf8(string utf16String)
        {
            // Get UTF16 bytes and convert UTF16 bytes to UTF8 bytes
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // Return UTF8 bytes as ANSI string
            return Encoding.GetEncoding(0).GetString(utf8Bytes);
        }
    }
}
