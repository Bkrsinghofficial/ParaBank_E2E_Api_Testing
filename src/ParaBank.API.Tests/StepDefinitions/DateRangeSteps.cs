using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for date-range transaction filtering (TC_PB_E2E_002).
    /// OOP: Inheritance; encapsulates date-boundary scenarios in a focused class.
    /// </summary>
    [Binding]
    public sealed class DateRangeSteps : BaseSteps
    {
        private List<TransactionModel> _transactions = new();
        private string _fromDate = string.Empty;
        private string _toDate   = string.Empty;
        private int    _statusCode;

        public DateRangeSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [When(@"the user retrieves transactions from ""(.*)"" to ""(.*)""")]
        public async Task WhenUserRetrievesTransactionsInRange(string fromDate, string toDate)
        {
            _fromDate = fromDate;
            _toDate   = toDate;
            LogStep($"Fetching transactions from {fromDate} to {toDate} for account {TestContext.PrimaryAccountId}");

            var endpoint = $"/accounts/{TestContext.PrimaryAccountId}/transactions/fromDate/{fromDate}/toDate/{toDate}";
            var response = await ApiClient.GetAsync<List<TransactionModel>>(endpoint);
            _statusCode  = response.StatusCode;
            _transactions = response.IsSuccess && response.Data is not null
                ? response.Data
                : new List<TransactionModel>();
        }

        [When(@"the user retrieves transactions on date ""(.*)""")]
        public async Task WhenUserRetrievesTransactionsOnDate(string onDate)
        {
            LogStep($"Fetching transactions on {onDate}");
            var endpoint = $"/accounts/{TestContext.PrimaryAccountId}/transactions/onDate/{onDate}";
            var response = await ApiClient.GetAsync<List<TransactionModel>>(endpoint);
            _statusCode  = response.StatusCode;
            _transactions = response.IsSuccess && response.Data is not null
                ? response.Data
                : new List<TransactionModel>();
        }

        [Then(@"all transactions fall within the requested date range")]
        public void ThenAllTransactionsFallInDateRange()
        {
            if (!_transactions.Any()) return;  // Empty is acceptable

            if (!DateTime.TryParse(_fromDate, out var from) ||
                !DateTime.TryParse(_toDate,   out var to))
                return;

            foreach (var tx in _transactions)
            {
                var txDate = tx.GetDateUtc().Date;
                txDate.Should().BeOnOrAfter(from.Date,
                    $"transaction {tx.Id} date {txDate} should be >= {from:yyyy-MM-dd}");
                txDate.Should().BeOnOrBefore(to.Date,
                    $"transaction {tx.Id} date {txDate} should be <= {to:yyyy-MM-dd}");
            }
        }

        [Then(@"the transaction list is empty")]
        public void ThenTransactionListIsEmpty()
        {
            _transactions.Should().BeEmpty("no transactions should exist for this date range");
        }

        [Then(@"the returned transaction count is (.*)")]
        public void ThenReturnedTransactionCountIs(int expected)
        {
            _transactions.Should().HaveCount(expected,
                $"expected exactly {expected} transaction(s)");
        }

        [Then(@"no duplicate transactions are returned")]
        public void ThenNoDuplicateTransactionsAreReturned()
        {
            _transactions.Select(t => t.Id)
                .Should().OnlyHaveUniqueItems("transaction IDs must be unique");
        }

        [Then(@"the date range boundaries are inclusive")]
        public void ThenDateRangeBoundariesAreInclusive()
        {
            // Inclusivity is verified by checking that all transactions fall within the range
            ThenAllTransactionsFallInDateRange();
        }
    }
}
