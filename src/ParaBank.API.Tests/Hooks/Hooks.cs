using ParaBank.API.Tests.Configuration;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Helpers;
using Reqnroll;
using Serilog;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ParaBank.API.Tests.Hooks
{
    /// <summary>
    /// Reqnroll lifecycle hooks: wires up DI, logging, and scenario teardown.
    /// OOP: Single Responsibility — only handles test lifecycle (setup/teardown).
    ///      Dependency Inversion — all dependencies injected via constructor injection.
    /// </summary>
    [Binding]
    public sealed class Hooks
    {
        private readonly ITestContext _testContext;
        private readonly ScenarioContext _scenarioContext;

        public Hooks(ITestContext testContext, ScenarioContext scenarioContext)
        {
            _testContext     = testContext;
            _scenarioContext = scenarioContext;
        }

        // ─── Before Test Run ────────────────────────────────────────────────────

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var settings = new AppSettings();
            LoggerHelper.Initialise(settings);
            Log.Information("=== ParaBank API Test Run Started ===");
        }

        // ─── Before Scenario ────────────────────────────────────────────────────

        [BeforeScenario]
        public void BeforeScenario()
        {
            Log.Information("▶ Scenario: {ScenarioTitle}", _scenarioContext.ScenarioInfo.Title);
            _testContext.ClearContext();
        }

        // ─── After Scenario ─────────────────────────────────────────────────────

        [AfterScenario]
        public void AfterScenario()
        {
            var status = _scenarioContext.ScenarioExecutionStatus;
            if (status == ScenarioExecutionStatus.OK)
                Log.Information("✔ PASSED: {ScenarioTitle}", _scenarioContext.ScenarioInfo.Title);
            else
                Log.Warning("✘ FAILED: {ScenarioTitle} — {Error}",
                    _scenarioContext.ScenarioInfo.Title,
                    _scenarioContext.TestError?.Message ?? "Unknown error");

            _testContext.ClearContext();
        }

        // ─── After Test Run ─────────────────────────────────────────────────────

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Log.Information("=== ParaBank API Test Run Completed ===");
            LoggerHelper.CloseAndFlush();
        }
    }
}
