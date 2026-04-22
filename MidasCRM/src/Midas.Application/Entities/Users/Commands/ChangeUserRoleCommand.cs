using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Enums;
using Midas.Core.Users;

namespace Midas.Application.Entities.Users.Commands
{
    public class ChangeUserRoleCommand : IRequest<User>
    {
        public required Guid Id { get; init; }
        public required UserRole Role { get; init; }
    }

    public class ChangeUserRoleCommandHandler(
        IGetQueries<User, Guid> queries,
        IEntityRepository<User> repository)
        : IRequestHandler<ChangeUserRoleCommand, User>
    {
        public async Task<User> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new Exception($"User with id {request.Id} not found.");
            }

            user.ChangeRole(request.Role);
            await repository.UpdateAsync(user, cancellationToken);
            return user;
        }
    }
}
