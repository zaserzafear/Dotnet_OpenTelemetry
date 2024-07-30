using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Services.AuthService;

namespace Services.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServicesLayers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetryExtensions(configuration);
            services.AddMassTransitExtension(configuration);

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
