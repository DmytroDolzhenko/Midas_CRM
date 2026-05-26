using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core
{
    public interface IEntity<TKey>
    {
        TKey Id { get;}
    }
}
