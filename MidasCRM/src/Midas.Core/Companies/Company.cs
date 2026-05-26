using Midas.Core.CompanyMembers;
using Midas.Core.Enums;
using Midas.Core.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Companies
{
    public class Company : IEntity<Guid>, ICompanyOwnedEntity
    {
        public Guid Id { get; }
        public Guid CompanyId => Id;
        public string Name { get; private set; }
        public string? TaxNumber { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private readonly List<CompanyMember> _members = new();
        public IReadOnlyCollection<CompanyMember> Members => _members.AsReadOnly();

        private Company(
            Guid id,
            string name,
            DateTime createdAt)
        {
            Id = id;
            Name = name;
            CreatedAt = createdAt;
        }
        public static Company Create(string name, string? taxNumber, Guid ownerUserId)
        {
            var company = new Company(
                Guid.NewGuid(),
                name,
                DateTime.UtcNow
                );
            company.UpdateTaxNumber(taxNumber);

            company.AddMember(ownerUserId, CompanyRole.Owner);

            return company;
        }
        public void UpdateName(string newName)
        {
            Name = newName;
        }
        public void UpdateTaxNumber(string? newTaxNumber)
        {
            TaxNumber = newTaxNumber;

        }
        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
        public void ApplyFinancialOperation(FinancialOperationType operationType, decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Сума фінансової операції має бути більше 0.");
            }

            switch (operationType)
            {
                case FinancialOperationType.Accrual:
                    Balance += amount;
                    break;
                case FinancialOperationType.WriteOff:
                    Balance -= amount;
                    break;
                default:
                    throw new InvalidOperationException("Невідомий тип фінансової операції.");
            }
        }
        public void AddMember(Guid userId, CompanyRole role)
        {
            if (_members.Any(m => m.UserId == userId))
            {
                throw new InvalidOperationException("Користувач вже є учасником цієї компанії.");
            }

            _members.Add(CompanyMember.Create(Id, userId, role));
        }
        public void RemoveMember(Guid userId)
        {
            var member = _members.FirstOrDefault(m => m.UserId == userId);
            if (member != null)
            {
                if (member.Role == CompanyRole.Owner)
                {
                    throw new InvalidOperationException("Неможливо видалити власника компанії.");
                }
                _members.Remove(member);
            }
        }
    }
}
