using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Contacts.Commands;
using Midas.Core.Contacts;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController(
        ISender sender,
        ICurrentUserService currentUserService,
        IGetQueries<Contact, int> getQueries
        ) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ContactDto>>> GetContacts(CancellationToken cancellationToken)
        {
            var contacts = await getQueries.GetAllAsync(cancellationToken);
            return Ok(contacts.Select(ContactDto.FromDomain));
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContactDto>> GetContactById(int id, CancellationToken cancellationToken)
        {
            var contact = await getQueries.GetByIdAsync(id, cancellationToken);

            if (contact is null)
            {
                return NotFound();
            }

            return Ok(ContactDto.FromDomain(contact));
        }

        [HttpPost]
        public async Task<ActionResult<ContactDto>> CreateContact
            ([FromBody] CreateContactDto request,
            CancellationToken cancellationToken)
        {
            var input = new CreateContactCommand
            {
                Value = request.Value
            };

            var result = await sender.Send(input, cancellationToken);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> MarkAsDelete(int id, CancellationToken cancellationToken)
        {
            var contact = await getQueries.GetByIdAsync(id, cancellationToken);

            if (contact is null)
            {
                return NotFound();
            }

            contact.Delete();

            var input = new DeleteContactCommand
            {
                Id = contact.Id
            };

            var result = await sender.Send(input, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateContact(int id, CancellationToken cancellationToken)
        {
            var contact = await getQueries.GetByIdAsync(id, cancellationToken);

            if (contact is null)
            {
                return NotFound();
            }

            var input = new UpdateContactCommand
            {
                Id = contact.Id,
                Value = contact.Value
            };

            var result = await sender.Send(input, cancellationToken);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
