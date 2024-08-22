using Microsoft.Extensions.Logging;

namespace wan24.Tests
{
    /// <summary>
    /// Attribute for setting test options
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TestsOptionsAttribute() : Attribute()
    {
        /// <summary>
        /// Logfile
        /// </summary>
        public string LogFile { get; set; } = "tests.log";

        /// <summary>
        /// Log level
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// If to create a stack info for disposables
        /// </summary>
        public bool CreateDisposableStackInfo { get; set; }

        /// <summary>
        /// Run before initialization
        /// </summary>
        public virtual void OnBeforeInitialization() { }

        /// <summary>
        /// Run after initialization
        /// </summary>
        public virtual void OnAfterInitialization() { }

        /// <summary>
        /// Run before tests initialization
        /// </summary>
        /// <param name="tests">Initializing tests</param>
        public virtual void OnBeforeTestsInitialization(TestBase tests) { }

        /// <summary>
        /// Run after tests initialization
        /// </summary>
        /// <param name="tests">Initializing tests</param>
        public virtual void OnAfterTestsInitialization(TestBase tests) { }
    }
}
