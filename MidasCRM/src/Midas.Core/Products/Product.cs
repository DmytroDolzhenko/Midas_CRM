using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Products
{
    public class Product : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }
        public int WarehouseId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int ProductCategoryId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public Guid OwnerId { get; private set; }

        private Product(
            int id,
            int warehouseId,
            string name,
            string description,
            int productCategoryId,
            DateTime createdAt,
            Guid ownerId)
        {
            Id = id;
            WarehouseId = warehouseId;
            Name = name;
            Description = description;
            ProductCategoryId = productCategoryId;
            CreatedAt = createdAt;
            OwnerId = ownerId;
        }

        public static Product Create(
            int warehouseId,
            string name,
            string description,
            int productCategoryId,
            Guid ownerId)
        {
            return new Product(
                0,
                warehouseId,
                name,
                description,
                productCategoryId,
                DateTime.UtcNow,
                ownerId);
        }

        public void Update(
            string name,
            string description)
        {
            Name = name;
            Description = description;
        }

        public void UpdateCategory(int productCategoryId)
        {
            ProductCategoryId = productCategoryId;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
        public void ChangeWarehouse(int newWarehouseId)
        {
            WarehouseId = newWarehouseId;
        }
    }
}
