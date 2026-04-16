using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.OrderSources
{
    public class OrderSource
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public bool IsDeleted { get; private set; } 
        private OrderSource(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static OrderSource Create(int id, string name)
        {
            return new OrderSource(id, name);
        }

        public void Update(string name)
        {
            Name = name;
        }
        public void MarkAsDelete()
        {
            IsDeleted = true;
        }
    }
}
