using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Midas.Core.Enums;

namespace Midas.Core.Users
{
    public class User : IdentityUser<Guid>, IEntity<Guid>
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Fathername { get; private set; }
       //public string PhoneNumber { get; private set; }

        public UserRole Role { get; private set; }
        public DateTime? RegistrationDate { get; private set; }

        public bool IsApproved { get; private set; }
        public bool IsDeleted { get; private set; }
        //NOVA POSHTA


        private User(
            string name,
            string surname,
            string fathername,
            string email,
            string phoneNumber,
            UserRole role,
            bool isApproved)
        {
            Id = Guid.NewGuid();
            Name = name;
            Surname = surname;
            Fathername = fathername;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;
            RegistrationDate = DateTime.UtcNow;
            IsApproved = isApproved;
        }

        public static User Create(
            string name,
            string surname,
            string fathername,
            string email,
            string phoneNumber,
            UserRole role,
            bool isApproved)
        {
            return new User(
                name,
                surname,
                fathername,
                email,
                phoneNumber,
                role,
                isApproved);
        }

        public void Update(
            string name,
            string surname,
            string fathername,
            string email,
            string phoneNumber
            )
        {
            Name = name;
            Surname = surname;
            Fathername = fathername;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
        }
        public void MarkAsDeleted()
        {
            IsDeleted = true;

            var suffix = Guid.NewGuid().ToString().Substring(0, 8);
            Email = $"deleted_{suffix}_{Email}";
            NormalizedEmail = Email.ToUpper();
            UserName = Email;
            NormalizedUserName = Email.ToUpper();

            PasswordHash = null;
        }
        public void ChangePhoneNumber(string newPhoneNumber)
        {
            PhoneNumber = newPhoneNumber;
        }
    }
}
