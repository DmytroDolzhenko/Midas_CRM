using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Midas.Core.Customers;

namespace Midas.Core.Contacts
{
    public class Contact : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }
        public string PhoneNumber { get; private set; }
        public Guid OwnerId { get; private set; }
        public bool IsDeleted { get; private set; }

        private readonly List<Customer> _customers = new();
        public IReadOnlyCollection<Customer> Customers => _customers.AsReadOnly();

        private Contact(int id, string phoneNumber, Guid ownerId)
        {
            Id = id;
            PhoneNumber = phoneNumber;
            OwnerId = ownerId;
        }

        public static Contact Create(string phoneNumber, Guid ownerId)
        {
            return new Contact(0, phoneNumber, ownerId);
        }

        public void Update(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
