using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace GetStatsMailAndClean
{
    public class SlackHelper
    {
        public static void PostMessage(Payload payload, string url)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(
                    url,
                    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")).Result;
            }
        }
    }


#pragma warning disable IDE1006 // Naming Styles
    public class Payload
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        public Attachment[] attachments { get; set; }
    }

    public class Attachment
    {
        public string title { get; set; }

        public string title_link { get; set; }


        public string image_url { get; set; }
        public string thumb_url { get; set; }
        public string fallback { get; set; }
        public Action[] actions { get; set; }
        public string footer_icon { get; set; }
        public string footer { get; set; }

        [JsonProperty("short")]
        public bool IsShort => true;
    }
}
