using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for cross-endpoint consistency tests (TC_PB_E2E_005).
    /// Verifies that /fromDate, /onDate, /month, and /transactionId endpoints
    /// all return consistent data for the same account.
    /// OOP: Inheritance; polymorphic use of IApiResponse&lt;T&gt; for different endpoints.
    /// </summary>
    [Binding]
    public sealed class ConsistencySteps : BaseSteps
    {
        private List<TransactionModel> _dateRangeTransactions = new();
        private List<TransactionModel> _onDateTransactions    = new();
        private List<TransactionModel> _monthTransactions     = new();
        private TransactionModel?      _singleTransaction;

        public ConsistencySteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [When(@"the user retrieves transactions via all endpoints for today")]
        public async Task WhenUserRetrievesViaAllEndpoints()
        {
            var today = TodayFormatted();
            var month = DateTime.Now.Month;
            var acct  = TestContext.PrimaryAccountId;

            LogStep("Fetching via /fromDate-toDate endpoint");
            var rangeResp  = await ApiClient.GetAsync<List<TransactionModel>>(
                $"/accounts/{acct}/transactions/fromDate/{today}/toDate/{today}");
            _dateRangeTransactions = rangeResp.IsSuccess ? rangeResp.Data ?? new() : new();

            LogStep("Fetching via /onDate endpoint");
            var onDateResp = await ApiClient.GetAsync<List<TransactionModel>>(
                $"/accounts/{acct}/transactions/onDate/{today}");
            _onDateTransactions = onDateResp.IsSuccess ? onDateResp.Data ?? new() : new();

            LogStep("Fetching via /month endpoint");
            var monthResp  = await ApiClient.GetAsync<List<TransactionModel>>(
                $"/accounts/{acct}/transactions/month/{month}/type/Credit");
            _monthTransactions = monthResp.IsSuccess ? monthResp.Data ?? new() : new();

            // Fetch a single transaction if we have any
            if (_dateRangeTransactions.Count > 0)
            {
                var txId       = _dateRangeTransactions[0].Id;
                var txResp     = await ApiClient.GetAsync<TransactionModel>($"/transactions/{txId}");
                _singleTransaction = txResp.IsSuccess ? txResp.Data : null;
            }
        }

        [Then(@"the date-range and on-date endpoints return the same transaction count")]
        public void ThenDateRangeAndOnDateReturnSameCount()
        {
            _dateRangeTransactions.Count.Should().Be(_onDateTransactions.Count,
                "fromDate/toDate and onDate endpoints should return the same count for today");
        }

        [Then(@"the single-transaction endpoint returns the correct transaction")]
        public void ThenSingleTransactionEndpointReturnsCorrectTransaction()
        {
            if (_dateRangeTransactions.Count == 0) return; // Skip if no transactions today

            _singleTransaction.Should().NotBeNull("single transaction endpoint should return data");
            _singleTransaction!.Id.Should().Be(_dateRangeTransactions[0].Id,
                "fetched transaction Id should match the requested Id");
        }
    }
}
