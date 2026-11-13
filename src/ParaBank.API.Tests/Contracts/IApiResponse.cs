namespace ParaBank.API.Tests.Contracts
{
    /// <summary>
    /// Contract (Interface) for the API response wrapper.
    /// OOP: Abstraction — hides internal HTTP details from consumers.
    /// </summary>
    /// <typeparam name="T">The deserialized response body type.</typeparam>
    public interface IApiResponse<out T>
    {
        int StatusCode { get; }
        bool IsSuccess { get; }
        T? Data { get; }
        string? ErrorMessage { get; }
        string RawContent { get; }
    }
}
