using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Contacts;
using Midas.Application.Common.Messaging;
using System.Windows.Input;

namespace Midas.Application.Entities.Contacts.Commands
{
    public class CreateContactCommand : ICommand<Contact>
    {
        public required string PhoneNumber { get; init; }
    }

    public class CreateContactCommandHandler(
        IEntityRepository<Contact> repository,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateContactCommand, Contact>
    {
        public async Task<Contact> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var contact = Contact.Create(request.PhoneNumber, currentUserId);
            await repository.AddAsync(contact, cancellationToken);
            return contact;
        }
    }
}
