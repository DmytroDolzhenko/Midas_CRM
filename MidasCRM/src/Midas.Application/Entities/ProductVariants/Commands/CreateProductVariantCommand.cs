using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Products;
using Midas.Core.ProductVariants;

namespace Midas.Application.Entities.ProductVariants.Commands
{
    public class CreateProductVariantCommand : ICommand<ProductVariant>
    {
        public required int ProductId { get; init; }
        public required string Color { get; init; }
        public required string Size { get; init; }
        public required int Quantity { get; init; }
        public required decimal CostPrice { get; init; }
        public required decimal SellPrice { get; init; }
    }

    public class CreateProductVariantCommandHandler(
        IEntityRepository<ProductVariant> repository,
        IGetQueries<Product, int> productQueries,
        IUniqCodeGenerator uniqCodeGenerator,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateProductVariantCommand, ProductVariant>
    {
        public async Task<ProductVariant> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken) ?? throw new UnauthorizedAccessException();
            var product = await productQueries.GetByIdAsync(
                request.ProductId,
                cancellationToken,
                query => query.Include(x => x.ProductCategories));

            if (product is null)
            {
                throw new Exception($"Product with ID {request.ProductId} not found");
            }

            var uniqCode = await uniqCodeGenerator.GenerateProductVariantCodeAsync(
                product,
                request.Size,
                request.Color,
                cancellationToken);

            var productVariant = ProductVariant.Create(
                request.ProductId,
                request.Color,
                request.Size,
                request.Quantity,
                request.CostPrice,
                request.SellPrice,
                Core.Enums.ProductVariantStatus.Available,
                uniqCode,
                companyId);

            await repository.AddAsync(productVariant, cancellationToken);
            return productVariant;
        }
    }
}

