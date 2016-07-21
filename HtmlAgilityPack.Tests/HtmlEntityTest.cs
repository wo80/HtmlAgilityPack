using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgilityPack.Tests
{
    [TestFixture]
    public class HtmlEntityTest
    {
        [Test]
        public void DeEntitizeKnownEntity()
        {
            var expected = "Ren & Stimpy";

            var text = HtmlEntity.DeEntitize("Ren &amp; Stimpy");

            Assert.AreEqual(expected, text);
        }

        [Test]
        public void DeEntitizeUnknownEntity()
        {
            var expected = "Some &stupid; entity.";

            var text = HtmlEntity.DeEntitize(expected);

            Assert.AreEqual(expected, text);
        }
    }
}
