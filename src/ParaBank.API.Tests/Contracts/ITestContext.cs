namespace ParaBank.API.Tests.Contracts
{
    /// <summary>
    /// Contract for shared scenario-scoped test state.
    /// OOP: Abstraction — step definitions depend on this interface, not the concrete store.
    /// </summary>
    public interface ITestContext
    {
        int CustomerId { get; set; }
        int PrimaryAccountId { get; set; }
        int SecondaryAccountId { get; set; }
        decimal StartingBalance { get; set; }
        int LastTransactionId { get; set; }

        Dictionary<string, int> AccountIds { get; }
        Dictionary<string, decimal> AccountBalances { get; }
        Dictionary<string, object> ScenarioData { get; }

        /// <summary>Stores an arbitrary value by key for cross-step access.</summary>
        void Set<T>(string key, T value);

        /// <summary>Retrieves a stored value by key.</summary>
        T? Get<T>(string key);

        /// <summary>Resets all state at end of scenario.</summary>
        void ClearContext();
    }
}
