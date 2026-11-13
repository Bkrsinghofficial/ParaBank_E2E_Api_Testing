using Newtonsoft.Json;

namespace ParaBank.API.Tests.Models
{
    /// <summary>
    /// Represents a bill payee for the /billpay endpoint.
    /// OOP: Composition — Payee "has-a" Address (re-using the Address model).
    /// </summary>
    public sealed class Payee
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("address")]
        public Address Address { get; set; } = new Address();

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonProperty("accountNumber")]
        public int AccountNumber { get; set; }

        [JsonConstructor]
        public Payee() { }

        /// <summary>
        /// Factory method to create a standard test payee.
        /// OOP: Factory pattern — centralises test data creation.
        /// </summary>
        public static Payee CreateTestPayee(string name = "Test Payee", int accountNumber = 12345) =>
            new Payee
            {
                Name = name,
                AccountNumber = accountNumber,
                PhoneNumber = "555-1234"
            };

        public override string ToString() => $"Payee[Name={Name}, AccountNumber={AccountNumber}]";
    }

    /// <summary>
    /// Response returned by the /billpay endpoint.
    /// </summary>
    public sealed class BillPayResult
    {
        [JsonProperty("payeeName")]
        public string PayeeName { get; private set; } = string.Empty;

        [JsonProperty("amount")]
        public decimal Amount { get; private set; }

        [JsonProperty("accountId")]
        public int AccountId { get; private set; }

        [JsonConstructor]
        public BillPayResult() { }

        public override string ToString() =>
            $"BillPayResult[Payee={PayeeName}, Amount={Amount:C}, AccountId={AccountId}]";
    }
}
