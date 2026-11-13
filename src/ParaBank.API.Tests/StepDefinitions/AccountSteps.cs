using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for account retrieval and lifecycle scenarios.
    /// OOP: Inheritance from BaseSteps; Encapsulation of account state in TestContext.
    /// </summary>
    [Binding]
    public sealed class AccountSteps : BaseSteps
    {
        private List<Account> _lastAccountList = new();

        public AccountSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [Given(@"the user retrieves their account list")]
        [When(@"the user retrieves their account list")]
        public async Task WhenUserRetrievesAccountList()
        {
            LogStep($"Fetching accounts for customerId {TestContext.CustomerId}");
            var endpoint = $"/customers/{TestContext.CustomerId}/accounts";
            var response = await ApiClient.GetAsync<List<Account>>(endpoint);

            AssertSuccess(response, "account list");
            _lastAccountList = response.Data!;

            // Populate TestContext with account IDs
            for (var i = 0; i < _lastAccountList.Count; i++)
                TestContext.AccountIds[$"Account_{i + 1}"] = _lastAccountList[i].Id;

            if (_lastAccountList.Count > 0)
                TestContext.PrimaryAccountId = _lastAccountList[0].Id;
            if (_lastAccountList.Count > 1)
                TestContext.SecondaryAccountId = _lastAccountList[1].Id;

            LogStep($"Found {_lastAccountList.Count} accounts");
        }

        [Then(@"the account list is not empty")]
        public void ThenAccountListIsNotEmpty()
        {
            _lastAccountList.Should().NotBeEmpty("customer should have at least one account");
        }

        [Then(@"the customerId is saved for later use")]
        public void ThenCustomerIdIsSaved()
        {
            TestContext.CustomerId.Should().BeGreaterThan(0, "customerId must be set before this step");
        }

        [When(@"the user retrieves the account balance")]
        public async Task WhenUserRetrievesAccountBalance()
        {
            LogStep($"Fetching balance for account {TestContext.PrimaryAccountId}");
            var endpoint = $"/accounts/{TestContext.PrimaryAccountId}";
            var response = await ApiClient.GetAsync<Account>(endpoint);

            AssertSuccess(response, "account balance");
            TestContext.AccountBalances["Current"] = response.Data!.Balance;
            LogStep($"Current balance: {response.Data.Balance:C}");
        }

        [Then(@"the balance should equal the starting balance plus (.*) minus (.*)")]
        public void ThenBalanceShouldEqualExpected(decimal depositAmount, decimal withdrawAmount)
        {
            var expected = TestContext.StartingBalance + depositAmount - withdrawAmount;
            var current  = TestContext.AccountBalances["Current"];
            current.Should().Be(expected,
                $"balance after +{depositAmount} -{withdrawAmount} should equal {expected:C}");
        }

        [When(@"the user creates a new (.*) account")]
        public async Task WhenUserCreatesNewAccount(string accountType)
        {
            var typeCode = accountType.ToLower() == "savings" ? 1 : 0;
            LogStep($"Creating new {accountType} account for customer {TestContext.CustomerId}");
            var endpoint = $"/createAccount?customerId={TestContext.CustomerId}&newAccountType={typeCode}&fromAccountId={TestContext.PrimaryAccountId}";
            var response = await ApiClient.PostAsync<Account>(endpoint);

            AssertSuccess(response, "create account");
            TestContext.SecondaryAccountId = response.Data!.Id;
            LogStep($"New account created with id {response.Data.Id}");
        }

        [Then(@"the new account has a positive balance")]
        public async Task ThenNewAccountHasPositiveBalance()
        {
            var endpoint = $"/accounts/{TestContext.SecondaryAccountId}";
            var response = await ApiClient.GetAsync<Account>(endpoint);

            AssertSuccess(response, "new account balance");
            response.Data!.Balance.Should().BeGreaterThan(0, "newly created account should have initial balance");
        }
    }
}
