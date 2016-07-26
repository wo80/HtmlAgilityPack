
namespace HtmlAgilityPack.Fizzler.Tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class LRUCacheTest
    {
        static Func<int, IEnumerable<int>> Eval(string repeat)
        {
            return i => Enumerable.Repeat(i, int.Parse(repeat));
        }

        [Test]
        public void GetCapacity()
        {
            var cache = new LRUCache<string, Func<int, IEnumerable<int>>>(Eval, 3);

            Assert.AreEqual(3, cache.Capacity);
        }

        [Test]
        public void SetCapacity()
        {
            var cache = new LRUCache<string, Func<int, IEnumerable<int>>>(Eval, 6);

            Assert.AreEqual(6, cache.Capacity);

            cache.Capacity = 5;

            Assert.AreEqual(5, cache.Capacity);

            var value1 = cache.GetValue("1");
            var value2 = cache.GetValue("2");
            var value3 = cache.GetValue("3");
            var value4 = cache.GetValue("4");
            var value5 = cache.GetValue("5");

            cache.Capacity = 2;

            Assert.AreEqual(2, cache.Capacity);
        }

        [Test]
        public void GetValue()
        {
            var cache = new LRUCache<string, Func<int, IEnumerable<int>>>(Eval, 3);

            var value1 = cache.GetValue("1");
            var value2 = cache.GetValue("2");
            var value3 = cache.GetValue("3");

            Assert.AreEqual(1, value1(0).Count());
            Assert.AreEqual(2, value2(0).Count());
            Assert.AreEqual(3, value3(0).Count());
        }

        [Test]
        public void GetValueExceedCapacity()
        {
            var cache = new LRUCache<string, Func<int, IEnumerable<int>>>(Eval, 3);

            var value1 = cache.GetValue("1");
            var value2 = cache.GetValue("2");
            var value3 = cache.GetValue("3");
            var value4 = cache.GetValue("4");
            var value5 = cache.GetValue("5");
            var value6 = cache.GetValue("6");
            var value7 = cache.GetValue("7");

            Assert.AreEqual(1, value1(0).Count());
            Assert.AreEqual(2, value2(0).Count());
            Assert.AreEqual(3, value3(0).Count());
            Assert.AreEqual(4, value4(0).Count());
            Assert.AreEqual(5, value5(0).Count());
            Assert.AreEqual(6, value6(0).Count());
            Assert.AreEqual(7, value7(0).Count());
        }
    }
}
