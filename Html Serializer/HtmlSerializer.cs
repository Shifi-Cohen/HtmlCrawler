using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class HtmlSerializer
    {
        public async Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }
        public List<string> SplitToLines(string html)
        {
            var cleanHtml = new Regex("[\\r\\n\\t]+").Replace(html, "");
            cleanHtml = new Regex("\\s{2,}").Replace(cleanHtml, " ");

            var lines = new Regex("<(.*?)>")
                .Split(cleanHtml)
                .Where(s => s.Length > 0)
                .ToList();

            return lines;
        }
        public HtmlElement Serialize(string html)
        {
            var lines = SplitToLines(html);
            var helper = HtmlHelper.Instance;

            var root = new HtmlElement { Name = "root" };
            var currentElement = root;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("!"))
                    continue;

                var firstWord = trimmedLine.Split(' ')[0];

                if (firstWord == "/html")
                    break;

                if (firstWord.StartsWith("/"))
                {
                    if (currentElement.Parent != null)
                        currentElement = currentElement.Parent;
                    continue;
                }

                var tagName = firstWord.TrimEnd('/').ToLowerInvariant();

                if (helper.AllTags.Contains(tagName))
                {
                    var newElement = new HtmlElement
                    {
                        Name = tagName,
                        Parent = currentElement
                    };
                    currentElement.Children.Add(newElement);

                    var attributeString = trimmedLine.Substring(firstWord.Length);
                    var matches = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(attributeString);

                    foreach (Match match in matches)
                    {
                        var attrName = match.Groups[1].Value;
                        var attrValue = match.Groups[2].Value;

                        newElement.Attributes.Add(match.Value);

                        if (attrName == "id")
                            newElement.Id = attrValue;
                        else if (attrName == "class")
                            newElement.Classes = attrValue
                                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .ToList();
                    }

                    bool isSelfClosing = trimmedLine.EndsWith("/")
                                         || helper.SelfClosingTags.Contains(tagName);

                    if (!isSelfClosing)
                        currentElement = newElement;
                }
                else
                {
                    currentElement.InnerHtml = (currentElement.InnerHtml ?? "") + trimmedLine;
                }
            }

            return root.Children.FirstOrDefault() ?? root;
        }
    }
}
