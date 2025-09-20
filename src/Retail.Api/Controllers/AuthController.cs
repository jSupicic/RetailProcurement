using Microsoft.AspNetCore.Mvc;
using Retail.Api.Users;
using Retail.Application.DTOs;

namespace Retail.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TokenProvider _tokenProvider;

        public AuthController(IConfiguration configuration, TokenProvider tokenProvider)
        {
            _configuration = configuration;
            _tokenProvider = tokenProvider;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            if (dto.Username == "RetailAdmin" && dto.Password == "password")
            {
                string token = _tokenProvider.Create(dto);
                var response = new LoginResponseDto { Token = token, Username = dto.Username };
                return Ok(response);
            }

            return Unauthorized();
        }
    }
}
