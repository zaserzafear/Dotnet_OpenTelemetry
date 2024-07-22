using Microsoft.Extensions.DependencyInjection;
using Services.Services.AuthService;

namespace Services.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServicesLayers(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();

            return services;
        }

    }
}
