using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Html_Serializer
{
    internal class HtmlHelper
    {
        private static readonly Lazy<HtmlHelper> _instance =
        new Lazy<HtmlHelper>(() => new HtmlHelper());

        public static HtmlHelper Instance => _instance.Value;
        public string[] AllTags { get; set; }
        public string[] SelfClosingTags { get; set; }

        private HtmlHelper()
        {
            var allTagsContent = File.ReadAllText("HtmlTags.json");
            AllTags = JsonSerializer.Deserialize<string[]>(allTagsContent)!;

            var voidTagsContent = File.ReadAllText("HtmlVoidTags.json");
            SelfClosingTags = JsonSerializer.Deserialize<string[]>(voidTagsContent)!;
        }
    }
}
