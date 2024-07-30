using Contract.Auth;
using MassTransit;
using Services.Services.AuthService;

namespace Services.Consumers.AuthService
{
    public class AuthServiceConsumer : IConsumer<AuthRequest>
    {
        private readonly IAuthService _authService;

        public AuthServiceConsumer(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task Consume(ConsumeContext<AuthRequest> context)
        {
            var result = await _authService.AuthenticateAsync(context.Message);

            await context.RespondAsync(result);
        }
    }
}
