using FluentAssertions;
using ParaBank.API.Tests.Contracts;
using ParaBank.API.Tests.Models;
using Reqnroll;

namespace ParaBank.API.Tests.StepDefinitions
{
    /// <summary>
    /// Step definitions for data isolation / security tests (TC_PB_E2E_009).
    /// Verifies that a user cannot access another user's accounts.
    /// OOP: Inheritance; separate ApiClient calls per user demonstrate isolation.
    /// </summary>
    [Binding]
    public sealed class SecuritySteps : BaseSteps
    {
        private int  _otherCustomerId;
        private int  _otherAccountId;
        private int  _unauthorisedStatusCode;

        public SecuritySteps(IApiClient apiClient, ITestContext testContext, IAppSettings settings)
            : base(apiClient, testContext, settings) { }

        [Given(@"a second user exists with username ""(.*)"" and password ""(.*)""")]
        public async Task GivenSecondUserExists(string username, string password)
        {
            LogStep($"Logging in as second user '{username}'");
            var endpoint = $"/login/{Uri.EscapeDataString(username)}/{Uri.EscapeDataString(password)}";
            var response = await ApiClient.GetAsync<Customer>(endpoint);

            // Store the other user's customerId
            if (response.IsSuccess && response.Data is not null)
            {
                _otherCustomerId = response.Data.Id;
                TestContext.Set("OtherCustomerId", _otherCustomerId);

                // Grab the first account of the second user
                var acctResp = await ApiClient.GetAsync<List<Account>>(
                    $"/customers/{_otherCustomerId}/accounts");
                if (acctResp.IsSuccess && acctResp.Data?.Count > 0)
                    _otherAccountId = acctResp.Data[0].Id;
            }
        }

        [When(@"the first user tries to access the second user's accounts")]
        public async Task WhenFirstUserTriesToAccessSecondUsersAccounts()
        {
            // Primary user attempts to see the second user's account data
            LogStep($"User {TestContext.CustomerId} attempting to access account {_otherAccountId}");
            var response = await ApiClient.GetAsync<Account>($"/accounts/{_otherAccountId}");
            _unauthorisedStatusCode = response.StatusCode;
        }

        [Then(@"the API does not return the other user's private data")]
        public void ThenApiDoesNotReturnOtherUsersPrivateData()
        {
            // ParaBank returns 500 or 404 when the account doesn't belong to the requester
            _unauthorisedStatusCode.Should().NotBe(200,
                "users should not be able to access each other's account data");
        }

        [Then(@"the customer IDs of the two users are different")]
        public void ThenCustomerIdsAreDifferent()
        {
            TestContext.CustomerId.Should().NotBe(_otherCustomerId,
                "two different users must have different customer IDs");
        }
    }
}
