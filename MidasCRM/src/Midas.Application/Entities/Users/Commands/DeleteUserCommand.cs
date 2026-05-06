using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Users;

namespace Midas.Application.Entities.Users.Commands
{
    public class DeleteUserCommand : ICommand<User>
    {
        public required Guid Id { get; init; }
    }

    public class DeleteUserCommandHandler(
        IGetQueries<User, Guid> queries,
        IEntityRepository<User> repository)
        : IRequestHandler<DeleteUserCommand, User>
    {
        public async Task<User> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new Exception($"User with id {request.Id} not found.");
            }

            user.MarkAsDeleted();
            await repository.UpdateAsync(user, cancellationToken);
            return user;
        }
    }
}
