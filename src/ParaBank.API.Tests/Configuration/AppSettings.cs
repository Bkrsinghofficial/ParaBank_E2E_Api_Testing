using Microsoft.Extensions.Configuration;
using ParaBank.API.Tests.Contracts;

namespace ParaBank.API.Tests.Configuration
{
    /// <summary>
    /// Concrete implementation of <see cref="IAppSettings"/> that reads from appsettings.json.
    /// OOP: Encapsulation — configuration loading logic is hidden; only the interface is exposed.
    ///      Single Responsibility — this class only handles configuration.
    /// </summary>
    public sealed class AppSettings : IAppSettings
    {
        // ─── Private backing fields ────────────────────────────────────────────
        private readonly string _apiBaseUrl;
        private readonly string _defaultUsername;
        private readonly string _defaultPassword;
        private readonly int _timeoutSeconds;
        private readonly int _retryCount;
        private readonly string _dateFormat;
        private readonly string _logLevel;

        // ─── Public interface implementation ───────────────────────────────────
        public string ApiBaseUrl       => _apiBaseUrl;
        public string DefaultUsername  => _defaultUsername;
        public string DefaultPassword  => _defaultPassword;
        public int    TimeoutSeconds   => _timeoutSeconds;
        public int    RetryCount       => _retryCount;
        public string DateFormat       => _dateFormat;
        public string LogLevel         => _logLevel;

        /// <summary>
        /// Initialises settings from the JSON configuration file.
        /// </summary>
        public AppSettings()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _apiBaseUrl      = config["ApiBaseUrl"]      ?? "https://parabank.parasoft.com/parabank/services/bank";
            _defaultUsername = config["DefaultUsername"] ?? "john";
            _defaultPassword = config["DefaultPassword"] ?? "demo";
            _timeoutSeconds  = int.TryParse(config["TimeoutSeconds"], out var t) ? t : 10;
            _retryCount      = int.TryParse(config["RetryCount"],     out var r) ? r : 3;
            _dateFormat      = config["DateFormat"]      ?? "MM-dd-yyyy";
            _logLevel        = config["LogLevel"]        ?? "Information";
        }

        public override string ToString() =>
            $"AppSettings[BaseUrl={_apiBaseUrl}, User={_defaultUsername}, Timeout={_timeoutSeconds}s]";
    }
}
