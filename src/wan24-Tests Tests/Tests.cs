using wan24.Tests;

namespace wan24_Tests_Tests
{
    [TestClass]
    public class Tests : TestBase
    {
        private bool Initialized = false;

        [TestMethod]
        public void General_Tests()
        {
            Assert.IsNotNull(TestsInitialization.LoggerFactory);
            Assert.IsNotNull(TestsInitialization.Options);

            Assert.IsTrue(Initialized);
            Assert.IsNotNull(TestContext);
            Assert.IsNotNull(Logger);
        }

        [TestInitialize]
        public override void InitTests()
        {
            base.InitTests();
            Initialized = true;
        }
    }
}
