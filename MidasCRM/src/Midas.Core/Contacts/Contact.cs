using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Midas.Core.Customers;

namespace Midas.Core.Contacts
{
    public class Contact : IEntity<int>, ICompanyOwnedEntity
    {
        public int Id { get; }
        public string PhoneNumber { get; private set; }
        public Guid CompanyId { get; private set; }
        public bool IsDeleted { get; private set; }

        private readonly List<Customer> _customers = new();
        public IReadOnlyCollection<Customer> Customers => _customers.AsReadOnly();

        private Contact(int id, string phoneNumber, Guid companyId)
        {
            Id = id;
            PhoneNumber = phoneNumber;
            CompanyId = companyId;
        }

        public static Contact Create(string phoneNumber, Guid companyId)
        {
            return new Contact(0, phoneNumber, companyId);
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

