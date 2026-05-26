using Midas.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Midas.Core.Orders;
using Midas.Core.Products;
using Infrastructure.Persistence;

namespace Midas.Infrastructure.Persistence.Services
{
    public class UniqCodeGenerator(ApplicationDbContext dbContext) : IUniqCodeGenerator
    {
        public async Task<string> GenerateProductVariantCodeAsync(
            Product product,
            string size,
            string color,
            CancellationToken cancellationToken)
        {
            var categoryPart = GetFirstThree(product.ProductCategories.FirstOrDefault()?.Category?.Name);
            var namePart = GetFirstThree(product.Name);
            var sizePart = (size ?? string.Empty).Trim().ToUpperInvariant();
            var colorPart = GetFirstThree(color);

            var baseCode = $"{categoryPart}-{namePart}-{sizePart}-{colorPart}";
            var code = baseCode;
            var suffix = 1;

            while (await dbContext.ProductVariants.AnyAsync(x => x.UniqCode == code, cancellationToken))
            {
                suffix++;
                code = $"{baseCode}-{suffix:D2}";
            }

            return code;
        }

        public async Task<string> GenerateOrderCodeAsync(
            Guid companyId,
            DateTime createdAtUtc,
            CancellationToken cancellationToken)
        {
            var dayStart = createdAtUtc.Date;
            var dayEnd = dayStart.AddDays(1);
            var datePart = createdAtUtc.ToString("yyyyMMdd");

            var countForDay = await dbContext.Orders.CountAsync(
                x => x.CompanyId == companyId && x.CreatedAt >= dayStart && x.CreatedAt < dayEnd,
                cancellationToken);

            var nextNumber = countForDay + 1;
            var code = $"{datePart}-{nextNumber:D4}";

            while (await dbContext.Orders.AnyAsync(x => x.UniqCode == code, cancellationToken))
            {
                nextNumber++;
                code = $"{datePart}-{nextNumber:D4}";
            }

            return code;
        }

        private static string GetFirstThree(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "XXX";
            }

            var cleaned = new string(value
                .Trim()
                .Where(char.IsLetterOrDigit)
                .ToArray())
                .ToUpperInvariant();

            if (cleaned.Length == 0)
            {
                return "XXX";
            }

            return cleaned.Length >= 3 ? cleaned[..3] : cleaned.PadRight(3, 'X');
        }
    }
}

