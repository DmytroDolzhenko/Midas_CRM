using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Products
{
    public class Product
    {
        public int Id { get; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int ProductCategoryId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private Product(
            int id,
            string name,
            string description,
            int productCategoryId,
            DateTime createdAt)
        {
            Id = id;
            Name = name;
            Description = description;
            ProductCategoryId = productCategoryId;
            CreatedAt = createdAt;
        }

        public static Product Create(
            string name,
            string description,
            int productCategoryId)
        {
            return new Product(
                0,
                name,
                description,
                productCategoryId,
                DateTime.UtcNow);
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
    }
}
