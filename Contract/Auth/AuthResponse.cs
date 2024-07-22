namespace Contract.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = "mock_token"; // Example token
        public string UserId { get; set; } = "mock_user_id"; // Example user ID
        public string Username { get; set; } = "mock_username"; // Example username
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(1); // Token expiration
    }
}
