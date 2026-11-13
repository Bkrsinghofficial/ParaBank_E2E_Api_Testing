using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for side-effect isolation tests (TC_PB_E2E_007).
    /// Verifies that failed/invalid operations do not corrupt account data.
    /// OOP: Inheritance; encapsulates before/after balance state for comparison.
    /// </summary>
    [Binding]
    public sealed class SideEffectSteps : BaseSteps
    {
        private decimal _balanceBeforeInvalidOp;
        private decimal _balanceAfterInvalidOp;

        public SideEffectSteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [When(@"the user records the current balance")]
        public async Task WhenUserRecordsCurrentBalance()
        {
            var response = await ApiClient.GetAsync<Account>($"/accounts/{TestContext.PrimaryAccountId}");
            AssertSuccess(response, "record balance");
            _balanceBeforeInvalidOp = response.Data!.Balance;
            LogStep($"Balance before invalid op: {_balanceBeforeInvalidOp:C}");
        }

        [When(@"the user attempts an invalid withdrawal")]
        public async Task WhenUserAttemptsInvalidWithdrawal()
        {
            LogStep("Attempting invalid withdrawal");
            // Attempt an invalid operation: withdrawal from non-existent account number so the operation fails
            await ApiClient.PostAsync<object>("/withdraw?accountId=999999&amount=100");
        }

        [When(@"the user checks the balance again")]
        public async Task WhenUserChecksBalanceAgain()
        {
            var response = await ApiClient.GetAsync<Account>($"/accounts/{TestContext.PrimaryAccountId}");
            if (response.IsSuccess && response.Data is not null)
                _balanceAfterInvalidOp = response.Data.Balance;
        }

        [Then(@"the balance should be unchanged")]
        public void ThenBalanceShouldBeUnchanged()
        {
            _balanceAfterInvalidOp.Should().Be(_balanceBeforeInvalidOp,
                "a failed invalid operation must not modify the account balance");
        }

        [Then(@"the account data is not corrupted")]
        public async Task ThenAccountDataIsNotCorrupted()
        {
            var response = await ApiClient.GetAsync<Account>($"/accounts/{TestContext.PrimaryAccountId}");
            AssertSuccess(response, "account integrity check");
            response.Data!.Id.Should().Be(TestContext.PrimaryAccountId,
                "account ID must not change after failed operations");
            response.Data.Balance.Should().Be(_balanceBeforeInvalidOp,
                "account balance must remain unchanged after failed operation");
        }
    }
}
