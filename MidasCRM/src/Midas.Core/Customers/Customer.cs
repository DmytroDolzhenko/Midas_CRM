using System;
using System.Collections.Generic;
using System.Text;
using Midas.Core.Contacts;
using Midas.Core.CustomerAddresses;
using Midas.Core.Orders;

namespace Midas.Core.Customers
{
    public class Customer : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }
        public string Name { get; private set; }
        public string Surname { get; private set; }

        public int ContactId { get; private set; }
        public Contact Contact { get; private set; } = null!;

        public string Email { get; private set; }
        public bool IsDeleted { get; private set; }
        public Guid OwnerId { get; private set; }

        private readonly List<CustomerAddress> _addresses = new();
        public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        private Customer(
            int id,
            string name,
            string surname,
            int contactId,
            string email,
            Guid ownerId)
        {
            Id = id;
            Name = name;
            Surname = surname;
           // Contact = contact;
            ContactId = contactId;
            Email = email;
            OwnerId = ownerId;
        }

        public static Customer Create(
            string name,
            string surname,
            int contactId,
            string email,
            Guid ownerId)
        {
            return new Customer(
                0,
                name,
                surname,
                contactId,
                email,
                ownerId);
        }

        public void Update(
            string name,
            string surname,
            int contactId,
            string email)
        {
            Name = name;
            Surname = surname;
            ContactId = contactId;
            Email = email;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
    }
}
