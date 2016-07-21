using NUnit.Framework;
using System.IO;
using System.Net;

namespace HtmlAgilityPack.Tests
{
	[TestFixture]
    public class StackOverflowTest
    {
        [Test]
        public void StackOverflow()
        {
            var url = "http://rewarding.me/active-tel-domains/index.php/index.php?rescan=amour.tel&w=A&url=&by=us&limits=0";
            var request = WebRequest.Create(url);
            var htmlDocument = HtmlDocument.Load((request.GetResponse()).GetResponseStream());
            Stream memoryStream = new MemoryStream();
            htmlDocument.Save(memoryStream);
        }
    }
}
