namespace ParaBank.API.Tests.Contracts
{
    /// <summary>
    /// Contract for application configuration settings.
    /// OOP: Abstraction — decouples config reading from consuming classes.
    /// </summary>
    public interface IAppSettings
    {
        string ApiBaseUrl { get; }
        string DefaultUsername { get; }
        string DefaultPassword { get; }
        int TimeoutSeconds { get; }
        int RetryCount { get; }
        string DateFormat { get; }
        string LogLevel { get; }
    }
}
