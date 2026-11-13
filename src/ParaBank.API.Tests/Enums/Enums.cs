namespace ParaBank.API.Tests.Enums
{
    /// <summary>
    /// Represents the HTTP methods supported by the API client.
    /// OOP: Encapsulates HTTP verb semantics as a named type.
    /// </summary>
    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Patch
    }

    /// <summary>
    /// Represents the type of a banking transaction.
    /// OOP: Encapsulates domain knowledge as a named enumeration.
    /// </summary>
    public enum TransactionType
    {
        Credit,
        Debit
    }

    /// <summary>
    /// Represents the type of a bank account.
    /// </summary>
    public enum AccountType
    {
        Checking = 0,
        Savings = 1
    }

    /// <summary>
    /// Log severity level for test output.
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Information,
        Warning,
        Error
    }
}
