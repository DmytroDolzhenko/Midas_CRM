namespace Midas.Application.Common.Interfaces.Repositories
{
    public interface IAiAssistantService
    {
        Task<string> GetRecommendationAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken = default);
    }
}
