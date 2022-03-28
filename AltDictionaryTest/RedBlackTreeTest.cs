using Alt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AltTest
{
    [TestClass]
    public class RedBlackTreeTest
    {
        private readonly TestPerson p1 = new("Nikola", 28);
        private readonly TestPerson p2 = new("Nebojsa", 60);
        private readonly TestPerson p3 = new("Marija", 22);
        private readonly TestPerson a3 = new("Marija", 23);
        private readonly TestPerson b3 = new("Marijaa", 22);
        private RedBlackTree<TestPerson, int> tree = new(); 

        [TestInitialize]
        public void Init()
        {
            tree = new RedBlackTree<TestPerson, int>()
            {
                { p1, 1 }, { p2, 2 }, { p3, 3 }
            };
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsTrue(tree.Contains(p1));
            Assert.IsTrue(tree.Contains(p2));
            Assert.IsTrue(tree.Contains(p3));
            Assert.IsFalse(tree.Contains(a3));
            Assert.IsFalse(tree.Contains(b3));
        }

        [TestMethod]
        public void CountTest()
        {
            Assert.IsTrue(tree.Count == 3);
            tree.Add(a3, 1);
            Assert.IsTrue(tree.Count == 4);
            tree.Remove(p1);
            tree.Remove(p2);
            Assert.IsTrue(tree.Count == 2);
        }

        [TestMethod]
        public void IsReadOnlyTest()
        {
            Assert.IsFalse(tree.IsReadOnly);
        }

        [TestMethod]
        public void AddTest()
        {
            tree.Add(a3, 1);
            tree.Add(b3, 1);
            Assert.IsTrue(tree.Count == 5);
            Assert.IsTrue(tree.Contains(a3));
            Assert.IsTrue(tree.Contains(b3));
        }

        [TestMethod]
        public void RemoveTest()
        {
            Assert.IsTrue(tree.Remove(p1));
            Assert.IsFalse(tree.Contains(p1));
            Assert.IsFalse(tree.Remove(p1));
            Assert.IsTrue(tree.Remove(p2));
            Assert.IsTrue(tree.Remove(p3));
            Assert.IsFalse(tree.Contains(p2));
            Assert.IsFalse(tree.Contains(p3));
            Assert.IsTrue(tree.Count == 0);
        }

        [TestMethod]
        public void ClearTest()
        {
            tree.Clear();
            Assert.IsTrue(tree.Count == 0);
            Assert.IsFalse(tree.Contains(p1));
            Assert.IsFalse(tree.Contains(p2));
            Assert.IsFalse(tree.Contains(p3));
        }
    }
}
