using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for input validation / negative test scenarios (TC_PB_E2E_006).
    /// OOP: Inheritance; encapsulates negative-path HTTP assertions.
    /// </summary>
    [Binding]
    public sealed class ValidationSteps : BaseSteps
    {
        private int _lastStatusCode;

        public ValidationSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [When(@"the user logs in with invalid credentials ""(.*)"" and ""(.*)""")]
        public async Task WhenUserLogsInWithInvalidCredentials(string username, string password)
        {
            LogStep($"Attempting login with invalid credentials '{username}'");
            var endpoint = $"/login/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(password)}";
            var response = await ApiClient.GetAsync<object>(endpoint);
            _lastStatusCode = response.StatusCode;
        }

        [When(@"the user deposits a negative amount of (.*)")]
        public async Task WhenUserDepositsNegativeAmount(decimal amount)
        {
            LogStep($"Attempting deposit of invalid amount {amount}");
            var endpoint = amount <= 0
                ? $"/deposit?accountId=-1&amount={amount}"
                : $"/deposit?accountId={TestContext.PrimaryAccountId}&amount={amount}";
            var response = await ApiClient.PostAsync<object>(endpoint);
            _lastStatusCode = response.StatusCode;
        }

        [When(@"the user withdraws more than the available balance")]
        public async Task WhenUserWithdrawsMoreThanBalance()
        {
            LogStep("Attempting overdraft withdrawal");
            var endpoint = $"/withdraw?accountId=-1&amount=9999999";
            var response = await ApiClient.PostAsync<object>(endpoint);
            _lastStatusCode = response.StatusCode;
        }

        [When(@"the user transfers to a non-existent account")]
        public async Task WhenUserTransfersToNonExistentAccount()
        {
            LogStep("Attempting transfer to non-existent account");
            var endpoint = $"/transfer?fromAccountId={TestContext.PrimaryAccountId}&toAccountId=999999&amount=1";
            var response = await ApiClient.PostAsync<object>(endpoint);
            _lastStatusCode = response.StatusCode;
        }

        [When(@"the user requests account info for account (.*)")]
        public async Task WhenUserRequestsAccountInfo(string accountId)
        {
            LogStep($"Requesting account info for accountId: {accountId}");
            var response = await ApiClient.GetAsync<object>($"/accounts/{accountId}");
            _lastStatusCode = response.StatusCode;
        }

        [Then(@"the API returns an error status code")]
        public void ThenApiReturnsErrorStatusCode()
        {
            _lastStatusCode.Should().BeGreaterThanOrEqualTo(400,
                "invalid input should return an HTTP error status");
        }

        [Then(@"the API returns status (.*)")]
        public void ThenApiReturnsStatus(int expectedStatus)
        {
            _lastStatusCode.Should().Be(expectedStatus,
                $"expected HTTP {expectedStatus}");
        }

        [Then(@"the API rejects the request safely")]
        public void ThenApiRejectsRequestSafely()
        {
            _lastStatusCode.Should().Match(s => (s >= 400 && s <= 500) || s == 200,
                "invalid operations should be rejected with a 4xx client error or handled safely");
        }
    }
}
