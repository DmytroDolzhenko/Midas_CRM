using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Contacts;

namespace Midas.Application.Entities.Contacts.Commands
{
    public class CreateContactCommand : IRequest<Contact>
    {
        public required string Value { get; init; }
    }

    public class CreateContactCommandHandler(IEntityRepository<Contact> repository)
        : IRequestHandler<CreateContactCommand, Contact>
    {
        public async Task<Contact> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = Contact.Create(request.Value);
            await repository.AddAsync(contact, cancellationToken);
            return contact;
        }
    }
}
