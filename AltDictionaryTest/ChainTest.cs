using Alt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AltTest
{
    [TestClass]
    public class ChainTest
    {
        KeyValuePair<int, int> item1 = new KeyValuePair<int, int>(0, 0);
        KeyValuePair<int, int> item2 = new KeyValuePair<int, int>(1, 1);
        KeyValuePair<int, int> item3 = new KeyValuePair<int, int>(2, 2);

        [TestMethod]
        public void ReadOnlyTest()
        {
            Chain<int, int> chain = new();
            Assert.IsFalse(chain.IsReadOnly);
        }

        [TestMethod]
        public void CountTest()
        {
            Chain<int, int> chain = new();
            Assert.IsTrue(chain.Count == 0);
            chain.Add(item1);
            Assert.IsTrue(chain.Count == 1);
            chain.Add(item2);
            Assert.IsTrue(chain.Count == 2);
            chain.Add(item3);
            Assert.IsTrue(chain.Count == 3);
        }

        [TestMethod]
        public void AddTest()
        {
            Chain<int, int> chain = new();
            chain.Add(item1);
            Assert.IsTrue(chain.Count == 1);
            chain.Add(item2);
            Assert.IsTrue(chain.Count == 2);
            Assert.IsTrue(chain.Contains(item1));
            Assert.IsTrue(chain.Contains(item2));
        }

        [TestMethod]
        public void RemoveTest()
        {
            var chain = new Chain<int, int>() { item1, item2, item3 };
            Assert.IsTrue(chain.Remove(item1));
            Assert.IsFalse(chain.Remove(item1));
            Assert.IsTrue(chain.Contains(item2));
            Assert.IsTrue(chain.Contains(item3));
            Assert.IsTrue(chain.Remove(item2));
            Assert.IsFalse(chain.Remove(item2));
            Assert.IsTrue(chain.Contains(item3));
            Assert.IsTrue(chain.Remove(item3));
            Assert.IsFalse(chain.Contains(item1));
            Assert.IsFalse(chain.Contains(item2));
            Assert.IsFalse(chain.Contains(item3));
            Assert.IsTrue(chain.Count == 0);
            Assert.IsFalse(chain.Remove(item3));
        }

        [TestMethod]
        public void ContainsTest()
        {
            var chain = new Chain<int, int>() { item1, item2 };
            Assert.IsTrue(chain.Contains(item1));
            Assert.IsTrue(chain.Contains(item2));
            Assert.IsFalse(chain.Contains(item3));
            chain.Remove(item1);
            Assert.IsFalse(chain.Contains(item1));
            Assert.IsTrue(chain.Contains(item2));
            Assert.IsFalse(chain.Contains(item3));
            chain.Remove(item2);
            Assert.IsFalse(chain.Contains(item1));
            Assert.IsFalse(chain.Contains(item2));
            Assert.IsFalse(chain.Contains(item3));
            chain.Add(item3);
            Assert.IsFalse(chain.Contains(item1));
            Assert.IsFalse(chain.Contains(item2));
            Assert.IsTrue(chain.Contains(item3));
        }

        [TestMethod]
        public void ClearTest()
        {
            var chain = new Chain<int, int>() { item1, item2 };
            chain.Clear();
            Assert.IsFalse(chain.Contains(item1));
            Assert.IsFalse(chain.Contains(item2));
            Assert.IsFalse(chain.Contains(item3));
            Assert.IsTrue(chain.Count == 0);
        }
    }
}
