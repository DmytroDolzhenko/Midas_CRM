using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.CustomerAddresses
{
    public class CustomerAddress : IEntity<int>
    {
        public int Id { get; }
        public int CustomerId { get; private set; }
        public string City { get; private set; }
        public int PostalCode { get; private set; }
        public int PostDepartmentNumber { get; private set; }
        public bool IsDeleted { get; private set; }

        private CustomerAddress(
            int id,
            int customerId,
            string city,
            int postalCode,
            int postDepartmentNumber)
        {
            Id = id;
            CustomerId = customerId;
            City = city;
            PostalCode = postalCode;
            PostDepartmentNumber = postDepartmentNumber;
        }
        public static CustomerAddress Create(
            int id,
            int customerId,
            string city,
            int postalCode,
            int postDepartmentNumber)
        {
            return new CustomerAddress(
                id,
                customerId,
                city,
                postalCode,
                postDepartmentNumber);
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
