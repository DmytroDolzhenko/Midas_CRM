using Midas.Core.ProductVariants;

namespace Api.Dtos
{
    public record ProductVariantDto(
        int Id,
        int ProductId,
        string UniqCode,
        string Color,
        string Size,
        int StockQuantity,
        decimal CostPrice,
        decimal SellPrice,
        bool IsDeleted
    )
    {
        public static ProductVariantDto FromDomain(ProductVariant productVariant)
            => new(
                productVariant.Id,
                productVariant.ProductId,
                productVariant.UniqCode,
                productVariant.Color,
                productVariant.Size,
                productVariant.StockQuantity,
                productVariant.CostPrice,
                productVariant.SellPrice,
                productVariant.IsDeleted
            );
    }

    public class CreateProductVariantDto
    {
        public int ProductId { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
    }
    public record ProductVariantRequestItem(
        string Color,
        string Size,
        int Quantity,
        decimal CostPrice,
        decimal SellPrice
    );

    public record UpdateProductVariantDto(
        int ProductId,
        string UniqCode,
        string Color,
        string Size,
        decimal CostPrice,
        decimal SellPrice
    );

    public record DeleteProductVariantDto(
        bool IsDeleted
    );
    public record UpdateProductVariantQuantity(
        int Quantity
    );
}
