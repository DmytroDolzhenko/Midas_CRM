namespace Midas.Api.DTOs
{
    public record RegisterRequest(string Email, string Password, string Name, string Surname, string Fathername);
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string Token, string Email);
}
