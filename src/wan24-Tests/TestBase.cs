using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using wan24.Core;

namespace wan24.Tests
{
    /// <summary>
    /// Base class for tests
    /// </summary>
    public abstract class TestBase()
    {
        /// <summary>
        /// Test context
        /// </summary>
        public TestContext TestContext { get; set; } = null!;

        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger { get; protected set; } = null!;

        /// <summary>
        /// Initialize tests
        /// </summary>
        [TestInitialize]
        public virtual void InitTests()
        {
            Logger = Logging.Logger ?? TestsInitialization.LoggerFactory.CreateLogger("Tests");
            TestsInitialization.Options.OnBeforeTestsInitialization(this);
            Logging.WriteInfo($"Running test {TestContext.FullyQualifiedTestClassName}.{TestContext.ManagedMethod}");
            TestsInitialization.Options.OnAfterTestsInitialization(this);
        }
    }
}
