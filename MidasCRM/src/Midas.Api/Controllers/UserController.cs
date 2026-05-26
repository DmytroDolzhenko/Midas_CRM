using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Users.Commands;
using Midas.Core.Users;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(ISender sender, IGetQueries<User, Guid> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> GetUsers(CancellationToken cancellationToken)
        {
            var users = await getQueries.GetAllAsync(cancellationToken);
            return Ok(users.Select(UserDto.FromDomain));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            var user = await getQueries.GetByIdAsync(id, cancellationToken);
            if (user is null)
            {
                return NotFound();
            }

            return Ok(UserDto.FromDomain(user));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto request, CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand
            {
                Name = request.Name,
                Surname = request.Surname,
                Fathername = request.Fathername,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                IsApproved = request.IsApproved
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(UserDto.FromDomain(result));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateUserCommand
            {
                Id = id,
                Name = request.Name,
                Surname = request.Surname,
                Fathername = request.Fathername,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(UserDto.FromDomain(result));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<UserDto>> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteUserCommand { Id = id }, cancellationToken);
            return Ok(UserDto.FromDomain(result));
        }

        [HttpPatch("{id:guid}/role")]
        public async Task<ActionResult<UserDto>> ChangeUserRole(Guid id, [FromBody] ChangeUserRoleDto request, CancellationToken cancellationToken)
        {
            var command = new ChangeUserRoleCommand
            {
                Id = id,
                Role = request.NewRole
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(UserDto.FromDomain(result));
        }
    }
}
