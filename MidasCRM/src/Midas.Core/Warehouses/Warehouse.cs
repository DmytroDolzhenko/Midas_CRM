using Midas.Core.Products;
using System;
using System.Collections.Generic;

namespace Midas.Core.Warehouses
{
    public class Warehouse : IEntity<int>, ICompanyOwnedEntity
    {
        public int Id { get; }
        public string Name { get; private set; }
        public Guid CompanyId { get; private set; }

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public Warehouse(string name, Guid companyId)
        {
            Name = name;
            CompanyId = companyId;
        }

        public static Warehouse Create(string name, Guid companyId)
        {
            return new Warehouse(name, companyId);
        }

        public void Update(string name)
        {
            Name = name;
        }

        public void ChangeCompany(Guid newCompanyId)
        {
            CompanyId = newCompanyId;
        }

        public void AddProduct(Product product)
        {
            _products.Add(product);
        }

        public void RemoveProduct(Product product)
        {
            _products.Remove(product);
        }
    }
}

