using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Payments.Commands;
using Midas.Core.Payments;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController(ISender sender, IGetQueries<Payment, Guid> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetPayments(CancellationToken cancellationToken)
        {
            var payments = await getQueries.GetAllAsync(cancellationToken);
            return Ok(payments.Select(PaymentDto.FromDomain));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PaymentDto>> GetPaymentById(Guid id, CancellationToken cancellationToken)
        {
            var payment = await getQueries.GetByIdAsync(id, cancellationToken);
            if (payment is null)
            {
                return NotFound();
            }

            return Ok(PaymentDto.FromDomain(payment));
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto request, CancellationToken cancellationToken)
        {
            var command = new CreatePaymentCommand
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Method = request.Method
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(PaymentDto.FromDomain(result));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<PaymentDto>> DeletePayment(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeletePaymentCommand { Id = id }, cancellationToken);
            return Ok(PaymentDto.FromDomain(result));
        }
    }
}
