using Midas.Core.ProductImages;

namespace Midas.Api.DTOs
{
    public record ProductImageDto(
        int Id,
        string Url,
        bool IsMain,
        int ProductId
    )
    {
        public static ProductImageDto FromDomainModel(ProductImage image)
            => new(
                image.Id,
                image.Url,
                image.IsMain,
                image.ProductId
            );
    }
}
