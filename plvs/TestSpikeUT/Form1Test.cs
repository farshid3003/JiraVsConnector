//using Microsoft.VisualStudio.TestTools.UnitTesting;
using MbUnit.Framework;
using TestSpike;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = MbUnit.Framework.Assert;
using TestContext = Gallio.Framework.TestContext;

namespace TestSpikeUT {
    
//    [TestClass]
    public class Form1Test {
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

//        [TestMethod]
        [Test]
        public void doStuffTest() {
            Form1 target = new Form1();
            Assert.IsTrue(target.doStuff());
        }
    }
}
