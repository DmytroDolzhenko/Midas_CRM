using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Users;

namespace Midas.Application.Entities.Users.Commands
{
    public class UpdateUserCommand : ICommand<User>
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required string Fathername { get; init; }
        public required string Email { get; init; }
    }

    public class UpdateUserCommandHandler(
        IGetQueries<User, Guid> queries,
        IEntityRepository<User> repository)
        : IRequestHandler<UpdateUserCommand, User>
    {
        public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new Exception($"User with id {request.Id} not found.");
            }

            user.Update(
                request.Name,
                request.Surname,
                request.Fathername,
                request.Email);

            await repository.UpdateAsync(user, cancellationToken);
            return user;
        }
    }
}
