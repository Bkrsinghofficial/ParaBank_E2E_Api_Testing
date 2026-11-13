using FluentAssertions;
using Newtonsoft.Json;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for bill payment scenarios (TC_PB_E2E_004).
    /// OOP: Inheritance; Composition — uses Payee model (Payee has-a Address).
    /// </summary>
    [Binding]
    public sealed class BillPaySteps : BaseSteps
    {
        private BillPayResult? _lastBillPayResult;
        private int            _lastStatusCode;
        private decimal        _paidAmount;

        public BillPaySteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [When(@"the user pays a bill of (.*) to payee ""(.*)""")]
        public async Task WhenUserPaysABill(decimal amount, string payeeName)
        {
            _paidAmount = amount;
            // OOP Factory method — creates a standardised test payee
            var payee = Payee.CreateTestPayee(payeeName);

            LogStep($"Paying bill of {amount:C} to '{payeeName}' from account {TestContext.PrimaryAccountId}");

            var endpoint = $"/billpay?accountId={TestContext.PrimaryAccountId}&amount={amount}";
            var response = await ApiClient.PostJsonAsync<BillPayResult>(endpoint, payee);

            _lastStatusCode    = response.StatusCode;
            _lastBillPayResult = response.Data;
        }

        [Then(@"the bill payment returns status 200")]
        public void ThenBillPaymentReturnsStatus200()
        {
            _lastStatusCode.Should().Be(200, "bill payment should succeed");
        }

        [Then(@"the bill payment result contains the payee name")]
        public void ThenBillPaymentResultContainsPayeeName()
        {
            _lastBillPayResult.Should().NotBeNull("bill pay result should not be null");
            _lastBillPayResult!.PayeeName.Should().NotBeNullOrWhiteSpace(
                "payee name should be returned in the response");
        }

        [Then(@"the bill payment amount matches the requested amount")]
        public void ThenBillPaymentAmountMatches()
        {
            _lastBillPayResult.Should().NotBeNull();
            _lastBillPayResult!.Amount.Should().Be(_paidAmount,
                $"bill pay amount should match the requested {_paidAmount:C}");
        }
    }
}
