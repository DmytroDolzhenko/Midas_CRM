using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Contacts;

namespace Midas.Application.Entities.Contacts.Commands
{
    public class DeleteContactCommand : ICommand<Contact>
    {
        public required int Id { get; init; }
    }

    public class DeleteContactCommandHandler(
        IGetQueries<Contact, int> queries,
        IEntityRepository<Contact> repository)
        : IRequestHandler<DeleteContactCommand, Contact>
    {
        public async Task<Contact> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (contact == null)
            {
                throw new Exception($"Contact with id {request.Id} not found.");
            }

            contact.Delete();
            await repository.DeleteAsync(contact, cancellationToken);
            return contact;
        }
    }
}
