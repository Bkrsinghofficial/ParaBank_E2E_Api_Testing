using ParaBank.API.Tests.Contracts;
using Serilog;
using Serilog.Events;

namespace ParaBank.API.Tests.Helpers
{
    /// <summary>
    /// Initialises and provides a static Serilog logger for the entire test run.
    /// OOP: Single Responsibility — only handles logger configuration.
    ///      Encapsulation — configuration details hidden behind a static factory.
    /// </summary>
    public static class LoggerHelper
    {
        private static bool _initialised;
        private static readonly object _lock = new();

        /// <summary>
        /// Configures the Serilog global logger once per process.
        /// Safe to call multiple times; subsequent calls are no-ops.
        /// </summary>
        public static void Initialise(IAppSettings settings)
        {
            lock (_lock)
            {
                if (_initialised) return;

                var level = settings.LogLevel.ToUpperInvariant() switch
                {
                    "DEBUG"       => LogEventLevel.Debug,
                    "WARNING"     => LogEventLevel.Warning,
                    "ERROR"       => LogEventLevel.Error,
                    _             => LogEventLevel.Information
                };

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(level)
                    .WriteTo.Console(outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        path: Path.Combine(AppContext.BaseDirectory, "logs", "test-run-.log"),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                _initialised = true;
                Log.Information("Logger initialised at {Level} level", level);
            }
        }

        /// <summary>Flushes and closes the logger at end of test run.</summary>
        public static void CloseAndFlush() => Log.CloseAndFlush();
    }
}
