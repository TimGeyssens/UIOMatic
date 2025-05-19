using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UIOMatic.Front.API.Models;
using UIOMatic.Front.API.Services;
using UIOMatic.Front.API.Services.Auth;

namespace UIOMatic.Front.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(ILogger<AuthController> logger, UserService userService, JwtService jwtService)
        {
            _logger = logger;
            _userService = userService;
            _jwtService = jwtService;
            EnsureDefaultAdminExists();
        }

        private void EnsureDefaultAdminExists()
        {
            try
            {
                var adminUser = _userService.GetUserByUsernameOrEmail("admin", "admin@admin.com");
                if (adminUser == null)
                {
                    _userService.CreateUser("admin", "admin", "admin@admin.com");
                    _logger.LogInformation("Default admin user created");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating default admin user");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = _userService.AuthenticateUser(request.Username, request.Password);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = _jwtService.GenerateToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }
    }
} 