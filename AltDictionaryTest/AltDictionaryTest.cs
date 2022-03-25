using Alt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AltTest
{
    [TestClass]
    public class AltDictionaryTest
    {
        private TestPerson p1 = new("Nikola", 28);
        private TestPerson p2 = new("Nebojsa", 60);
        private TestPerson p3 = new("Marija", 22);
        private AltDictionary<TestPerson, int> dict = new();
        private AltDictionary<TestPerson, int> emptyDict = new();

        [TestInitialize]
        public void Init()
        {
            dict = new AltDictionary<TestPerson, int>()
            {
                { p1, 20 },
                { p2, 30 },
                { p3, 34 }
            };
            emptyDict = new AltDictionary<TestPerson, int>();
        }

        [TestMethod]
        public void AccessorsTest()
        {
            Assert.IsTrue(dict[p1] == 20);
            Assert.IsTrue(dict[p2] == 30);
            Assert.IsTrue(dict[p3] == 34);
            dict[p1]++;
            dict[p2]--;
            Assert.IsTrue(dict[p1] == 21);
            Assert.IsTrue(dict[p2] == 29);
            var p = new TestPerson("Nema", 1);
            Assert.ThrowsException<KeyNotFoundException>(() => dict[p]);
            Assert.ThrowsException<KeyNotFoundException>(() => dict[p] = 11);
        }

        [TestMethod]
        public void KeysTest()
        {
            Assert.IsTrue(dict.Keys.Count == 3);
            Assert.IsTrue(emptyDict.Keys.Count == 0);
        }

        [TestMethod]
        public void ValuesTest()
        {
            Assert.IsTrue(dict.Values.Count == 3);
            Assert.IsTrue(emptyDict.Values.Count == 0);
        }

        [TestMethod]
        public void CountTest()
        {
            Assert.IsTrue(dict.Count == 3);
            Assert.IsTrue(emptyDict.Count == 0);
        }

        [TestMethod]
        public void IsReadOnlyTest()
        {
            Assert.IsFalse(dict.IsReadOnly);
            Assert.IsFalse(emptyDict.IsReadOnly);
        }

        [TestMethod]
        public void AddKeyValueTest()
        {
            var p = new TestPerson("Neko", 25);
            dict.Add(p, 13);
            Assert.IsTrue(dict[p] == 13);
            emptyDict.Add(p, 13);
            Assert.IsTrue(emptyDict[p] == 13);
            Assert.ThrowsException<ArgumentNullException>(() => emptyDict.Add(null, 13));
            Assert.ThrowsException<ArgumentException>(() => dict.Add(p1, 13));
        }

        [TestMethod]
        public void AddPairTest()
        {
            var p = new KeyValuePair<TestPerson, int>(new TestPerson("Neko", 25), 13);
            dict.Add(p);
            Assert.IsTrue(dict.Contains(p));
            emptyDict.Add(p);
            Assert.IsTrue(dict.Contains(p));
        }

        [TestMethod]
        public void ClearTest()
        {
            dict.Clear();
            Assert.IsTrue(dict.Count == 0);
            Assert.IsFalse(dict.ContainsKey(p1));
            Assert.IsFalse(dict.ContainsKey(p2));
            Assert.IsFalse(dict.ContainsKey(p3));
        }

        [TestMethod]
        public void ContainsTest()
        {
            var t1 = new KeyValuePair<TestPerson, int>(p1, 20);
            var t2 = new KeyValuePair<TestPerson, int>(p1, 21);
            var t3 = new KeyValuePair<TestPerson, int>(p2, 20);
            Assert.IsTrue(dict.Contains(t1));
            Assert.IsFalse(dict.Contains(t2));
            Assert.IsFalse(dict.Contains(t3));
            Assert.IsFalse(emptyDict.Contains(t1));
            Assert.IsFalse(emptyDict.Contains(t2));
            Assert.IsFalse(emptyDict.Contains(t3));
        }

        [TestMethod]
        public void ContainsKeyTest()
        {
            var p = new TestPerson("Neki", 19);
            Assert.IsTrue(dict.ContainsKey(p1));
            Assert.IsTrue(dict.ContainsKey(p2));
            Assert.IsTrue(dict.ContainsKey(p3));
            Assert.IsFalse(dict.ContainsKey(p));
            Assert.IsFalse(emptyDict.ContainsKey(p1));
            Assert.IsFalse(emptyDict.ContainsKey(p2));
            Assert.IsFalse(emptyDict.ContainsKey(p3));
            Assert.IsFalse(emptyDict.ContainsKey(p));
            Assert.ThrowsException<ArgumentNullException>(() => emptyDict.ContainsKey(null));
        }

        [TestMethod]
        public void CopyToTest()
        {
            KeyValuePair<TestPerson, int>[] array = new KeyValuePair<TestPerson, int>[4];
            KeyValuePair<TestPerson, int>[] emptyArray = new KeyValuePair<TestPerson, int>[3];
            dict.CopyTo(array, 1);
            Assert.IsTrue(array[0].Key == null);
            Assert.IsTrue(array[1].Key == p1 || array[1].Key == p2 || array[1].Key == p3);
            Assert.ThrowsException<ArgumentNullException>(() => dict.CopyTo(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => dict.CopyTo(array, -1));
            Assert.ThrowsException<ArgumentException>(() => dict.CopyTo(array, 3));
        }

        [TestMethod]
        public void EnumeratorTest()
        {
            int count = 0;
            foreach (var item in dict)
            {
                Assert.IsTrue(item.Key != null);
                count++;
            }
            Assert.AreEqual(count, dict.Count);
        }

        [TestMethod]
        public void RemoveKeyTest()
        {
            Assert.IsTrue(dict.ContainsKey(p1));
            Assert.IsTrue(dict.Remove(p1));
            Assert.IsFalse(dict.ContainsKey(p1));
            Assert.IsFalse(emptyDict.Remove(p1));
            Assert.ThrowsException<ArgumentNullException>(() => dict.Remove(null));
        }

        [TestMethod]
        public void RemoveTest()
        {
            var t1 = new KeyValuePair<TestPerson, int>(p1, 20);
            Assert.IsTrue(dict.Contains(t1));
            Assert.IsTrue(dict.Remove(t1));
            Assert.IsFalse(dict.Contains(t1));
            Assert.IsFalse(dict.Remove(t1));
        }

        [TestMethod]
        public void TryGetValueTest()
        {
            int v = 0;
            Assert.IsTrue(dict.TryGetValue(p1, out v));
            Assert.AreEqual(v, 20);
            Assert.IsTrue(dict.TryGetValue(p2, out v));
            Assert.AreEqual(v, 30);
            Assert.IsTrue(dict.TryGetValue(p3, out v));
            Assert.AreEqual(v, 34);
            Assert.IsFalse(emptyDict.TryGetValue(p1, out v));
            Assert.ThrowsException<ArgumentNullException>(() => dict.TryGetValue(null, out v));
        }
    }
}
