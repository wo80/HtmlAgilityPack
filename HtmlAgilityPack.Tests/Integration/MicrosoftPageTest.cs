﻿using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace HtmlAgilityPack.Tests.Integration
{
	[TestFixture]
	public class MicrosoftPageTest
    {
        private const string HTML_FILE = "mshome-2003.htm";

        [Test]
        public void TestParse()
        {
            var doc = TestHelper.Load(HTML_FILE);
            Assert.IsTrue(doc.DocumentNode.Descendants().Count() > 0);
        }

        [Test]
        public void TestLimitDepthParse()
        {
            HtmlAgilityPack.HtmlDocument.MaxDepthLevel = 10;
            var doc = TestHelper.Load(HTML_FILE);
            try
            {
                Assert.IsTrue(doc.DocumentNode.Descendants().Count() > 0);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message == HtmlAgilityPack.HtmlNode.DepthLevelExceptionMessage);
            }
            HtmlAgilityPack.HtmlDocument.MaxDepthLevel = int.MaxValue;
        }

        [Test]
        public void TestParseSaveParse()
        {
            string file = TestHelper.GetLocalPath("testsaveparse.html");

            var doc = TestHelper.Load(HTML_FILE);
            var doc1desc =
                doc.DocumentNode.Descendants().Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ToList();
            TestHelper.Save(doc, file);

            var doc2 = TestHelper.Load(file);
            var doc2desc =
                doc2.DocumentNode.Descendants().Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ToList();
            Assert.AreEqual(doc1desc.Count, doc2desc.Count);
            //for(var i=0; i< doc1desc.Count;i++)
            //{
            //    try
            //    {
            //        Assert.AreEqual(doc1desc[i].Name, doc2desc[i].Name);
            //    }catch(Exception e)
            //    {
            //        throw;
            //    }
            //}

            File.Delete(file);
        }

        [Test]
        public void TestRemoveUpdatesPreviousSibling()
        {
            var doc = TestHelper.Load(HTML_FILE);
            var docDesc = doc.DocumentNode.Descendants().ToList();
            var toRemove = docDesc[1200];
            var toRemovePrevSibling = toRemove.PreviousSibling;
            var toRemoveNextSibling = toRemove.NextSibling;
            toRemove.Remove();
            Assert.AreSame(toRemovePrevSibling, toRemoveNextSibling.PreviousSibling);
        }

        [Test]
        public void TestReplaceUpdatesSiblings()
        {
            var doc = TestHelper.Load(HTML_FILE);
            var docDesc = doc.DocumentNode.Descendants().ToList();
            var toReplace = docDesc[1200];
            var toReplacePrevSibling = toReplace.PreviousSibling;
            var toReplaceNextSibling = toReplace.NextSibling;
            var newNode = doc.CreateElement("tr");
            toReplace.ParentNode.ReplaceChild(newNode, toReplace);
            Assert.AreSame(toReplacePrevSibling, newNode.PreviousSibling);
            Assert.AreSame(toReplaceNextSibling, newNode.NextSibling);
        }

        [Test]
        public void TestInsertUpdateSiblings()
        {
            var doc = TestHelper.Load(HTML_FILE);
            var newNode = doc.CreateElement("td");
            var toReplace = doc.DocumentNode.ChildNodes[2];
            var toReplacePrevSibling = toReplace.PreviousSibling;
            var toReplaceNextSibling = toReplace.NextSibling;
            doc.DocumentNode.ChildNodes.Insert(2, newNode);
            Assert.AreSame(newNode.NextSibling, toReplace);
            Assert.AreSame(newNode.PreviousSibling, toReplacePrevSibling);
            Assert.AreSame(toReplaceNextSibling, toReplace.NextSibling);
        }
    }
}
