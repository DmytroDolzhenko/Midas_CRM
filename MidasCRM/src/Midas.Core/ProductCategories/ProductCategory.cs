using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.ProductCategories
{
    public class ProductCategory
    {
        public int Id { get; }
        public string Name { get; private set; }
        public bool IsDeleted { get; private set; }
        private ProductCategory(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public static ProductCategory Create(int id, string name)
        {
            return new ProductCategory(id, name);
        }
        public void Update(string name)
        {
            Name = name;
        }
        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
    }
}
