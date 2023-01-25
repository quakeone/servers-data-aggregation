using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.IO;

namespace ServersDataAggregation.Common;

public static class Logging
{
    private static ILogger logger;
    public static void Initialize()
    {
        if (logger != null)
            return;
        logger = LoggerFactory.Create(options => {
            options.SetMinimumLevel(LogLevel.Debug);
            //options.AddConsole(options =>
            //{

            //    options.IncludeScopes = false;
            //});
            options.AddConsole(options => options.FormatterName = nameof(CustomLoggingFormatter))
                 .AddConsoleFormatter<CustomLoggingFormatter, ConsoleFormatterOptions>();
        }).CreateLogger("baseLogger");
    }

    public static void LogWarning(string message) => logger.LogWarning(message);
    public static void LogError(Exception ex, string message) => logger.LogError(ex, message);
    public static void LogInfo(string message) => logger.LogInformation(message);
    public static void LogTrace(string message) => logger.LogTrace(message);
    public static void LogDebug(string message) => logger.LogDebug(message);


    // Taken & modified from
    // https://learn.microsoft.com/en-us/answers/questions/234610/net-core-removing-the-type-name-from-logging-outpu
    public sealed class CustomLoggingFormatter : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable _optionsReloadToken;
        private ConsoleFormatterOptions _formatterOptions;
        public CustomLoggingFormatter(IOptionsMonitor<ConsoleFormatterOptions> options)
    // Case insensitive  
    : base(nameof(CustomLoggingFormatter)) =>
    (_optionsReloadToken, _formatterOptions) =
        (options.OnChange(ReloadLoggerOptions), options.CurrentValue);
        private void ReloadLoggerOptions(ConsoleFormatterOptions options) =>
    _formatterOptions = options;

        public void WriteLevel(TextWriter writer, LogLevel level) {
            var color = ConsoleColor.White;
            switch(level)
            {
                case LogLevel.Debug: color = ConsoleColor.Green; break;
                case LogLevel.Warning: color = ConsoleColor.Yellow; break;
                case LogLevel.Error: color = ConsoleColor.Red; break;
            }
            writer.WriteWithColor(
               $"{level}",
                ConsoleColor.Black,
                color);
            writer.Write(": ");
        }

        public override void Write<TState>(
            in LogEntry<TState> logEntry,
            IExternalScopeProvider scopeProvider,
            TextWriter textWriter)
        {
            string? message =
                logEntry.Formatter?.Invoke(
                    logEntry.State, logEntry.Exception);

            if (message is null)
            {
                return;
            }

            WriteLevel(textWriter, logEntry.LogLevel);
            textWriter.Write($"{message}\n");
        }
        public void Dispose() => _optionsReloadToken?.Dispose();
    }
}
