using Reqnroll.BoDi;
using ParaBank.API.Tests.Configuration;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Helpers;
using Reqnroll;

namespace ParaBank.API.Tests.Hooks
{
    /// <summary>
    /// Configures the Reqnroll dependency injection container.
    /// OOP: Dependency Inversion — binds interfaces to concrete types; callers never depend on concretions.
    /// </summary>
    [Binding]
    public static class DependencyContainer
    {
        [BeforeTestRun]
        public static void RegisterDependencies(IObjectContainer container)
        {
            // ── Configuration ──────────────────────────────────────────────────
            var settings = new AppSettings();
            container.RegisterInstanceAs<IAppSettings>(settings);

            // ── HTTP Client (one per test run for connection re-use) ───────────
            var apiClient = new ApiClient(settings);
            container.RegisterInstanceAs<IApiClient>(apiClient);
        }

        [BeforeScenario(Order = 0)]
        public static void RegisterScenarioDependencies(IObjectContainer container)
        {
            // ── Scenario-scoped shared test state ──────────────────────────────
            container.RegisterTypeAs<TestContext, ITestContext>();
        }
    }
}
