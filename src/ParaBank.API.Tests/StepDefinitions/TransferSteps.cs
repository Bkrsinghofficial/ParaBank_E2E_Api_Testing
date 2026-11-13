using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for fund transfer consistency scenarios (TC_PB_E2E_003).
    /// OOP: Inheritance; encapsulates transfer-specific state (balances before/after).
    /// </summary>
    [Binding]
    public sealed class TransferSteps : BaseSteps
    {
        private decimal _sourceBalanceBefore;
        private decimal _targetBalanceBefore;
        private int     _transferStatusCode;

        public TransferSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [When(@"the user transfers (.*) from the first account to the second account")]
        public async Task WhenUserTransfersFunds(decimal amount)
        {
            // Capture balances before transfer
            _sourceBalanceBefore = await FetchBalance(TestContext.PrimaryAccountId);
            _targetBalanceBefore = await FetchBalance(TestContext.SecondaryAccountId);

            LogStep($"Transferring {amount:C} from {TestContext.PrimaryAccountId} to {TestContext.SecondaryAccountId}");

            var endpoint = "/transfer" +
                           $"?fromAccountId={TestContext.PrimaryAccountId}" +
                           $"&toAccountId={TestContext.SecondaryAccountId}" +
                           $"&amount={amount}";
            var response       = await ApiClient.PostAsync<object>(endpoint);
            _transferStatusCode = response.StatusCode;

            TestContext.Set("TransferAmount", amount);
        }

        [Then(@"the transfer returns status 200")]
        public void ThenTransferReturnsStatus200()
        {
            _transferStatusCode.Should().Be(200, "transfer should succeed");
        }

        [Then(@"the source account balance is reduced by the transfer amount")]
        public async Task ThenSourceBalanceIsReduced()
        {
            var amount  = TestContext.Get<decimal>("TransferAmount");
            var current = await FetchBalance(TestContext.PrimaryAccountId);
            current.Should().Be(_sourceBalanceBefore - amount,
                $"source balance should decrease by {amount:C}");
        }

        [Then(@"the target account balance is increased by the transfer amount")]
        public async Task ThenTargetBalanceIsIncreased()
        {
            var amount  = TestContext.Get<decimal>("TransferAmount");
            var current = await FetchBalance(TestContext.SecondaryAccountId);
            current.Should().Be(_targetBalanceBefore + amount,
                $"target balance should increase by {amount:C}");
        }

        // ─── Private helper ────────────────────────────────────────────────────

        private async Task<decimal> FetchBalance(int accountId)
        {
            var response = await ApiClient.GetAsync<Account>($"/accounts/{accountId}");
            return response.IsSuccess ? response.Data!.Balance : 0m;
        }
    }
}
