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
        string PhoneNumber,
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
                user.PhoneNumber,
                user.IsApproved,
                user.RegistrationDate
            );
    }
    public record CreateUserDto(
        string Name,
        string Surname,
        string Fathername,
        string Email,
        string PhoneNumber,
        UserRole Role,
        bool IsApproved
    );
    public record UpdateUserDto(
        string Name,
        string Surname,
        string Fathername,
        string Email,
        string PhoneNumber
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
