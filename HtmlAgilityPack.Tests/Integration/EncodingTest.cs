using NUnit.Framework;
using System;
using System.IO;

namespace HtmlAgilityPack.Tests.Integration
{
	[TestFixture]
	public class EncodingTest
    {
        private string _contentDirectory;

        [OneTimeSetUp]
        public void Setup()
        {
            _contentDirectory = Path.Combine(Directory.GetCurrentDirectory(),
                "HtmlAgilityPack.Tests\\Resources");
        }
    }
}
