using Midas.Core.Warehouses;

namespace Api.Dtos
{
    public record WarehouseDto(
        int Id,
        string Name,
        Guid CompanyId,
        IReadOnlyCollection<ProductDto> Products
    )
    {
        public static WarehouseDto FromDomain(Warehouse warehouse)
            => new(
                warehouse.Id,
                warehouse.Name,
                warehouse.CompanyId,
                warehouse.Products.Select(ProductDto.FromDomain).ToList()
            );
    }

    public record CreateWarehouseDto(
        string Name
    );

    public record UpdateWarehouseDto(
        string Name
    );

    public record AddProductToWarehouseDto(
        int WarehouseId,
        int ProductId
    );

    public record RemoveProductFromWarehouseDto(
        int WarehouseId,
        int ProductId
    );
}

