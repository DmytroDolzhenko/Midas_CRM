namespace Midas.Api.DTOs
{
    public record ConnectExternalServiceRequest(string Provider, string Code);
    public record SaveStaticTokenRequest(string Provider, string Token);
}
