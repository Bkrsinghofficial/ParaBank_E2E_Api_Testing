using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using Serilog;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Abstract base class for all step definition classes.
    /// OOP: Inheritance — all step classes derive from BaseSteps to inherit common helpers.
    ///      Abstraction — shared assertion logic defined here, not duplicated in subclasses.
    ///      Template Method pattern — subclasses fill in scenario-specific logic.
    /// </summary>
    public abstract class BaseSteps
    {
        // ─── Protected dependencies (available to all subclasses) ───────────────
        protected readonly IApiClient    ApiClient;
        protected readonly ITestContext  TestContext;
        protected readonly IAppSettings  Settings;

        protected BaseSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
        {
            ApiClient   = apiClient   ?? throw new ArgumentNullException(nameof(apiClient));
            TestContext = testContext  ?? throw new ArgumentNullException(nameof(testContext));
            Settings    = settings    ?? throw new ArgumentNullException(nameof(settings));
        }

        // ─── Shared helper methods (Abstraction via protected access) ───────────

        /// <summary>
        /// Asserts that a response was successful and its data is not null.
        /// Reusable across all step subclasses (Inheritance + DRY principle).
        /// </summary>
        protected void AssertSuccess<T>(IApiResponse<T> response, string context = "")
        {
            var ctx = string.IsNullOrWhiteSpace(context) ? typeof(T).Name : context;
            response.Should().NotBeNull($"response for {ctx} should not be null");
            response.StatusCode.Should().BeInRange(200, 299,
                $"{ctx} should return a success status code");
            response.Data.Should().NotBeNull($"{ctx} data should not be null");
        }

        /// <summary>Asserts that a response has the expected HTTP status code.</summary>
        protected static void AssertStatusCode<T>(IApiResponse<T> response, int expectedStatus, string reason = "")
        {
            response.StatusCode.Should().Be(expectedStatus,
                string.IsNullOrWhiteSpace(reason) ? $"expected HTTP {expectedStatus}" : reason);
        }

        /// <summary>Formats today's date using the configured date format.</summary>
        protected string TodayFormatted() =>
            DateTime.Now.ToString(Settings.DateFormat);

        /// <summary>Formats a date using the configured date format.</summary>
        protected string FormatDate(DateTime date) =>
            date.ToString(Settings.DateFormat);

        /// <summary>Logs a step-level informational message.</summary>
        protected static void LogStep(string message) =>
            Log.Information("  ➤ {Step}", message);
    }
}
