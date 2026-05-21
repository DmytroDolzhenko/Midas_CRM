using Midas.Api.DTOs;
using Midas.Core.Products;

namespace Api.Dtos
{

    public record ProductDto(
        int Id,
        int WarehouseId,
        string Name,
        string Description,
        decimal Weight,
        IEnumerable<int> CategoryIds,
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
                product.Weight,
                product.ProductCategories.Select(pc => pc.CategoryId),
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
        decimal Weight,
        List<int> ProductCategoryIds
    );

    public record UpdateProductDto(
        string Name,
        string Description,
        decimal Weight
    );

    public record UpdateProductCategoryDto(
        int ProductCategoryId,
        int NewProductCategoryId
    );

    public record ChangeWarehouseDto(
        int NewWarehouseId
    );

    public record DeleteProductDto(
        bool IsDeleted
    );

    public class CreateProductWithVariantDto
    {
        public int WarehouseId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Weight { get; set; }
        public List<int> ProductCategoryIds { get; set; } = new();
        public List<CreateProductVariantDto> Variants { get; set; } = new();
    }
}
