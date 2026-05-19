using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.Users;

namespace Midas.Application.Entities.Users.Commands
{
    public class CreateUserCommand : ICommand<User>
    {
        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required string Fathername { get; init; }
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }
        public required UserRole Role { get; init; }
        public required bool IsApproved { get; init; }
    }

    public class CreateUserCommandHandler(IEntityRepository<User> repository)
        : IRequestHandler<CreateUserCommand, User>
    {
        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = User.Create(
                request.Name,
                request.Surname,
                request.Fathername,
                request.Email,
                request.PhoneNumber,
                request.Role,
                request.IsApproved);

            await repository.AddAsync(user, cancellationToken);
            return user;
        }
    }
}
