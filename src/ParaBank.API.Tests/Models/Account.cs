using Newtonsoft.Json;
using ParaBank.API.Tests.Enums;

namespace ParaBank.API.Tests.Models
{
    /// <summary>
    /// Represents a bank account returned by the ParaBank API.
    /// OOP: Encapsulation — internal state is protected; exposed via read-only properties.
    /// </summary>
    public sealed class Account
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("customerId")]
        public int CustomerId { get; private set; }

        [JsonProperty("type")]
        public string Type { get; private set; } = string.Empty;

        [JsonProperty("balance")]
        public decimal Balance { get; private set; }

        [JsonConstructor]
        public Account() { }

        /// <summary>
        /// Returns true if this account is a Checking account.
        /// OOP: Behavior encapsulated inside the model rather than scattered in callers.
        /// </summary>
        public bool IsCheckingAccount() =>
            string.Equals(Type, AccountType.Checking.ToString(), StringComparison.OrdinalIgnoreCase);

        /// <summary>Returns true if this account is a Savings account.</summary>
        public bool IsSavingsAccount() =>
            string.Equals(Type, AccountType.Savings.ToString(), StringComparison.OrdinalIgnoreCase);

        public override string ToString() => $"Account[Id={Id}, Type={Type}, Balance={Balance:C}]";
    }
}
