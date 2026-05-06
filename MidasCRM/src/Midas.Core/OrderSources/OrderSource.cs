using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.OrderSources
{
    public class OrderSource : IEntity<int>, IOwnedEntity
    {
        //цей клас під питання, його взагалі можна забрати з системи,
        //а на заміну йому використовувати просто захардкожений Enum (OLX, Shafa, Telegram ...)
        public int Id { get;}
        public string Name { get; private set; }
        public Guid OwnerId { get; private set; }
        public bool IsDeleted { get; private set; } 
        private OrderSource(int id, string name, Guid ownerId)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
        }

        public static OrderSource Create(int id, string name, Guid ownerId)
        {
            return new OrderSource(id, name, ownerId);
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
