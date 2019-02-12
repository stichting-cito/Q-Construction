using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Citolab.QConstruction.Logic.HelperClasses
{
    /// <summary>
    ///     Extenstion Methods
    /// </summary>
    public static class Extensions
    {
        private static readonly Regex StripHtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        ///     Get latest version of version property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="versionedProperty"></param>
        /// <returns></returns>
        public static T Latest<T>(this VersionedProperty<T> versionedProperty)
        {
            var latest = versionedProperty?.LastOrDefault();
            return latest != null ? latest.Value : default(T);
        }

        /// <summary>
        ///     Strips the html
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHtml(this string input)
        {
            return string.IsNullOrEmpty(input) ? string.Empty : StripHtmlRegex.Replace(input, string.Empty).Trim();
        }

        /// <summary>
        ///     Replace illegal filename chars
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceIllegalFilenameChars(this string input)
        {
            return System.IO.Path.GetInvalidFileNameChars().Aggregate(input, (current, c) => current.Replace(c, '_'));
        }

        /// <summary>
        ///     Trim the end
        /// </summary>
        /// <param name="input"></param>
        /// <param name="suffixToRemove"></param>
        /// <returns></returns>
        public static string TrimEnd(this string input, string suffixToRemove)
        {
            if (input != null && suffixToRemove != null
                && input.EndsWith(suffixToRemove))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            return input;
        }

        /// <summary>
        ///     Truncate string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        ///     Trucate and remove html tags
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string TruncateAndPlainText(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            value = value.StripHtml();
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        ///     Get Name
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string Name(this ClaimsPrincipal user)
        {
            var userMetadata = user?.Claims.FirstOrDefault(c => c.Type == "user_metadata")?.Value;
            if (string.IsNullOrEmpty(userMetadata)) return string.Empty;
            var userdataObject = JObject.Parse(userMetadata);
            return userdataObject.SelectToken("name")?.ToString();
        }

        /// <summary>
        ///     Prevents warning when not waiting for result while running a task.
        /// </summary>
        /// <param name="task"></param>
        public static void Forget(this Task task)
        {
        }

        /// <summary>
        ///     Increments the version if there is a version property
        /// </summary>
        /// <param name="obj"></param>
        public static void IncrementVersion(this Repository.Model obj)
        {
            if (obj is Item item)
            {
                if (item.ItemStatus != ItemStatus.Draft) item.Version++;
            }
            else
            {
#if NET46
                var versionProperty = obj.GetType().GetProperty("Version");
                if (versionProperty == null) return;
                int version;
                int.TryParse(versionProperty.GetValue(obj, null).ToString(), out version);
                version++;
                versionProperty.SetValue(obj, version, null);
#endif
            }
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toClone"></param>
        /// <returns></returns>
        public static T Clone<T>(this T toClone) where T : class
        {
            var tmp = JsonConvert.SerializeObject(toClone);
            return JsonConvert.DeserializeObject<T>(tmp);
        }

        public static bool IsImageOrVideoContentType(this string contentType)
        {
            return contentType.StartsWith("image");
        }


        public static string Auth0Id(this ClaimsPrincipal user) =>
    user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        /// <summary>
        ///     Get Role
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string Role(this ClaimsPrincipal user)
        {
            var appMetadata = user?.Claims.FirstOrDefault(c => c.Type == "app_metadata")?.Value;

            if (string.IsNullOrEmpty(appMetadata)) return string.Empty;
            var appdataObject = JObject.Parse(appMetadata);
            return appdataObject.SelectToken("roles").FirstOrDefault()?.ToString();
        }

        public static Dictionary<string, byte[]> GetImages(this HtmlDocument doc, string itemcode,
            IRepository<Attachment> attachmentRepository)
        {
            return GetImages(doc, itemcode, attachmentRepository, (imageNode, name, image) => imageNode.Attributes["src"].Value = $"../img/{name}");
        }


        public static Dictionary<string, byte[]> GetImages(this HtmlDocument doc, string itemcode, IRepository<Attachment> attachmentRepository, Action<HtmlNode, string, byte[]> fixMethod)
        {
            var images = new Dictionary<string, byte[]>();
            var nodeCollection = doc?.DocumentNode.SelectNodes("//*[name()=\"img\"]");
            if (nodeCollection == null) return null;
            foreach (var imageNode in nodeCollection)
            {
                if (imageNode.Attributes?["alt"] == null) imageNode.Attributes?.Append(doc.CreateAttribute("alt"));
                var containsBase64Image = imageNode.Attributes?["src"]?.Value.Contains("base64");
                var image = new byte[0];
                var name = string.Empty;
                if (containsBase64Image.HasValue && containsBase64Image.Value)
                {
                    var base64Splitted = imageNode.Attributes["src"].Value.Split(Convert.ToChar(","));
                    if (base64Splitted.Length <= 1) continue;
                    var filename = imageNode.Attributes?["alt"]?.Value;
                    name = !string.IsNullOrEmpty(filename)
                        ? $"IMG_{itemcode.ReplaceIllegalFilenameChars()}-{Path.GetFileNameWithoutExtension(filename).ReplaceIllegalFilenameChars()}.{GetExtension(base64Splitted[0])}"
                        : $"IMG_{itemcode.ReplaceIllegalFilenameChars()}-{images.Count + 1}.{GetExtension(base64Splitted[0])}";
                    image = Convert.FromBase64String(base64Splitted[1]);
                }
                else if (imageNode.Attributes?["src"] != null)
                {
                    var src = imageNode.Attributes?["src"]?.Value;
                    var start = src.IndexOf("attachment/") + "attachment/".Length;
                    var end = src.IndexOf("/thumbnail");
                    if (Guid.TryParse(src.Substring(start, (end - start)), out var attachmentId))
                    {
                        var imageAttachment = attachmentRepository.GetAsync(attachmentId).Result;
                        name = $"{itemcode.ReplaceIllegalFilenameChars()}-{imageAttachment.FileName}";
                        image = imageAttachment.Bytes;
                    }

                }
                var alreadyAdded = false;
                if (images.Values.Any(i => i.Length == image.Length))
                {
                    var imageAlreadyAdded = images.First(i => i.Value != null && i.Value.SequenceEqual(image));
                    if (imageAlreadyAdded.Value != null)
                    {
                        name = imageAlreadyAdded.Key;

                        alreadyAdded = true;
                    }
                }
                fixMethod(imageNode, name, image);
                if (alreadyAdded) continue;
                images.Add(name, image);
            }
            return images;
        }



        private static string GetExtension(string src)
        {
            src = src.Replace(" ", "").ToLower();
            if (src.Contains("image/png")) return "png";
            if (src.Contains("image/jpg")) return "jpg";
            if (src.Contains("image/jpeg")) return "jpg";
            if (src.Contains("image/gif")) return "gif";
            throw new Exception("Image not supported");
        }
    }
}