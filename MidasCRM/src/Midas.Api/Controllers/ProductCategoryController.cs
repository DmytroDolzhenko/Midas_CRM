using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.ProductCategories.Commands;
using Midas.Core.ProductCategories;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController(ISender sender, IGetQueries<ProductCategory, int> getQueries, IProductCategoryQueries productCategoryQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductCategoryDto>>> GetProductCategories(CancellationToken cancellationToken)
        {
            var categories = await getQueries.GetAllAsync(cancellationToken);
            return Ok(categories.Select(ProductCategoryDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductCategoryDto>> GetProductCategoryById(int id, CancellationToken cancellationToken)
        {
            var category = await getQueries.GetByIdAsync(id, cancellationToken);
            if (category is null)
            {
                return NotFound();
            }

            return Ok(ProductCategoryDto.FromDomain(category));
        }

        [HttpPost]
        public async Task<ActionResult<ProductCategoryDto>> CreateProductCategory([FromBody] CreateProductCategoryDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateProductCategoryCommand { Name = request.Name }, cancellationToken);
            return Ok(ProductCategoryDto.FromDomain(result));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductCategoryDto>> UpdateProductCategoryName(int id, [FromBody] UpdateProductCategoryNameDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateProductCategoryNameCommand { Id = id, Name = request.Name }, cancellationToken);
            return Ok(ProductCategoryDto.FromDomain(result));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductCategoryDto>> DeleteProductCategory(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteProductCategoryCommand { Id = id }, cancellationToken);
            return Ok(ProductCategoryDto.FromDomain(result));
        }
        [HttpGet("available")]
        public async Task<ActionResult<IReadOnlyList<ProductCategoryDto>>> GetAvailableProductCategories(CancellationToken cancellationToken)
        {
            var categories = await productCategoryQueries.GetAvailableCategoryAsync(cancellationToken);
            return Ok(categories.Select(ProductCategoryDto.FromDomain));
        }
    }
}
