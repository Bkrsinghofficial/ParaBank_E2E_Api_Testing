using ParaBank.API.Tests.Contracts;

namespace ParaBank.API.Tests.Helpers
{
    /// <summary>
    /// Concrete, generic API response wrapper.
    /// OOP: Encapsulation — wraps HTTP status, deserialized data, and error info.
    ///      Generics (Polymorphism) — works for any response body type T.
    /// </summary>
    /// <typeparam name="T">The deserialized response body type.</typeparam>
    public sealed class ApiResponse<T> : IApiResponse<T>
    {
        // ─── Private backing fields ────────────────────────────────────────────
        private readonly int    _statusCode;
        private readonly T?     _data;
        private readonly string? _errorMessage;
        private readonly string  _rawContent;

        // ─── IApiResponse<T> implementation ───────────────────────────────────
        public int     StatusCode    => _statusCode;
        public bool    IsSuccess     => _statusCode is >= 200 and < 300;
        public T?      Data          => _data;
        public string? ErrorMessage  => _errorMessage;
        public string  RawContent    => _rawContent;

        // ─── Constructor (private — use factory methods) ───────────────────────
        private ApiResponse(int statusCode, T? data, string? errorMessage, string rawContent)
        {
            _statusCode   = statusCode;
            _data         = data;
            _errorMessage = errorMessage;
            _rawContent   = rawContent;
        }

        /// <summary>Creates a successful response.</summary>
        public static ApiResponse<T> Success(int statusCode, T data, string rawContent) =>
            new(statusCode, data, null, rawContent);

        /// <summary>Creates a failure response.</summary>
        public static ApiResponse<T> Failure(int statusCode, string errorMessage, string rawContent) =>
            new(statusCode, default, errorMessage, rawContent);

        public override string ToString() =>
            $"ApiResponse<{typeof(T).Name}>[Status={StatusCode}, Success={IsSuccess}]";
    }
}
