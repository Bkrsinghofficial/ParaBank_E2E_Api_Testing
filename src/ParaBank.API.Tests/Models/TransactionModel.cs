using Newtonsoft.Json;

namespace ParaBank.API.Tests.Models
{
    /// <summary>
    /// Represents a single banking transaction returned by the API.
    /// OOP: Encapsulation with private setters; ToString() overrides for debugging.
    /// </summary>
    public sealed class TransactionModel
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("accountId")]
        public int AccountId { get; private set; }

        [JsonProperty("type")]
        public string Type { get; private set; } = string.Empty;

        [JsonProperty("date")]
        public long Date { get; private set; }

        [JsonProperty("amount")]
        public decimal Amount { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; } = string.Empty;

        [JsonConstructor]
        public TransactionModel() { }

        /// <summary>
        /// Returns true if this is a credit transaction.
        /// OOP: Behavior belongs to the model, not scattered across step definitions.
        /// </summary>
        public bool IsCredit() => string.Equals(Type, "Credit", StringComparison.OrdinalIgnoreCase);

        /// <summary>Returns true if this is a debit transaction.</summary>
        public bool IsDebit() => string.Equals(Type, "Debit", StringComparison.OrdinalIgnoreCase);

        /// <summary>Parses the Unix epoch timestamp to a UTC DateTime.</summary>
        public DateTime GetDateUtc() => DateTimeOffset.FromUnixTimeMilliseconds(Date).UtcDateTime;

        public override string ToString() =>
            $"Transaction[Id={Id}, Type={Type}, Amount={Amount:C}, Date={GetDateUtc():yyyy-MM-dd}]";
    }
}
