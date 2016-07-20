using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace HtmlAgilityPack.Tests.Integration
{
	[TestFixture]
	public class EncodingTest
    {
        [Test]
        public void TestUtf8_1()
        {
            var doc = TestHelper.Load("encoding-utf8-test-1.htm");

            Assert.AreEqual(doc.DeclaredEncoding.WebName, "utf-8");
        }

        [Test]
        public void TestUtf8_2()
        {
            var doc = TestHelper.Load("encoding-utf8-test-2.htm");

            Assert.AreEqual(doc.DeclaredEncoding.WebName, "utf-8");
        }

        [Test]
        public void TestUtf8_3()
        {
            var doc = TestHelper.Load("encoding-utf8-test-3.htm");

            Assert.IsNull(doc.DeclaredEncoding);
        }

        [Test]
        public void TestIso8859_1()
        {
            var doc = TestHelper.Load("encoding-iso-8859-test-1.htm");

            Assert.AreEqual(doc.DeclaredEncoding.WebName, "iso-8859-1");
        }

        [Test]
        public void TestIso8859_2()
        {
            var doc = TestHelper.Load("encoding-iso-8859-test-2.htm");

            Assert.AreEqual(doc.DeclaredEncoding.WebName, "iso-8859-1");
        }

        [Test]
        public void TestIso8859_3()
        {
            var doc = TestHelper.Load("encoding-iso-8859-test-3.htm");

            Assert.IsNull(doc.DeclaredEncoding);
        }
    }
}
