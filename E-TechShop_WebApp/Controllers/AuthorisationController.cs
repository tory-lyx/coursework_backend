using Microsoft.AspNetCore.Mvc;
using OnlineTechShop_WebApp.Models;
using OnlineTechShop_WebApp.Services;

namespace OnlineTechShop_WebApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthorisationController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthorisationController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthorisationModel model)
        {
            await _authService.RegisterAsync(model);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthorisationModel model)
        {
            var token = await _authService.LoginAsync(model);
            return Ok(new { token });
        }
    }
}
