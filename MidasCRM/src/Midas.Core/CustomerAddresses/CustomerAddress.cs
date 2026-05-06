using System;
using System.Collections.Generic;
using System.Text;
using Midas.Core.Customers;
using Midas.Core.Orders;

namespace Midas.Core.CustomerAddresses
{
    public class CustomerAddress : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }
        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;

        public string City { get; private set; }
        public int PostalCode { get; private set; }
        public int PostDepartmentNumber { get; private set; }
        public Guid OwnerId { get; private set; }
        public bool IsDeleted { get; private set; }

        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        private CustomerAddress(
            int id,
            int customerId,
            string city,
            int postalCode,
            int postDepartmentNumber,
            Guid ownerId)
        {
            Id = id;
            CustomerId = customerId;
            City = city;
            PostalCode = postalCode;
            PostDepartmentNumber = postDepartmentNumber;
            OwnerId = ownerId;
        }

        public static CustomerAddress Create(
            int id,
            int customerId,
            string city,
            int postalCode,
            int postDepartmentNumber,
            Guid ownerId)
        {
            return new CustomerAddress(
                id,
                customerId,
                city,
                postalCode,
                postDepartmentNumber,
                ownerId);
        }

        public void Update(
            int customerId,
            string city,
            int postalCode,
            int postDepartmentNumber)
        {
            CustomerId = customerId;
            City = city;
            PostalCode = postalCode;
            PostDepartmentNumber = postDepartmentNumber;
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
