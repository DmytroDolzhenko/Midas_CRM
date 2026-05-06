using Midas.Core.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Warehouses
{
    public class Warehouse : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }
        public string Name { get; private set; }
        public Guid OwnerId { get; private set; }
        public List<Product> Products { get; private set; }

        public Warehouse(string name, Guid ownerId)
        {
            Name = name;
            OwnerId = ownerId;
            Products = new List<Product>();
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
            Products.Add(product);
        }
        public void RemoveProduct(Product product)
        {
           Products.Remove(product);
        }
    }
}
