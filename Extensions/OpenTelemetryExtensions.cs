using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Reflection;

namespace Extensions
{
    public static class OpenTelemetryExtensions
    {
        /// <summary>
        /// Configures and adds OpenTelemetry services to the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to add OpenTelemetry services to.</param>
        /// <param name="configuration">The configuration settings to use for OpenTelemetry.</param>
        /// <returns>The service collection with OpenTelemetry services added.</returns>
        public static IServiceCollection AddOpenTelemetryExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure logging to use OpenTelemetry
            ConfigureOpenTelemetryLogging(services);

            // Configure metrics collection and reporting
            ConfigureOpenTelemetryMetrics(services);

            // Configure tracing and instrumentations
            ConfigureOpenTelemetryTracing(services);

            // Conditionally add OTLP exporter if the endpoint is configured
            ConfigureOtlpExporter(services, configuration);

            // Register ActivitySource for tracing with a service name
            RegisterActivitySource(services, configuration);

            return services;
        }

        /// <summary>
        /// Configures logging to include OpenTelemetry.
        /// </summary>
        /// <param name="services">The service collection to add logging to.</param>
        private static void ConfigureOpenTelemetryLogging(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddOpenTelemetryLogging();
            });
        }

        /// <summary>
        /// Configures OpenTelemetry metrics collection and reporting.
        /// </summary>
        /// <param name="services">The service collection to add metrics instrumentation to.</param>
        private static void ConfigureOpenTelemetryMetrics(IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()    // Instrumentation for ASP.NET Core
                        .AddHttpClientInstrumentation()    // Instrumentation for HttpClient
                        .AddRuntimeInstrumentation();      // Instrumentation for runtime metrics
                });
        }

        /// <summary>
        /// Configures OpenTelemetry tracing and adds required instrumentations.
        /// </summary>
        /// <param name="services">The service collection to add tracing instrumentation to.</param>
        private static void ConfigureOpenTelemetryTracing(IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()     // Instrumentation for ASP.NET Core
                        .AddHttpClientInstrumentation()     // Instrumentation for HttpClient
                        .AddSource("MassTransit");
                });
        }

        /// <summary>
        /// Conditionally adds OTLP exporter based on the configuration settings.
        /// </summary>
        /// <param name="services">The service collection to add the OTLP exporter to.</param>
        /// <param name="configuration">The configuration settings to check for OTLP exporter endpoint.</param>
        private static void ConfigureOtlpExporter(IServiceCollection services, IConfiguration configuration)
        {
            bool useOtlpExporter = !string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            if (useOtlpExporter)
            {
                services.AddOpenTelemetry()
                    .UseOtlpExporter();    // Add OTLP exporter configuration
            }
        }

        /// <summary>
        /// Registers an ActivitySource for tracing with a service name.
        /// </summary>
        /// <param name="services">The service collection to add the ActivitySource to.</param>
        /// <param name="configuration">The configuration settings to get the service name from.</param>
        private static void RegisterActivitySource(IServiceCollection services, IConfiguration configuration)
        {
            string serviceName = configuration["OTEL_SERVICE_NAME"]
                ?? Assembly.GetExecutingAssembly().GetName().Name!;    // Default to assembly name if not configured

            services.AddSingleton(new ActivitySource(serviceName));
        }
    }
}
