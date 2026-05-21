using Midas.Core.Products;

namespace Midas.Application.Common.Interfaces
{
    public interface IUniqCodeGenerator
    {
        Task<string> GenerateProductVariantCodeAsync(
            Product product,
            string size,
            string color,
            CancellationToken cancellationToken);

        Task<string> GenerateOrderCodeAsync(
            Guid companyId,
            DateTime createdAtUtc,
            CancellationToken cancellationToken);
    }
}

