using Midas.Core.ProductCategories;

namespace Api.Dtos
{
    public record ProductCategoryDto(
        int Id,
        string Name,
        bool IsDeleted
    )
    {
        public static ProductCategoryDto FromDomain(ProductCategory productCategory)
            => new(
                productCategory.Id,
                productCategory.Name,
                productCategory.IsDeleted
            );
    }

    public record CreateProductCategoryDto(
        string Name
    );

    public record UpdateProductCategoryNameDto(
        string Name
    );

    public record DeleteProductCategoryDto(
        bool IsDeleted
    );
}
