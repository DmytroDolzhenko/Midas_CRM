using Midas.Core.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.ProductImages
{
    public class ProductImage : IEntity<int>, IOwnedEntity
    {
        public int Id { get; private set; }
        public string Url { get; private set; } = null!;
        public bool IsMain { get; private set; }
        public int ProductId { get; private set; }
        public Guid OwnerId { get; private set; }
        public Product Product { get; private set; }

        public static ProductImage Create(string url, int productId, Guid ownerId, bool isMain = false)
        {
            return new ProductImage
            {
                Url = url,
                ProductId = productId,
                OwnerId = ownerId,
                IsMain = isMain
            };
        }
        public void SetAsMain() => IsMain = true;
        public void UnsetMain() => IsMain = false;
    }
}
