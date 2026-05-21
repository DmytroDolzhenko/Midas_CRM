using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.ProductCategories;
using Midas.Core.UserProductCategories;

namespace Midas.Application.Entities.ProductCategories.Commands
{
    public class CreateProductCategoryCommand : ICommand<ProductCategory>
    {
        public required string Name { get; init; }
    }

    public class CreateProductCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateProductCategoryCommand, ProductCategory>
    {
        public async Task<ProductCategory> Handle(CreateProductCategoryCommand request, CancellationToken ct)
        {
            var userId = currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Користувач не авторизований");

            var normalizedName = request.Name.Trim().ToUpperInvariant();

            var existingCategory = await context.ProductCategories
                .FirstOrDefaultAsync(c => c.Name.ToUpper() == normalizedName, ct);

            if (existingCategory != null)
            {
                if (existingCategory.IsPublic)
                    throw new InvalidOperationException("Ця категорія є базовою і вже доступна.");

                var alreadyLinked = await context.UserProductCategories
                    .AnyAsync(uc => uc.UserId == userId && uc.ProductCategoryId == existingCategory.Id, ct);

                if (alreadyLinked)
                    throw new InvalidOperationException("Ви вже маєте цю категорію.");

                context.UserProductCategories.Add(new UserProductCategory
                {
                    UserId = userId,
                    ProductCategoryId = existingCategory.Id
                });

                await context.SaveChangesAsync(ct);
                return existingCategory;
            }

            var newCategory = ProductCategory.Create(0, request.Name, isPublic: false);
            context.ProductCategories.Add(newCategory);

            context.UserProductCategories.Add(new UserProductCategory
            {
                UserId = userId,
                ProductCategory = newCategory
            });

            await context.SaveChangesAsync(ct);
            return newCategory;
        }
    }
}
