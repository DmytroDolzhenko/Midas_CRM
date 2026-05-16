using Midas.Core.ProductCategories;
using Midas.Core.ProductImages;
using Midas.Core.ProductVariants;
using Midas.Core.Warehouses;
using System;
using System.Collections.Generic;

namespace Midas.Core.Products
{
    public class Product : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }

        public int WarehouseId { get; private set; }
        public Warehouse Warehouse { get; private set; } = null!;

        public string Name { get; private set; }
        public string Description { get; private set; }

        public int ProductCategoryId { get; private set; }
        public ProductCategory ProductCategory { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public Guid OwnerId { get; private set; }

        private readonly List<ProductVariant> _variants = new();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

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
        public void AddImage(string url, bool isMain = false)
        {
            if (!_images.Any()) isMain = true;

            if (isMain)
            {
                foreach (var img in _images) img.UnsetMain();
            }

            _images.Add(ProductImage.Create(url, Id, OwnerId, isMain));
        }
    }
}
