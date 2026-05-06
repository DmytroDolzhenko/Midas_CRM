using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Products.Commands;
using Midas.Core.Products;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(ISender sender, IGetQueries<Product, int> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts(CancellationToken cancellationToken)
        {
            var products = await getQueries.GetAllAsync(cancellationToken);
            return Ok(products.Select(ProductDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id, CancellationToken cancellationToken)
        {
            var product = await getQueries.GetByIdAsync(id, cancellationToken);
            if (product is null)
            {
                return NotFound();
            }

            return Ok(ProductDto.FromDomain(product));
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto request, CancellationToken cancellationToken)
        {
            var command = new CreateProductCommand
            {
                WarehouseId = request.WarehouseId,
                Name = request.Name,
                Description = request.Description,
                ProductCategoryId = request.ProductCategoryId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(ProductDto.FromDomain(result));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateProductCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(ProductDto.FromDomain(result));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductDto>> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteProductCommand { Id = id }, cancellationToken);
            return Ok(ProductDto.FromDomain(result));
        }

        [HttpPatch("{id:int}/category")]
        public async Task<ActionResult<ProductDto>> UpdateProductCategory(int id, [FromBody] UpdateProductCategoryDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateProductCategoryCommand
            {
                Id = id,
                ProductCategoryId = request.ProductCategoryId
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(ProductDto.FromDomain(result));
        }

        [HttpPatch("{id:int}/warehouse")]
        public async Task<ActionResult<ProductDto>> ChangeWarehouse(int id, [FromBody] ChangeWarehouseDto request, CancellationToken cancellationToken)
        {
            var command = new ChangeWarehouseCommand
            {
                ProductId = id,
                NewWarehouseId = request.NewWarehouseId
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(ProductDto.FromDomain(result));
        }
    }
}
