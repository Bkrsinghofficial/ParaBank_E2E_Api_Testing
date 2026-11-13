using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for deposit, withdrawal, and transaction retrieval.
    /// OOP: Inherits from BaseSteps; encapsulates transaction state privately.
    /// </summary>
    [Binding]
    public sealed class TransactionSteps : BaseSteps
    {
        private List<TransactionModel> _lastTransactionList = new();
        private int _lastResponseStatusCode;

        public TransactionSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        // ─── Deposit ───────────────────────────────────────────────────────────

        [Given(@"the user deposits (.*) to the first account")]
        [When(@"the user deposits (.*) to the first account")]
        [Then(@"the user deposits (.*) to the first account")]
        public async Task WhenUserDeposits(decimal amount)
        {
            // Capture balance before deposit
            var balanceResponse = await ApiClient.GetAsync<Account>($"/accounts/{TestContext.PrimaryAccountId}");
            if (balanceResponse.IsSuccess && balanceResponse.Data is not null)
                TestContext.StartingBalance = balanceResponse.Data.Balance;

            LogStep($"Depositing {amount:C} to account {TestContext.PrimaryAccountId}");
            var endpoint = "/deposit" +
                           $"?accountId={TestContext.PrimaryAccountId}" +
                           $"&amount={amount}";
            var response = await ApiClient.PostAsync<object>(endpoint);
            _lastResponseStatusCode = response.StatusCode;
        }

        [Then(@"the deposit returns status 200")]
        public void ThenDepositReturnsStatus200()
        {
            _lastResponseStatusCode.Should().Be(200, "deposit should succeed with HTTP 200");
        }

        // ─── Withdrawal ────────────────────────────────────────────────────────

        [Given(@"the user withdraws (.*) from the first account")]
        [When(@"the user withdraws (.*) from the first account")]
        [Then(@"the user withdraws (.*) from the first account")]
        public async Task WhenUserWithdraws(decimal amount)
        {
            LogStep($"Withdrawing {amount:C} from account {TestContext.PrimaryAccountId}");
            var endpoint = "/withdraw" +
                           $"?accountId={TestContext.PrimaryAccountId}" +
                           $"&amount={amount}";
            var response = await ApiClient.PostAsync<object>(endpoint);
            _lastResponseStatusCode = response.StatusCode;
        }

        [Then(@"the withdrawal returns status 200")]
        public void ThenWithdrawalReturnsStatus200()
        {
            _lastResponseStatusCode.Should().Be(200, "withdrawal should succeed with HTTP 200");
        }

        // ─── Transaction retrieval ─────────────────────────────────────────────

        [When(@"the user retrieves transactions for today")]
        public async Task WhenUserRetrievesTransactionsForToday()
        {
            var today = TodayFormatted();
            LogStep($"Retrieving transactions for today ({today})");
            var endpoint = $"/accounts/{TestContext.PrimaryAccountId}/transactions/fromDate/{today}/toDate/{today}";
            var response = await ApiClient.GetAsync<List<TransactionModel>>(endpoint);

            _lastResponseStatusCode = response.StatusCode;
            _lastTransactionList = response.IsSuccess && response.Data is not null
                ? response.Data
                : new List<TransactionModel>();
        }

        [Then(@"the response status is 200")]
        public void ThenResponseStatusIs200()
        {
            _lastResponseStatusCode.Should().Be(200);
        }

        [Then(@"the transaction list contains a Credit of (.*)")]
        public void ThenTransactionListContainsCredit(decimal amount)
        {
            _lastTransactionList.Should().Contain(
                t => t.IsCredit() && t.Amount == amount,
                $"expected a Credit transaction of {amount:C}");
        }

        [Then(@"the transaction list contains a Debit of (.*)")]
        public void ThenTransactionListContainsDebit(decimal amount)
        {
            _lastTransactionList.Should().Contain(
                t => t.IsDebit() && t.Amount == amount,
                $"expected a Debit transaction of {amount:C}");
        }

        [Then(@"the transaction list is not empty")]
        public void ThenTransactionListIsNotEmpty()
        {
            _lastTransactionList.Should().NotBeEmpty("transactions should be returned");
        }

        [Then(@"transactions have no duplicates")]
        public void ThenTransactionsHaveNoDuplicates()
        {
            var ids = _lastTransactionList.Select(t => t.Id).ToList();
            ids.Should().OnlyHaveUniqueItems("transaction IDs must be unique");
        }
    }
}
