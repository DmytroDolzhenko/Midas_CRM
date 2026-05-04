using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.CustomerAddresses.Commands;
using Midas.Core.CustomerAddresses;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAddressController(ISender sender, IGetQueries<CustomerAddress, int> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerAddressDto>>> GetCustomerAddresses(CancellationToken cancellationToken)
        {
            var items = await getQueries.GetAllAsync(cancellationToken);
            return Ok(items.Select(CustomerAddressDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerAddressDto>> GetCustomerAddressById(int id, CancellationToken cancellationToken)
        {
            var item = await getQueries.GetByIdAsync(id, cancellationToken);
            if (item is null)
            {
                return NotFound();
            }

            return Ok(CustomerAddressDto.FromDomain(item));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerAddressDto>> CreateCustomerAddress([FromBody] CreateCustomerAddressDto request, CancellationToken cancellationToken)
        {
            var command = new CreateCustomerAddressCommand
            {
                CustomerId = request.CustomerId,
                City = request.City,
                PostalCode = request.PostalCode,
                PostDepartmentNumber = request.PostDepartmentNumber
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(CustomerAddressDto.FromDomain(result));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerAddressDto>> UpdateCustomerAddress(int id, [FromBody] UpdateCustomerAddressDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateCustomerAddressCommand
            {
                Id = id,
                CustomerId = request.CustomerId,
                City = request.City,
                PostalCode = request.PostalCode,
                PostDepartmentNumber = request.PostDepartmentNumber
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(CustomerAddressDto.FromDomain(result));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CustomerAddressDto>> DeleteCustomerAddress(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteCustomerAddressCommand { Id = id };
            var result = await sender.Send(command, cancellationToken);
            return Ok(CustomerAddressDto.FromDomain(result));
        }
    }
}
