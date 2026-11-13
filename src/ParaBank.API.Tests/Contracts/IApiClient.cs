namespace ParaBank.API.Tests.Contracts
{
    /// <summary>
    /// Contract for the REST API client.
    /// OOP: Abstraction — consumers depend on this interface, not the concrete implementation.
    ///      Polymorphism — can be swapped with mock clients in unit tests.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>Sends a GET request and deserializes the response body to type T.</summary>
        Task<IApiResponse<T>> GetAsync<T>(string endpoint);

        /// <summary>Sends a POST request with form/query parameters and deserializes the response.</summary>
        Task<IApiResponse<T>> PostAsync<T>(string endpoint, Dictionary<string, string>? parameters = null);

        /// <summary>Sends a POST request with a JSON body and deserializes the response.</summary>
        Task<IApiResponse<T>> PostJsonAsync<T>(string endpoint, object body);

        /// <summary>Updates the base URL at runtime (e.g. for multi-tenant scenarios).</summary>
        void SetBaseUrl(string baseUrl);
    }
}
