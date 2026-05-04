using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Contacts;
using Midas.Application.Common.Messaging;
using System.Windows.Input;

namespace Midas.Application.Entities.Contacts.Commands
{
    public class CreateContactCommand : ICommand<Contact>
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
