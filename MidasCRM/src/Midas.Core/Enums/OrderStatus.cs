using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Shipped = 2,
        Delivered = 3,
        Returned = 4,
        Received = 5,
        Cancelled = 6
    }
}
