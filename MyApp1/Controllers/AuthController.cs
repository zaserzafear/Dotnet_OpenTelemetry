using Contract.Auth;
using Microsoft.AspNetCore.Mvc;
using MyApp1.Services;
using Services.Publishers.AuthService;
using Services.Services.AuthService;
using System.Text;
using System.Text.Json;

namespace MyApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _service;
        private readonly MyApp2Client _client;
        private readonly IAuthServicePublisher _publisher;

        public AuthController(ILogger<AuthController> logger,
            IAuthService service,
            MyApp2Client client,
            IAuthServicePublisher publisher)
        {
            _logger = logger;
            _service = service;
            _client = client;
            _publisher = publisher;
        }

        [HttpPost("AuthMonolith")]
        public async Task<IActionResult> AuthMonolithAsync([FromBody] AuthRequest request)
        {
            var result = await _service.AuthenticateAsync(request);

            return Ok(result);
        }

        [HttpPost("AuthMicroServicesByHttp")]
        public async Task<IActionResult> AuthMicroServicesByHttp([FromBody] AuthRequest request)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var jsonResponse = await _client.CallServiceAsync(HttpMethod.Post, "/api/Auth/Authentication", jsonContent);

            try
            {
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);
                return Ok(authResponse);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deserializing the response.");
            }
        }

        [HttpPost("AuthMicroServicesByRabbitMQ")]
        public async Task<IActionResult> AuthMicroServicesByRabbitMQ([FromBody] AuthRequest request)
        {
            var result = await _publisher.AuthenticateAsync(request);
            return Ok(result);
        }
    }
}
