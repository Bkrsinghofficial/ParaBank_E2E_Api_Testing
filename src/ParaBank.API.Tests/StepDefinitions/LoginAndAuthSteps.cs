using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for login and authentication scenarios.
    /// OOP: Inheritance from BaseSteps; Single Responsibility — only handles auth steps.
    /// </summary>
    [Binding]
    public sealed class LoginAndAuthSteps : BaseSteps
    {
        public LoginAndAuthSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [Given(@"the user logs in with username ""(.*)"" and password ""(.*)""")]
        public async Task GivenTheUserLogsIn(string username, string password)
        {
            LogStep($"Logging in as '{username}'");
            var endpoint = $"/login/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(password)}";
            var response = await ApiClient.GetAsync<Customer>(endpoint);

            AssertSuccess(response, "login");
            TestContext.CustomerId = response.Data!.Id;
            LogStep($"Logged in — customerId: {TestContext.CustomerId}");
        }

        [Given(@"the user logs in with default credentials")]
        public async Task GivenTheUserLogsInWithDefaultCredentials()
        {
            await GivenTheUserLogsIn(Settings.DefaultUsername, Settings.DefaultPassword);
        }

        [Then(@"the login returns customerId greater than zero")]
        public void ThenLoginReturnsValidCustomerId()
        {
            TestContext.CustomerId.Should().BeGreaterThan(0, "customerId should be a positive integer");
        }
    }
}
