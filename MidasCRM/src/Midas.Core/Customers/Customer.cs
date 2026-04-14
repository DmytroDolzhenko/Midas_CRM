using Midas.Core.Contacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Customers
{
    public class Customer
    {
        public int Id { get; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public Contact Contact { get; private set; }
        public int Email { get; private set; }
        public bool IsDeleted { get; private set; }

        private Customer(
            int id,
            string name,
            string surname,
            Contact contact,
            int email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Contact = contact;
            Email = email;
        }

        public static Customer Create(
            string name,
            string surname,
            Contact contact,
            int email)
        {
            return new Customer(
                0,
                name,
                surname,
                contact,
                email);
        }
        public void Update(
            string name,
            string surname,
            Contact contact,
            int email)
        {
            Name = name;
            Surname = surname;
            Contact = contact;
            Email = email;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
    }
}
