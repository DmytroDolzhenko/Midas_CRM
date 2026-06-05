namespace Midas.Application.Common.Interfaces.Repositories
{
    public interface IAiAssistantService
    {
        Task<string> GetRecommendationAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken = default);

        Task<string> GenerateDescription(string type, string name, string category, List<string>? items = null);
    }
}
