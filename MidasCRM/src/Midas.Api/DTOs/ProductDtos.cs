using Midas.Api.DTOs;
using Midas.Core.Products;

namespace Api.Dtos
{
   
    public record ProductDto(
        int Id,
        int WarehouseId,
        string Name,
        string Description,
        int ProductCategoryId,
        DateTime CreatedAt,
        bool IsDeleted,
        IEnumerable<ProductImageDto> Images
    )
    {
        public static ProductDto FromDomain(Product product)
            => new(
                product.Id,
                product.WarehouseId,
                product.Name,
                product.Description,
                product.ProductCategoryId,
                product.CreatedAt,
                product.IsDeleted,
                product.Images.Select(img => new ProductImageDto(
                    img.Id,
                    img.Url,
                    img.IsMain,
                    img.ProductId))
            );
    }

    public record CreateProductDto(
        int WarehouseId,
        string Name,
        string Description,
        int ProductCategoryId
    );

    public record UpdateProductDto(
        string Name,
        string Description
    );

    public record UpdateProductCategoryDto(
        int ProductCategoryId
    );

    public record ChangeWarehouseDto(
        int NewWarehouseId
    );

    public record DeleteProductDto(
        bool IsDeleted
    );
}
