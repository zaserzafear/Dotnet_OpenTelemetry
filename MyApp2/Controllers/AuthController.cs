using Contract.Auth;
using Microsoft.AspNetCore.Mvc;
using Services.Services.AuthService;

namespace MyApp2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("Authentication")]
        public async Task<IActionResult> Auth([FromBody] AuthRequest request)
        {
            var result = await _service.AuthenticateAsync(request);

            return Ok(result);
        }
    }
}
