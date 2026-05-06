using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Customers.Commands;
using Midas.Core.Customers;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController(ISender sender, IGetQueries<Customer, int> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetCustomers(CancellationToken cancellationToken)
        {
            var customers = await getQueries.GetAllAsync(cancellationToken);
            return Ok(customers.Select(CustomerDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(int id, CancellationToken cancellationToken)
        {
            var customer = await getQueries.GetByIdAsync(id, cancellationToken);
            if (customer is null)
            {
                return NotFound();
            }

            return Ok(CustomerDto.FromDomain(customer));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto request, CancellationToken cancellationToken)
        {
            var command = new CreateCustomerCommand
            {
                Name = request.Name,
                Surname = request.Surname,
                ContactValue = request.ContactValue,
                Email = request.Email
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(CustomerDto.FromDomain(result));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateCustomerCommand
            {
                Id = id,
                Name = request.Name,
                Surname = request.Surname,
                ContactValue = request.ContactValue,
                Email = request.Email
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(CustomerDto.FromDomain(result));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CustomerDto>> DeleteCustomer(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteCustomerCommand { Id = id };
            var result = await sender.Send(command, cancellationToken);
            return Ok(CustomerDto.FromDomain(result));
        }
    }
}
