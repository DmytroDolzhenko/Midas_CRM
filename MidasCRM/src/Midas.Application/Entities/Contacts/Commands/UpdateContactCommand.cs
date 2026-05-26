using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Contacts;

namespace Midas.Application.Entities.Contacts.Commands
{
    public class UpdateContactCommand : ICommand<Contact>
    {
        public required int Id { get; init; }
        public required string PhoneNumber { get; init; }
    }

    public class UpdateContactCommandHandler(
        IGetQueries<Contact, int> queries,
        IEntityRepository<Contact> repository)
        : IRequestHandler<UpdateContactCommand, Contact>
    {
        public async Task<Contact> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (contact == null)
            {
                throw new Exception($"Contact with id {request.Id} not found.");
            }

            contact.Update(request.PhoneNumber);
            await repository.UpdateAsync(contact, cancellationToken);
            return contact;
        }
    }
}
