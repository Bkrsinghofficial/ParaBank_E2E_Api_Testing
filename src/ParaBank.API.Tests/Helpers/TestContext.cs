using ParaBank.API.Tests.Contracts;

namespace ParaBank.API.Tests.Helpers
{
    /// <summary>
    /// Scenario-scoped shared state bag injected into all step definition classes.
    /// OOP: Encapsulation — internal data store is a private dictionary.
    ///      Single Responsibility — stores and retrieves test state only.
    /// </summary>
    public sealed class TestContext : ITestContext
    {
        // ─── Typed shortcut properties ──────────────────────────────────────────
        public int     CustomerId         { get; set; }
        public int     PrimaryAccountId   { get; set; }
        public int     SecondaryAccountId { get; set; }
        public decimal StartingBalance    { get; set; }
        public int     LastTransactionId  { get; set; }

        // ─── Collection state ───────────────────────────────────────────────────
        public Dictionary<string, int>     AccountIds      { get; } = new();
        public Dictionary<string, decimal> AccountBalances { get; } = new();
        public Dictionary<string, object>  ScenarioData    { get; } = new();

        // ─── Generic key-value store ────────────────────────────────────────────

        /// <inheritdoc />
        public void Set<T>(string key, T value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ScenarioData[key] = value!;
        }

        /// <inheritdoc />
        public T? Get<T>(string key)
        {
            if (ScenarioData.TryGetValue(key, out var val) && val is T typed)
                return typed;
            return default;
        }

        /// <inheritdoc />
        public void ClearContext()
        {
            CustomerId         = 0;
            PrimaryAccountId   = 0;
            SecondaryAccountId = 0;
            StartingBalance    = 0m;
            LastTransactionId  = 0;
            AccountIds.Clear();
            AccountBalances.Clear();
            ScenarioData.Clear();
        }
    }
}
