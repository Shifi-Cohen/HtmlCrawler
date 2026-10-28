using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class Selector
    {
        public string? TagName { get; set; }
        public string? Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector? Parent { get; set; }
        public Selector? Child { get; set; }


        public static Selector FromString(string query)
        {
            var parts = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Selector root = new Selector();
            Selector current = root;

            for (int i = 0; i < parts.Length; i++)
            {
                var tokens = new Regex("(?=[#.])").Split(parts[i])
                                                  .Where(s => s.Length > 0);

                foreach (var token in tokens)
                {
                    if (token.StartsWith("#"))
                        current.Id = token.Substring(1);
                    else if (token.StartsWith("."))
                        current.Classes.Add(token.Substring(1));
                    else if (HtmlHelper.Instance.AllTags.Contains(token))
                        current.TagName = token;
                }

                if (i < parts.Length - 1)
                {
                    var child = new Selector { Parent = current };
                    current.Child = child;
                    current = child;
                }
            }

            return root;
        }
    }
}
