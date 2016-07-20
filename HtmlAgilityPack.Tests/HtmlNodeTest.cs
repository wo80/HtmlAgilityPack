using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace HtmlAgilityPack.Tests
{
	[TestFixture]
	public class HtmlNodeTest
	{
		[Test(Description="Attributes should maintain their original character casing if OptionOutputOriginalCase is true")]
		public void EnsureAttributeOriginalCaseIsPreserved()
		{
			var html = "<html><body><div AttributeIsThis=\"val\"></div></body></html>";
			var doc = HtmlDocument.Parse(html);
            doc.Options.OutputOriginalCase = true;
			var div = doc.DocumentNode.Descendants("div").FirstOrDefault();
			var writer = new StringWriter();
            (new HtmlWriter(doc)).WriteAttributes(writer, div, false);
			var result = writer.GetStringBuilder().ToString();
			Assert.AreEqual(" AttributeIsThis=\"val\"", result);
		}
	}
}
