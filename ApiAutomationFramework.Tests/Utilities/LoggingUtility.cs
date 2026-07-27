using System;
using System.IO;
using System.Threading;
using ApiAutomationFramework.Configuration;
using Serilog;
using Serilog.Events;

namespace ApiAutomationFramework.Utilities;

public static class LoggingUtility
{
    private static bool _isConfigured = false;
    private static readonly object _lock = new();

    public static void ConfigureSerilog()
    {
        if (_isConfigured) return;

        lock (_lock)
        {
            if (_isConfigured) return;

            var settings = FrameworkConfigurationManager.Instance.Settings;
            var reportsDir = settings.Reporting.OutputDirectory;
            var logsDir = Path.Combine(reportsDir, "Logs");

            Directory.CreateDirectory(logsDir);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.WithProperty("ThreadId", Thread.CurrentThread.ManagedThreadId)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logsDir, "test-execution-.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30)
                .CreateLogger();

            _isConfigured = true;

            Log.Information("Logging configured. Log directory: {LogDir}", logsDir);
        }
    }

    public static void FlushAndClose() => Log.CloseAndFlush();
}