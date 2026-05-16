namespace Midas.Api.DTOs
{
    public record UserIntegrationDto(
        int Id,
        string Provider,
        bool IsActive,
        DateTime? ExpiresAt
    );
}
