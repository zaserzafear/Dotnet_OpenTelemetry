using Contract.Auth;
using System.Diagnostics;

namespace Services.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly ActivitySource _activitySource;

        public AuthService(ActivitySource activitySource)
        {
            _activitySource = activitySource;
        }

        public async Task<AuthResponse> AuthenticateAsync(AuthRequest request)
        {
            // Start the main span for authentication
            using (var mainActivity = _activitySource.StartActivity("AuthenticateAsync"))
            {
                // Simulate connecting to a database and querying for user credentials
                (string? userId, string? username)? user;
                using (var dbActivity = _activitySource.StartActivity("DbQuery", ActivityKind.Internal))
                {
                    dbActivity?.AddTag("operation", "query");
                    dbActivity?.AddTag("db.type", "SQL");

                    // Simulate database query
                    user = await SimulateDatabaseQueryAsync(request.Username, request.Password);

                    // Check if user was found
                    if (user == null)
                    {
                        mainActivity?.AddTag("auth.result", "failure");
                        throw new UnauthorizedAccessException("Invalid credentials");
                    }

                    mainActivity?.AddTag("auth.result", "success");
                }

                // Simulate generating a token
                using (var tokenActivity = _activitySource.StartActivity("TokenGeneration", ActivityKind.Internal))
                {
                    tokenActivity?.AddTag("operation", "generate");
                    tokenActivity?.AddTag("token.type", "JWT");

                    // Simulate token generation
                    var token = await SimulateTokenGenerationAsync(user.Value.username!);

                    // Create the response object
                    var response = new AuthResponse
                    {
                        Token = token,
                        UserId = user.Value.userId!,
                        Username = user.Value.username!,
                        ExpiresAt = DateTime.UtcNow.AddHours(1)
                    };

                    // Add additional information to the main span
                    mainActivity?.SetTag("response.token", response.Token);
                    mainActivity?.SetTag("response.userId", response.UserId);

                    return response;
                }
            }
        }

        // Simulate a database query
        private async Task<(string? userId, string? username)?> SimulateDatabaseQueryAsync(string username, string password)
        {
            // Simulate some async work, e.g., querying a database
            await Task.Delay(10); // Simulate delay for database operation

            // Simulated user data
            // In a real scenario, you would query the database to get the user details
            return (username == "valid_user" && password == "valid_password")
                ? ("mock_user_id", username)
                : (null, null);
        }

        // Simulate token generation
        private async Task<string> SimulateTokenGenerationAsync(string username)
        {
            // Simulate some async work, e.g., generating a JWT
            await Task.Delay(10); // Simulate delay for token generation

            // Generate a mock token
            return "mock_jwt_token_for_" + username;
        }
    }
}
