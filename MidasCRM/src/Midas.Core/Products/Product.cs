using Midas.Core.ProductCategories;
using Midas.Core.ProductImages;
using Midas.Core.ProductVariants;
using Midas.Core.Warehouses;
using System;
using System.Collections.Generic;

namespace Midas.Core.Products
{
    public class Product : IEntity<int>, ICompanyOwnedEntity
    {
        public int Id { get; }

        public int WarehouseId { get; private set; }
        public Warehouse Warehouse { get; private set; } = null!;

        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Weight { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public Guid CompanyId { get; private set; }

        private readonly List<ProductCategoryLink> _productCategories = new();
        public IReadOnlyList<ProductCategoryLink> ProductCategories => _productCategories.AsReadOnly();

        private readonly List<ProductVariant> _variants = new();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();


        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
        private Product(
            int id,
            int warehouseId,
            string name,
            string description,
            decimal weight,
            DateTime createdAt,
            Guid companyId)
        {
            Id = id;
            WarehouseId = warehouseId;
            Name = name;
            Description = description;
            Weight = weight;
            CreatedAt = createdAt;
            CompanyId = companyId;
        }

        public static Product Create(
            int warehouseId,
            string name,
            string description,
            decimal weight,
            IEnumerable<int> productCategoryIds,
            Guid companyId)
        {
            var product = new Product(
                0,
                warehouseId,
                name,
                description,
                weight,
                DateTime.UtcNow,
                companyId);

            foreach (var productCategory in productCategoryIds)
            {
                product.AddCategory(productCategory);
            }
            return product;
        }

        public void Update(
            string name,
            string description,
            decimal weight)
        {
            Name = name;
            Description = description;
            Weight = weight;
        }

        public void UpdateCategory(int productCategoryId, int newProductCategoryId)
        {
            var editingCategory = _productCategories.FirstOrDefault(c => c.CategoryId == productCategoryId);
            if (editingCategory != null)
            {
                _productCategories.Remove(editingCategory);
            }

            var newCategory = _productCategories.FirstOrDefault(c => c.CategoryId == newProductCategoryId);
            if (newCategory == null)
            {
                _productCategories.Add(ProductCategoryLink.Create(Id, newProductCategoryId));
            }
        }
        public void AddCategory(int productCategoryId)
        {
            if (!_productCategories.Any(c => c.CategoryId == productCategoryId))
            {
                _productCategories.Add(ProductCategoryLink.Create(Id, productCategoryId));
            }
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

            _images.Add(ProductImage.Create(url, Id, CompanyId, isMain));
        }
    }
}

