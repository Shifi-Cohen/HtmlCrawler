using Html_Serializer;

var serializer = new HtmlSerializer();
var html = await serializer.Load("https://example.com");
var dom = serializer.Serialize(html);
var count = 0;

var result = dom.Query(Selector.FromString("head link"));
result.ToList().ForEach(e => Console.WriteLine(e.ToString()));

Console.ReadLine();