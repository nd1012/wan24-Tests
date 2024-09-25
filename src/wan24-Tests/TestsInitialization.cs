using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using wan24.Core;
using wan24.ObjectValidation;

namespace wan24.Tests
{
    /// <summary>
    /// Tests initialization
    /// </summary>
    public class TestsInitialization
    {
        /// <summary>
        /// Logger factory
        /// </summary>
        public static ILoggerFactory LoggerFactory { get; private set; } = null!;

        /// <summary>
        /// Options
        /// </summary>
        public static TestsOptionsAttribute Options { get; private set; } = null!;

        /// <summary>
        /// Tests initialization
        /// </summary>
        /// <param name="tc">Test context</param>
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Init(TestContext tc)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Options
            Options = (from ass in TypeHelper.Instance.Assemblies
                                          where ass.GetCustomAttributeCached<TestsOptionsAttribute>() is not null
                                          select ass.GetCustomAttributeCached<TestsOptionsAttribute>())
                                          .FirstOrDefault()
                                          ?? new();
            Options.OnBeforeInitialization();
            // Logging
            if (File.Exists(Options.LogFile)) File.Delete(Options.LogFile);
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(b => b.AddConsole());
            Settings.LogLevel = Options.LogLevel;
            Logging.Logger = FileLogger.CreateAsync(Options.LogFile, Options.LogLevel, LoggerFactory.CreateLogger("Tests")).GetAwaiter().GetResult();
            // Error handling
            ErrorHandling.ErrorHandler = (info) =>
            {
                Logging.WriteError($"Handling error from source #{info.Source}: {info.Info ?? info.Exception.Message ?? "(No information)"}");
                Logging.WriteError(info.Exception.ToString());
                if (info.Exception is StackInfoException six)
                    Logging.WriteError($"Object construction stack: {six.StackInfo.Stack}");
            };
            // Validation errors
            ValidationExtensions.OnObjectValidationFailed += (e) =>
            {
                Logging.WriteWarning($"Object validation for {e.Object.GetType()} failed {(e.ValidationResults.HasValidationException() ? $"exceptional with {e.ValidationResults.Count} errors" : $"with {e.ValidationResults.Count} errors")}");
                foreach (ValidationResult result in e.ValidationResults)
                    Logging.WriteWarning(result.ErrorMessage ?? $"No error message for {(result.MemberNames.Any() ? string.Join(" / ", result.MemberNames) : "unknown member")}");
            };
            ValidateObject.Logger = (message) => Logging.WriteDebug(message);
            // Disposable type construction stack information
            if (Options.CreateDisposableStackInfo)
            {
                DisposableBase.CreateStackInfo = true;
                DisposableRecordBase.CreateStackInfo = true;
                SimpleDisposableBase.CreateStackInfo = true;
                SimpleDisposableRecordBase.CreateStackInfo = true;
                BasicAllDisposableBase.CreateStackInfo = true;
                BasicAllDisposableRecordBase.CreateStackInfo = true;
                BasicAsyncDisposableBase.CreateStackInfo = true;
                BasicAsyncDisposableRecordBase.CreateStackInfo = true;
                BasicDisposableBase.CreateStackInfo = true;
                BasicDisposableRecordBase.CreateStackInfo = true;
            }
            // Boot
            Logging.WriteInfo("Tests initialized");
            Bootstrap.Async().Wait();
            Options.OnAfterInitialization();
        }
    }
}
