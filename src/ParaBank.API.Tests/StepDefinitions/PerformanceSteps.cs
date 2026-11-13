using FluentAssertions;
using Newtonsoft.Json;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;
using System.Diagnostics;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for contract compliance and performance tests (TC_PB_E2E_010).
    /// OOP: Inheritance; measures and asserts response time as a non-functional requirement.
    /// </summary>
    [Binding]
    public sealed class PerformanceSteps : BaseSteps
    {
        private TimeSpan _lastResponseTime;
        private int      _lastStatusCode;
        private string   _lastRawContent = string.Empty;

        public PerformanceSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [When(@"the user retrieves the account list and measures response time")]
        public async Task WhenUserRetrievesAccountListAndMeasures()
        {
            LogStep("Measuring account list response time");
            var sw = Stopwatch.StartNew();
            var response = await ApiClient.GetAsync<List<Account>>(
                $"/customers/{TestContext.CustomerId}/accounts");
            sw.Stop();

            _lastResponseTime = sw.Elapsed;
            _lastStatusCode   = response.StatusCode;
            _lastRawContent   = response.RawContent;
            LogStep($"Response time: {_lastResponseTime.TotalMilliseconds:F0}ms");
        }

        [When(@"the user retrieves transactions and measures response time")]
        public async Task WhenUserRetrievesTransactionsAndMeasures()
        {
            var today = TodayFormatted();
            LogStep("Measuring transaction list response time");
            var sw = Stopwatch.StartNew();
            var response = await ApiClient.GetAsync<List<TransactionModel>>(
                $"/accounts/{TestContext.PrimaryAccountId}/transactions/fromDate/{today}/toDate/{today}");
            sw.Stop();

            _lastResponseTime = sw.Elapsed;
            _lastStatusCode   = response.StatusCode;
            _lastRawContent   = response.RawContent;
        }

        [Then(@"the response time is under (.*) seconds")]
        public void ThenResponseTimeIsUnder(double maxSeconds)
        {
            _lastResponseTime.TotalSeconds.Should().BeLessThan(maxSeconds,
                $"API response should arrive within {maxSeconds}s");
            LogStep($"✓ Response time {_lastResponseTime.TotalMilliseconds:F0}ms < {maxSeconds * 1000}ms");
        }

        [Then(@"the response is valid JSON")]
        public void ThenResponseIsValidJson()
        {
            _lastRawContent.Should().NotBeNullOrWhiteSpace("response body should not be empty");

            Action parse = () => JsonConvert.DeserializeObject(_lastRawContent);
            parse.Should().NotThrow("response body must be valid JSON");
        }

        [Then(@"the response status is successful")]
        public void ThenResponseStatusIsSuccessful()
        {
            _lastStatusCode.Should().BeInRange(200, 299, "response should be a 2xx success");
        }

        [Then(@"the account list response contains id, customerId, type and balance fields")]
        public void ThenAccountListResponseContainsRequiredFields()
        {
            _lastRawContent.Should().Contain("\"id\"",         "account response must contain 'id'");
            _lastRawContent.Should().Contain("\"customerId\"", "account response must contain 'customerId'");
            _lastRawContent.Should().Contain("\"type\"",       "account response must contain 'type'");
            _lastRawContent.Should().Contain("\"balance\"",    "account response must contain 'balance'");
        }
    }
}
