using Midas.Core.Products;
using Midas.Core.UserProductCategories;
using System;
using System.Collections.Generic;

namespace Midas.Core.ProductCategories
{
    public class ProductCategory : IEntity<int>
    {
        public int Id { get; }
        public string Name { get; private set; }
        public bool IsDeleted { get; private set; }
        public bool IsPublic { get; private set; }

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private readonly List<UserProductCategory> _userCategories = new();
        public IReadOnlyCollection<UserProductCategory> UserCategories => _userCategories.AsReadOnly();

        private ProductCategory(int id, string name, bool isPublic)
        {
            Id = id;
            Name = name;
            IsPublic = isPublic;
        }

        public static ProductCategory Create(int id, string name, bool isPublic)
        {
            return new ProductCategory(id, name, isPublic);
        }

        public void Update(string name)
        {
            Name = name;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
        public void MarkAsPublic()
        {
            IsPublic = true;
        }
    }
}
