using System;
using Midas.Core.Enums;
using Midas.Core.Users;

namespace Api.Dtos
{
    public record UserDto(
        Guid Id,
        string Name,
        string Surname,
        string Fathername,
        UserRole Role,
        string Email,
        bool IsApproved,
        DateTime? RegistrationDate
    )
    {
        public static UserDto FromDomain(User user)
            => new(
                user.Id,
                user.Name,
                user.Surname,
                user.Fathername,
                user.Role,
                user.Email ?? string.Empty,
                user.IsApproved,
                user.RegistrationDate
            );
    }
    public record CreateUserDto(
        string Name,
        string Surname,
        string Fathername,
        string Email,
        UserRole Role,
        bool IsApproved
    );
    public record UpdateUserDto(
        string Name,
        string Surname,
        string Fathername,
        string Email
    );
    public record ApproveUserDto(
        bool IsApproved
    );
    public record DeleteUserDto(
        bool IsDeleted
    );
    public record ChangeUserRoleDto(
        UserRole NewRole
    );

}
