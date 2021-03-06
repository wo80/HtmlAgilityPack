namespace HtmlAgilityPack.Fizzler.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using HtmlAgilityPack.Fizzler;
    using HtmlAgilityPack;
    
    public abstract class SelectorBaseTest
	{
	    protected SelectorBaseTest()
		{
            string html;
			var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "HtmlAgilityPack.Fizzler.Tests.SelectorTest.html";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new Exception(string.Format("Resource, named {0}, not found.", resourceName));
                using(var reader = new StreamReader(stream))
                    html = reader.ReadToEnd();
            }
            Document = HtmlDocument.Parse(html);
        }

	    protected HtmlDocument Document { get; private set; }

	    protected IEnumerable<HtmlNode> Select(string selectorChain)
        {
            return Document.DocumentNode.QuerySelectorAll(selectorChain);
        }

        protected IList<HtmlNode> SelectList(string selectorChain)
        {
            return new ReadOnlyCollection<HtmlNode>(Select(selectorChain).ToArray());
        }
    }
}