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
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            // Configure OpenTelemetry metrics and tracing
            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    // Add instrumentation for metrics
                    metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("MyApp2Client"); // Add custom meter
                })
                .WithTracing(tracing =>
                {
                    // Add instrumentation for tracing
                    tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
                });

            // Determine if OTLP exporter should be used
            bool useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            if (useOtlpExporter)
            {
                // Add OTLP exporter configuration if enabled
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            // Get service name from configuration or fallback to assembly name
            string OtlserviceName = builder.Configuration["OTEL_SERVICE_NAME"]
                ?? Assembly.GetExecutingAssembly().GetName().Name!;
            // Register ActivitySource for tracing
            builder.Services.AddSingleton(new ActivitySource(OtlserviceName));

            // Register HttpClient
            builder.Services.AddHttpClient();

            // Register Meter
            var meter = new Meter("MyApp2Client");
            builder.Services.AddSingleton(meter);

            // Register MyApp2Client
            builder.Services.AddSingleton<MyApp2Client>();

            // Configure settings for MyApp2
            builder.Services.Configure<MyApp2Settings>(builder.Configuration.GetSection("MyApp2"));

            // Add services for application layers (e.g., business logic, data access, etc.)
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
