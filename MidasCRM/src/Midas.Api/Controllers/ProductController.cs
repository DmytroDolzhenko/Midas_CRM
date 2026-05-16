using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Midas.Api.DTOs;
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
            var products = await getQueries.GetAllAsync(cancellationToken,
                query => query.Include(p => p.Images));
            return Ok(products.Select(ProductDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id, CancellationToken cancellationToken)
        {
            var product = await getQueries.GetByIdAsync(id, cancellationToken,
                query => query.Include(p => p.Images));
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
        [HttpPost("{id:int}/images")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductImageDto>> AddProductImage(int id, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не вибрано або він порожній.");
            }

            var command = new AddImageToProductCommand
            {
                ProductId = id,
                Image = file
            };

            var result = await sender.Send(command, cancellationToken);

            return Ok(ProductImageDto.FromDomainModel(result));
        }

        [HttpGet("{id:int}/image-file")]
        [Produces("image/jpeg", "image/png")]
        public async Task<IActionResult> GetProductImageFile(int id, CancellationToken ct)
        {
            var product = await getQueries.GetByIdAsync(id, ct, q => q.Include(p => p.Images));
            var mainImage = product?.Images.FirstOrDefault(i => i.IsMain) ?? product?.Images.FirstOrDefault();

            if (mainImage == null) return NotFound();

            using var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(mainImage.Url, ct);

            return File(imageBytes, "image/jpeg");
        }
    }
}
