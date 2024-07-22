using Contract.Auth;

namespace Services.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateAsync(AuthRequest request);
    }
}
