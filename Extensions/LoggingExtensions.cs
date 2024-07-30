using Microsoft.Extensions.Logging;

namespace Extensions
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddOpenTelemetryLogging(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddOpenTelemetry(options =>
            {
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
            });

            return loggingBuilder;
        }
    }
}
