using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for account lifecycle state transitions (TC_PB_E2E_008).
    /// OOP: Inheritance; demonstrates state transition testing pattern.
    /// </summary>
    [Binding]
    public sealed class AccountLifecycleSteps : BaseSteps
    {
        private int _newAccountId;

        public AccountLifecycleSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [When(@"the user creates a savings account funded by the first account")]
        public async Task WhenUserCreatesSavingsAccount()
        {
            LogStep($"Creating savings account for customer {TestContext.CustomerId}");
            var endpoint = $"/createAccount?customerId={TestContext.CustomerId}&newAccountType=1&fromAccountId={TestContext.PrimaryAccountId}";
            var response = await ApiClient.PostAsync<Account>(endpoint);

            AssertSuccess(response, "create savings account");
            _newAccountId = response.Data!.Id;
            TestContext.SecondaryAccountId = _newAccountId;
            LogStep($"Created savings account: {_newAccountId}");
        }

        [Then(@"the new account appears in the customer's account list")]
        public async Task ThenNewAccountAppearsInList()
        {
            var endpoint = $"/customers/{TestContext.CustomerId}/accounts";
            var response = await ApiClient.GetAsync<List<Account>>(endpoint);

            AssertSuccess(response, "customer account list");
            response.Data.Should().Contain(a => a.Id == _newAccountId,
                $"new account {_newAccountId} should appear in the account list");
        }

        [Then(@"deposits can be made to the new account")]
        public async Task ThenDepositsCanBeMadeToNewAccount()
        {
            LogStep($"Depositing to new account {_newAccountId}");
            var endpoint = $"/deposit?accountId={_newAccountId}&amount=50";
            var response = await ApiClient.PostAsync<object>(endpoint);
            response.StatusCode.Should().Be(200, "deposit to new account should succeed");
        }

        [Then(@"the new account balance reflects the deposit")]
        public async Task ThenNewAccountBalanceReflectsDeposit()
        {
            var response = await ApiClient.GetAsync<Account>($"/accounts/{_newAccountId}");
            AssertSuccess(response, "new account balance");
            response.Data!.Balance.Should().BeGreaterThan(0m,
                "balance should be positive after deposit");
        }
    }
}
