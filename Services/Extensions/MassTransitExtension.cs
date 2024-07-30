using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Configuration;
using Services.Consumers.AuthService;
using Services.Publishers.AuthService;

namespace Services.Extensions
{
    internal static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                var rabbitMqSetting = configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>()!;

                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<AuthServiceConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitMqSetting.Host), h =>
                    {
                        h.Username(rabbitMqSetting.User);
                        h.Password(rabbitMqSetting.Pass);

                        h.ConfigureBatchPublish(x =>
                        {
                            x.Enabled = true;
                            x.Timeout = TimeSpan.FromMilliseconds(2);
                        });
                    });

                    cfg.ReceiveEndpoint("auth_request_queue", e =>
                    {
                        e.ConfigureConsumer<AuthServiceConsumer>(context);
                        e.PrefetchCount = rabbitMqSetting.AuthRequest.PrefetchCount;
                        e.ConcurrentMessageLimit = rabbitMqSetting.AuthRequest.ConcurrentMessageLimit;
                    });

                });
            });


            services.AddScoped<IAuthServicePublisher, AuthServicePublisher>();

            return services;
        }
    }
}
