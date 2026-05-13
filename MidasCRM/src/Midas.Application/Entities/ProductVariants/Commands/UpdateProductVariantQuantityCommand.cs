using Midas.Application.Common.Messaging;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.ProductVariants.Commands
{
    public class UpdateProductVariantQuantityCommand : ICommand<ProductVariant>
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}
