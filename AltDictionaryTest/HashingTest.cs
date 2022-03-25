using static Alt.Hashing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AltTest
{
    public class TestPerson
    {
        internal TestPerson(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }
        public int Age { get; set; }
    }

    [TestClass]
    public class HashingTest
    {
        private TestPerson p1 = new("Nikola", 28);
        private TestPerson p2 = new("Nebojsa", 60);
        private TestPerson p3 = new("Marija", 22);

        [TestInitialize()]
        public void Initialize()
        {
        }

        [TestMethod]
        public void HashingObjectsTest()
        {
            Assert.IsTrue(Hash(p1, 11) < 11);
            Assert.IsTrue(Hash(p2, 11) < 11);
            Assert.IsTrue(Hash(p3, 11) < 11);
            Assert.IsTrue(Hash(p1, 11) >= 0);
            Assert.IsTrue(Hash(p2, 11) >= 0);
            Assert.IsTrue(Hash(p3, 11) >= 0);
            Assert.IsTrue(Hash(p1, 0) == 0);
        }

        [TestMethod]
        public void HashingIntegers()
        {
            Assert.IsTrue(Hash(11, 11) == 0);
            Assert.IsTrue(Hash(0, 11) == 0);
            Assert.IsTrue(Hash(16, 11) == 5);
            Assert.IsTrue(Hash(100, 11) == 1);
            Assert.IsTrue(Hash(17, 0) == 0);
        }

        [TestMethod]
        public void IsPrimeTest()
        {
            Assert.IsTrue(IsPrime(0) == false);
            Assert.IsTrue(IsPrime(1) == false);
            Assert.IsTrue(IsPrime(-1) == false);
            Assert.IsTrue(IsPrime(2) == true);
            Assert.IsTrue(IsPrime(11) == true);
            Assert.IsTrue(IsPrime(69) == false);
        }

        [TestMethod]
        public void GetBucketCountTest()
        {
            Assert.IsTrue(GetBucketCount(-5) == -5);
            Assert.IsTrue(GetBucketCount(17) == 37);
            Assert.IsTrue(GetBucketCount(5) == 11);
            Assert.IsTrue(GetBucketCount(1) == 3);
            Assert.IsTrue(GetBucketCount(0) == 3);
            Assert.IsTrue(GetBucketCount(40) == 83);
        }
    }
}