using Newtonsoft.Json;
using ParaBank.API.Tests.Contracts;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;

namespace ParaBank.API.Tests.Helpers
{
    /// <summary>
    /// Concrete HTTP client implementation backed by RestSharp and Polly retry.
    /// OOP: Encapsulation — HTTP complexity hidden behind <see cref="IApiClient"/>.
    ///      Single Responsibility — this class only performs HTTP communication.
    ///      Open/Closed — new retry/logging behaviour can be added without touching callers.
    /// </summary>
    public sealed class ApiClient : IApiClient, IDisposable
    {
        // ─── Private state ─────────────────────────────────────────────────────
        private RestClient _restClient;
        private readonly IAppSettings _settings;
        private readonly AsyncRetryPolicy<RestResponse> _retryPolicy;
        private bool _disposed;

        // ─── Constructor ───────────────────────────────────────────────────────
        public ApiClient(IAppSettings settings)
        {
            _settings   = settings ?? throw new ArgumentNullException(nameof(settings));
            _restClient = BuildClient(settings.ApiBaseUrl);
            _retryPolicy = BuildRetryPolicy(settings.RetryCount);
        }

        // ─── IApiClient implementation ─────────────────────────────────────────

        /// <inheritdoc />
        public async Task<IApiResponse<T>> GetAsync<T>(string endpoint)
        {
            var request  = new RestRequest(endpoint, Method.Get);
            request.AddHeader("Accept", "application/json");
            return await ExecuteAsync<T>(request);
        }

        /// <inheritdoc />
        public async Task<IApiResponse<T>> PostAsync<T>(string endpoint, Dictionary<string, string>? parameters = null)
        {
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Accept", "application/json");
            if (parameters is not null)
                foreach (var kv in parameters)
                    request.AddQueryParameter(kv.Key, kv.Value);

            return await ExecuteAsync<T>(request);
        }

        /// <inheritdoc />
        public async Task<IApiResponse<T>> PostJsonAsync<T>(string endpoint, object body)
        {
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(body);
            return await ExecuteAsync<T>(request);
        }

        /// <inheritdoc />
        public void SetBaseUrl(string baseUrl)
        {
            _restClient.Dispose();
            _restClient = BuildClient(baseUrl);
        }

        // ─── Private helpers ───────────────────────────────────────────────────

        private async Task<IApiResponse<T>> ExecuteAsync<T>(RestRequest request)
        {
            Log.Debug("HTTP {Method} {Resource}", request.Method, request.Resource);
            var statusCode = 0;
            var content    = string.Empty;
            try
            {
                var raw = await _retryPolicy.ExecuteAsync(() => _restClient.ExecuteAsync(request));
                statusCode = (int)raw.StatusCode;
                content    = raw.Content ?? string.Empty;
                Log.Debug("Response {StatusCode} from {Resource}", statusCode, request.Resource);

                if (raw.IsSuccessful)
                {
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return ApiResponse<T>.Success(statusCode, default!, content);
                    }

                    if (typeof(T) == typeof(string))
                    {
                        return ApiResponse<T>.Success(statusCode, (T)(object)content, content);
                    }

                    try
                    {
                        var data = JsonConvert.DeserializeObject<T>(content);
                        if (data is not null)
                        {
                            return ApiResponse<T>.Success(statusCode, data, content);
                        }
                    }
                    catch (Exception)
                    {
                        if (typeof(T) == typeof(object))
                        {
                            return ApiResponse<T>.Success(statusCode, (T)(object)content, content);
                        }
                    }

                    return typeof(T) == typeof(object)
                        ? ApiResponse<T>.Success(statusCode, (T)(object)content, content)
                        : ApiResponse<T>.Failure(statusCode, "Deserialization returned null", content);
                }

                var error = raw.ErrorMessage ?? raw.StatusDescription ?? $"HTTP {statusCode}";
                return ApiResponse<T>.Failure(statusCode, error, content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "HTTP request to {Resource} threw an exception", request.Resource);
                return ApiResponse<T>.Failure(statusCode, ex.Message, content);
            }
        }

        private static RestClient BuildClient(string baseUrl) =>
            new(new RestClientOptions(baseUrl)
            {
                ThrowOnDeserializationError = false,
                FailOnDeserializationError  = false
            });

        private static AsyncRetryPolicy<RestResponse> BuildRetryPolicy(int retryCount) =>
            Policy
                .HandleResult<RestResponse>(r => !r.IsSuccessful && (int)r.StatusCode >= 500)
                .Or<Exception>()
                .WaitAndRetryAsync(
                    retryCount: retryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (outcome, delay, attempt, _) =>
                        Log.Warning("Retry {Attempt}/{Max} after {Delay}s. Reason: {Reason}",
                            attempt, retryCount, delay.TotalSeconds,
                            outcome.Exception?.Message ?? ((int)outcome.Result.StatusCode).ToString()));

        // ─── IDisposable ───────────────────────────────────────────────────────
        public void Dispose()
        {
            if (_disposed) return;
            _restClient.Dispose();
            _disposed = true;
        }
    }
}
