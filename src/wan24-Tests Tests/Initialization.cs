namespace wan24.Tests
{
    [TestClass]
    public class Initialization
    {
        [AssemblyInitialize]
        public static void Init(TestContext tc) => TestsInitialization.Init(tc);
    }
}
