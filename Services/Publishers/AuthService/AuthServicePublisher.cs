using Contract.Auth;
using MassTransit;

namespace Services.Publishers.AuthService
{
    public interface IAuthServicePublisher
    {
        Task<AuthResponse> AuthenticateAsync(AuthRequest request);
    }

    internal class AuthServicePublisher : IAuthServicePublisher
    {
        private readonly IRequestClient<AuthRequest> _requestClient;

        public AuthServicePublisher(IRequestClient<AuthRequest> requestClient)
        {
            _requestClient = requestClient;
        }

        public async Task<AuthResponse> AuthenticateAsync(AuthRequest request)
        {
            var response = await _requestClient.GetResponse<AuthResponse>(request);
            var result = response.Message;

            return result;
        }
    }
}
