using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.FinancialOperations.Commands;
using Midas.Core.FinancialOperations;

namespace Midas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialOperationController(ISender sender, IGetQueries<FinancialOperation, Guid> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<FinancialOperationDto>>> GetAll(CancellationToken cancellationToken)
        {
            var operations = await getQueries.GetAllAsync(cancellationToken);
            return Ok(operations.Select(FinancialOperationDto.FromDomain));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FinancialOperationDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var operation = await getQueries.GetByIdAsync(id, cancellationToken);
            if (operation is null)
            {
                return NotFound();
            }

            return Ok(FinancialOperationDto.FromDomain(operation));
        }

        [HttpPost]
        public async Task<ActionResult<FinancialOperationDto>> Create([FromBody] CreateFinancialOperationDto request, CancellationToken cancellationToken)
        {
            var command = new CreateFinancialOperationCommand
            {
                OperationType = request.OperationType,
                Category = request.Category,
                Amount = request.Amount,
                Comment = request.Comment,
                OrderId = request.OrderId
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(FinancialOperationDto.FromDomain(result));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FinancialOperationDto>> Update(Guid id, [FromBody] UpdateFinancialOperationDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateFinancialOperationCommand
            {
                Id = id,
                OperationType = request.OperationType,
                Category = request.Category,
                Amount = request.Amount,
                Comment = request.Comment,
                OrderId = request.OrderId
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(FinancialOperationDto.FromDomain(result));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FinancialOperationDto>> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteFinancialOperationCommand { Id = id }, cancellationToken);
            return Ok(FinancialOperationDto.FromDomain(result));
        }
    }
}
