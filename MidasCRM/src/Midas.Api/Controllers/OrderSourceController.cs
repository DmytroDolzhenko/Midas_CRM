using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.OrderSources.Commands;
using Midas.Core.OrderSources;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderSourceController(ISender sender, IGetQueries<OrderSource, int> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderSourceDto>>> GetOrderSources(CancellationToken cancellationToken)
        {
            var sources = await getQueries.GetAllAsync(cancellationToken);
            return Ok(sources.Select(OrderSourceDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderSourceDto>> GetOrderSourceById(int id, CancellationToken cancellationToken)
        {
            var source = await getQueries.GetByIdAsync(id, cancellationToken);
            if (source is null)
            {
                return NotFound();
            }

            return Ok(OrderSourceDto.FromDomain(source));
        }

        [HttpPost]
        public async Task<ActionResult<OrderSourceDto>> CreateOrderSource([FromBody] CreateOrderSourceDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateOrderSourceCommand { Name = request.Name }, cancellationToken);
            return Ok(OrderSourceDto.FromDomain(result));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<OrderSourceDto>> UpdateOrderSource(int id, [FromBody] UpdateOrderSourceDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateOrderSourceCommand { Id = id, Name = request.Name };
            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderSourceDto.FromDomain(result));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<OrderSourceDto>> DeleteOrderSource(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteOrderSourceCommand { Id = id }, cancellationToken);
            return Ok(OrderSourceDto.FromDomain(result));
        }
    }
}
