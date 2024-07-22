using MyApp1.Models;
using MyApp1.Services;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Services.Extensions;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace MyApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure OpenTelemetry logging
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true; // Include message formatting in logs
                logging.IncludeScopes = true; // Include scopes in logs for more context
            });

            // Configure OpenTelemetry metrics and tracing
            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    // Add instrumentation for runtime metrics and custom meters
                    metrics.AddRuntimeInstrumentation() // Collect runtime metrics (e.g., memory, CPU)
                        .AddMeter("Microsoft.AspNetCore.Hosting") // Add metrics for ASP.NET Core hosting
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel") // Add metrics for Kestrel server
                        .AddMeter("System.Net.Http") // Add metrics for HTTP client
                        .AddMeter("MyApp2Client"); // Add custom meter for MyApp2Client
                })
                .WithTracing(tracing =>
                {
                    // Add instrumentation for tracing
                    tracing.AddAspNetCoreInstrumentation() // Collect traces for ASP.NET Core requests
                        .AddHttpClientInstrumentation(); // Collect traces for HTTP client requests
                });

            // Determine if OTLP exporter should be used
            bool useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            // Get service name from configuration or fallback to assembly name
            string OtlserviceName = builder.Configuration["OTEL_SERVICE_NAME"]
                ?? Assembly.GetExecutingAssembly().GetName().Name!;

            if (useOtlpExporter)
            {
                // Add OTLP exporter configuration if enabled
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            // Register ActivitySource for tracing
            builder.Services.AddSingleton(new ActivitySource(OtlserviceName));

            // Register HttpClient
            builder.Services.AddHttpClient();

            // Register Meter
            var meter = new Meter("MyApp2Client");
            builder.Services.AddSingleton(meter);

            // Register MyApp2Client with DI
            builder.Services.AddSingleton<MyApp2Client>();

            // Configure settings for MyApp2
            builder.Services.Configure<MyApp2Settings>(builder.Configuration.GetSection("MyApp2"));

            builder.Services.AddServicesLayers();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
