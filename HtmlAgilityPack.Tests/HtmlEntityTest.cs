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
        public void EntitizeUnknownEntity()
        {
            var expected = "&#64257;";
            var actual = HtmlEntity.Entitize("ﬁ");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeEntitizeKnownEntity()
        {
            var expected = "Ren & Stimpy";
            var actual = HtmlEntity.DeEntitize("Ren &amp; Stimpy");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeEntitizeUnknownEntity()
        {
            var expected = "Some &stupid; entity.";
            var actual = HtmlEntity.DeEntitize(expected);

            Assert.AreEqual(expected, actual);
        }
    }
}
