using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class HtmlElement
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string? InnerHtml { get; set; }
        public HtmlElement? Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();


        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            var current = this.Parent;

            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public HashSet<HtmlElement> Query(Selector selector)
        {
            var result = new HashSet<HtmlElement>();
            FindElements(this, selector, result);
            return result;
        }

        private static void FindElements(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            var descendants = element.Descendants().Skip(1);

            foreach (var d in descendants.Where(d => MatchesSelector(d, selector)))
            {
                if (selector.Child == null)
                    result.Add(d);                          
                else
                    FindElements(d, selector.Child, result); 
            }
        }

        private static bool MatchesSelector(HtmlElement element, Selector selector)
        {
            if (selector.TagName != null && element.Name != selector.TagName)
                return false;

            if (selector.Id != null && element.Id != selector.Id)
                return false;

            if (selector.Classes.Any(c => !element.Classes.Contains(c)))
                return false;

            return true;
        }
        public override string ToString()
        {
            var attrs = Attributes.Count > 0 ? " " + string.Join(" ", Attributes) : "";
            return $"<{Name}{attrs}>";
        }
    }
}
