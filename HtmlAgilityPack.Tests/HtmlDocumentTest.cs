using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace HtmlAgilityPack.Tests
{
	[TestFixture]
	public class HtmlDocumentTest
    {
        HtmlDocument _doc1;
        HtmlDocument _doc2;
        HtmlDocument _doc3;

        [OneTimeSetUp]
        public void Setup()
        {
            _doc1 = TestHelper.Load("test-page-1.htm");
            _doc2 = TestHelper.Load("test-page-2.htm");
            _doc3 = TestHelper.Load("test-page-3.htm");
        }

        [Test]
        public void DeclaredEncoding_1()
        {
            Assert.AreEqual("utf-8", _doc1.DeclaredEncoding.WebName);
        }

        [Test]
        public void DeclaredEncoding_2()
        {
            Assert.AreEqual("iso-8859-1", _doc2.DeclaredEncoding.WebName);
        }

        [Test]
        public void DeclaredEncoding_3()
        {
            Assert.IsNull(_doc3.DeclaredEncoding);
        }

        [Test]
        public void GetElementById_1()
        {
            var section = _doc1.GetElementById("main");
            var dummy = _doc1.GetElementById("dummy");

            Assert.AreEqual("section", section.Name);
            Assert.IsNull(dummy);

            Assert.Throws<ArgumentNullException>(() => _doc1.GetElementById(null));
        }

        [Test]
        public void GetElementsByName_1()
        {
            var h1 = _doc1.GetElementsByName("h1");
            var ul = _doc1.GetElementsByName("ul");
            var li = _doc1.GetElementsByName("li");

            Assert.AreEqual(1, h1.Count());
            Assert.AreEqual(1, ul.Count());
            Assert.True(li.Count() > 1);
        }

        [Test]
        public void FormContentsAvailable()
        {
            var form = _doc1.GetElementById("form");

            Assert.IsNotNull(form);

            var inputs = form.Descendants("input").ToList();

            Assert.True(inputs.Count > 1);

            var in1 = inputs[0];
            var in2 = inputs[1];

            Assert.True(in1.HasAttributes);
            Assert.True(in2.HasAttributes);
        }

		[Test]
		public void CreateAttribute()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href");
			Assert.AreEqual("href", a.Name);
		}

		[Test]
		public void CreateAttributeWithEncodedText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href", "http://something.com\"&<>");
			Assert.AreEqual("href", a.Name);
			Assert.AreEqual("http://something.com\"&<>", a.Value);
		}

		[Test]
		public void CreateAttributeWithText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href", "http://something.com");
			Assert.AreEqual("href", a.Name);
			Assert.AreEqual("http://something.com", a.Value);
		}

		//[Test]
		//public void CreateComment()
		//{
		//    var doc = new HtmlDocument();
		//    var a = doc.CreateComment();
		//    Assert.AreEqual(HtmlNode.HtmlNodeTypeNameComment, a.Name);
		//    Assert.AreEqual(a.NodeType, HtmlNodeType.Comment);
		//}

		//[Test]
		//public void CreateCommentWithText()
		//{
		//    var doc = new HtmlDocument();
		//    var a = doc.CreateComment("something");
		//    Assert.AreEqual(HtmlNode.HtmlNodeTypeNameComment, a.Name);
		//    Assert.AreEqual("something", a.InnerText);
		//    Assert.AreEqual(a.NodeType, HtmlNodeType.Comment);
		//}

		[Test]
		public void CreateElement()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateElement("a");
			Assert.AreEqual("a", a.Name);
			Assert.AreEqual(a.NodeType, HtmlNodeType.Element);
		}

		//[Test]
		//public void CreateTextNode()
		//{
		//    var doc = new HtmlDocument();
		//    var a = doc.CreateTextNode();
		//    Assert.AreEqual(HtmlNode.HtmlNodeTypeNameText, a.Name);
		//    Assert.AreEqual(a.NodeType, HtmlNodeType.Text);
		//}

		[Test]
		public void CreateTextNodeWithText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateTextNode("something");
			Assert.AreEqual("something", a.InnerText);
			Assert.AreEqual(a.NodeType, HtmlNodeType.Text);
		}

		[Test]
		public void HtmlEncode()
		{
			var result = HtmlWriter.HtmlEncode("http://something.com\"&<>");
			Assert.AreEqual("http://something.com&quot;&amp;&lt;&gt;", result);
		}
	}
}