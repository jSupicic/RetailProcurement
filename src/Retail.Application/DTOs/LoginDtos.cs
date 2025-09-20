namespace Retail.Application.DTOs
{
    public class LoginRequestDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public required string Token { get; set; }
        public required string Username { get; set; }
    }
}
