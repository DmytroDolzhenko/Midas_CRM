using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.OrderItems;
using Midas.Core.Orders;
using Midas.Core.ProductVariants;
using System.Text;

namespace Midas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ai-agent")]
public class AiAgentController : ControllerBase
{
    private readonly IAiAssistantService _aiService;
    private readonly ApplicationDbContext _context;

    public AiAgentController(IAiAssistantService aiService, ApplicationDbContext context)
    {
        _aiService = aiService;
        _context = context;
    }

    [HttpGet("business-advice")]
    public async Task<IActionResult> GetBusinessAdvice(CancellationToken cancellationToken)
    {
        var salesByVariant = await _context.Set<OrderItem>()
            .AsNoTracking()
            .Where(item => !item.IsDeleted)
            .GroupBy(item => item.ProductVariantId)
            .Select(group => new
            {
                ProductVariantId = group.Key,
                SalesCount = group.Sum(item => item.Quantity),
                Revenue = group.Sum(item => item.Quantity * item.UnitPrice),
                GrossProfit = group.Sum(item => item.Quantity * (item.UnitPrice - item.CostPriceSnapshot))
            })
            .ToDictionaryAsync(item => item.ProductVariantId, cancellationToken);

        var variants = await _context.Set<ProductVariant>()
            .AsNoTracking()
            .Include(variant => variant.Product)
                .ThenInclude(product => product.Warehouse)
            .Where(variant => !variant.IsDeleted && !variant.Product.IsDeleted)
            .ToListAsync(cancellationToken);

        var lowStock = variants
            .Select(variant => new
            {
                Variant = variant,
                Sales = salesByVariant.GetValueOrDefault(variant.Id)
            })
            .Where(item => item.Variant.StockQuantity <= 3 && (item.Sales?.SalesCount ?? 0) > 2)
            .OrderBy(item => item.Variant.StockQuantity)
            .ThenByDescending(item => item.Sales?.GrossProfit ?? 0)
            .Take(20)
            .ToList();

        var deadStock = variants
            .Select(variant => new
            {
                Variant = variant,
                Sales = salesByVariant.GetValueOrDefault(variant.Id)
            })
            .Where(item => item.Variant.StockQuantity > 10 && (item.Sales?.SalesCount ?? 0) == 0)
            .OrderByDescending(item => item.Variant.StockQuantity * item.Variant.CostPrice)
            .Take(20)
            .ToList();

        var topPerformers = variants
            .Select(variant => new
            {
                Variant = variant,
                Sales = salesByVariant.GetValueOrDefault(variant.Id)
            })
            .Where(item => (item.Sales?.SalesCount ?? 0) > 0)
            .OrderByDescending(item => item.Sales?.GrossProfit ?? 0)
            .Take(20)
            .ToList();

        var currentRevenue = await _context.Set<Order>()
            .AsNoTracking()
            .Where(order => !order.IsDeleted)
            .SumAsync(order => order.TotalCost, cancellationToken);

        var currentGrossProfit = salesByVariant.Values.Sum(item => item.GrossProfit);
        var stockValue = variants.Sum(variant => variant.StockQuantity * variant.CostPrice);
        var potentialRevenue = variants.Sum(variant => variant.StockQuantity * variant.SellPrice);

        var report = BuildBusinessReport(
            currentRevenue,
            currentGrossProfit,
            stockValue,
            potentialRevenue,
            lowStock,
            deadStock,
            topPerformers);

        const string systemPrompt =
            "Ти — професійний фінансовий аналітик та AI-агент CRM системи MidasCRM.\n" +
            "Проаналізуй звіт і дай чіткі бізнес-рекомендації українською мовою.\n\n" +
            "Формат відповіді строго за пунктами у Markdown:\n" +
            "### ЩО ЗАКУПИТИ НАЙБЛИЖЧИМ ЧАСОМ\n" +
            "### ЯК ОЧИСТИТИ МЕРТВИЙ СКЛАД\n" +
            "### СТРАТЕГІЧНИЙ ІНВЕСТ-ПЛАН КОШТІВ\n\n" +
            "Пиши практично: що зробити, чому, і який ризик.";

        var advice = await _aiService.GetRecommendationAsync(systemPrompt, report, cancellationToken);

        return Ok(new { advice, report });
    }

    private static string BuildBusinessReport(
        decimal currentRevenue,
        decimal currentGrossProfit,
        decimal stockValue,
        decimal potentialRevenue,
        IEnumerable<dynamic> lowStock,
        IEnumerable<dynamic> deadStock,
        IEnumerable<dynamic> topPerformers)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ФІНАНСОВИЙ ЗВІТ МАГАЗИНУ:");
        sb.AppendLine($"- Оборот за всіма продажами: {currentRevenue:0.##} грн.");
        sb.AppendLine($"- Валовий прибуток за товарами: {currentGrossProfit:0.##} грн.");
        sb.AppendLine($"- Собівартість товарів на складах: {stockValue:0.##} грн.");
        sb.AppendLine($"- Потенційний дохід із поточних залишків: {potentialRevenue:0.##} грн.");
        sb.AppendLine("- Серверної сутності витрат поки немає, тому витрати не враховані.");

        sb.AppendLine();
        sb.AppendLine("ТОВАРИ, ЩО ЗАКІНЧУЮТЬСЯ І МАЮТЬ ПРОДАЖІ:");
        AppendVariantLines(sb, lowStock);

        sb.AppendLine();
        sb.AppendLine("МЕРТВИЙ СКЛАД:");
        AppendVariantLines(sb, deadStock);

        sb.AppendLine();
        sb.AppendLine("НАЙПРИБУТКОВІШІ ТОВАРИ:");
        AppendVariantLines(sb, topPerformers);

        return sb.ToString();
    }

    private static void AppendVariantLines(StringBuilder sb, IEnumerable<dynamic> items)
    {
        var hasItems = false;

        foreach (var item in items)
        {
            hasItems = true;
            ProductVariant variant = item.Variant;
            var sales = item.Sales;
            var salesCount = sales?.SalesCount ?? 0;
            var grossProfit = sales?.GrossProfit ?? 0;
            var margin = variant.CostPrice > 0
                ? Math.Round(((variant.SellPrice - variant.CostPrice) / variant.CostPrice) * 100, 2)
                : 0;

            sb.AppendLine(
                $"* {variant.Product.Name} / {variant.UniqCode} " +
                $"(Склад: {variant.Product.Warehouse.Name}, " +
                $"Залишок: {variant.StockQuantity} од., " +
                $"Продано: {salesCount} од., " +
                $"Собівартість: {variant.CostPrice:0.##} грн., " +
                $"Ціна: {variant.SellPrice:0.##} грн., " +
                $"Націнка: {margin:0.##}%, " +
                $"Валовий прибуток: {grossProfit:0.##} грн.)");
        }

        if (!hasItems)
        {
            sb.AppendLine("* Немає даних за цим блоком.");
        }
    }
}
