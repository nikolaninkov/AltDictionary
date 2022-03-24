using Alt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AltTest
{
    [TestClass]
    public class AltDictionaryTest
    {
        private TestPerson p1 = new("Nikola", 28);
        private TestPerson p2 = new("Nebojsa", 60);
        private TestPerson p3 = new("Marija", 22);
        private AltDictionary<TestPerson, int> dict = new AltDictionary<TestPerson, int>();

        [TestMethod]
        public void BasicDictionaryTest()
        {
            dict.Add(p1, 20);
            dict.Add(p2, 30);
            dict.Add(p3, 34);
            Assert.IsTrue(dict.ContainsKey(p1));
            Assert.IsTrue(dict.ContainsKey(p2));
            Assert.IsTrue(dict.ContainsKey(p3));
            Assert.IsTrue(dict[p1] == 20);
            Assert.IsTrue(dict[p2] == 30);
            Assert.IsTrue(dict[p3] == 34);
        }
    }
}
