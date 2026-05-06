using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.ProductVariants.Commands;
using Midas.Core.ProductVariants;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductVariantController(ISender sender, IGetQueries<ProductVariant, int> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductVariantDto>>> GetProductVariants(CancellationToken cancellationToken)
        {
            var variants = await getQueries.GetAllAsync(cancellationToken);
            return Ok(variants.Select(ProductVariantDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductVariantDto>> GetProductVariantById(int id, CancellationToken cancellationToken)
        {
            var variant = await getQueries.GetByIdAsync(id, cancellationToken);
            if (variant is null)
            {
                return NotFound();
            }

            return Ok(ProductVariantDto.FromDomain(variant));
        }

        [HttpPost]
        public async Task<ActionResult<ProductVariantDto>> CreateProductVariant([FromBody] CreateProductVariantDto request, CancellationToken cancellationToken)
        {
            var command = new CreateProductVariantCommand
            {
                ProductId = request.ProductId,
                UniqCode = request.UniqCode,
                Color = request.Color,
                Size = request.Size,
                CostPrice = request.CostPrice,
                SellPrice = request.SellPrice
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(ProductVariantDto.FromDomain(result));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductVariantDto>> UpdateProductVariant(int id, [FromBody] UpdateProductVariantDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateProductVariantCommand
            {
                Id = id,
                ProductId = request.ProductId,
                UniqCode = request.UniqCode,
                Color = request.Color,
                Size = request.Size,
                CostPrice = request.CostPrice,
                SellPrice = request.SellPrice
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(ProductVariantDto.FromDomain(result));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductVariantDto>> DeleteProductVariant(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteProductVariantCommand { Id = id }, cancellationToken);
            return Ok(ProductVariantDto.FromDomain(result));
        }
    }
}
