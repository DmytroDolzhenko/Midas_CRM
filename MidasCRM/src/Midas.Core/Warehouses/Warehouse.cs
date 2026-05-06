using Midas.Core.Products;
using System;
using System.Collections.Generic;

namespace Midas.Core.Warehouses
{
    public class Warehouse : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }
        public string Name { get; private set; }
        public Guid OwnerId { get; private set; }

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public Warehouse(string name, Guid ownerId)
        {
            Name = name;
            OwnerId = ownerId;
        }

        public static Warehouse Create(string name, Guid ownerId)
        {
            return new Warehouse(name, ownerId);
        }

        public void Update(string name)
        {
            Name = name;
        }

        public void ChangeOwner(Guid newOwnerId)
        {
            OwnerId = newOwnerId;
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
