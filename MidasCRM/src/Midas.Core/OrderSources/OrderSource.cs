using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.OrderSources
{
    public class OrderSource : IEntity<int>, ICompanyOwnedEntity
    {
        //цей клас під питання, його взагалі можна забрати з системи,
        //а на заміну йому використовувати просто захардкожений Enum (OLX, Shafa, Telegram ...)
        public int Id { get;}
        public string Name { get; private set; }
        public Guid CompanyId { get; private set; }
        public bool IsDeleted { get; private set; } 
        private OrderSource(int id, string name, Guid companyId)
        {
            Id = id;
            Name = name;
            CompanyId = companyId;
        }

        public static OrderSource Create(int id, string name, Guid companyId)
        {
            return new OrderSource(id, name, companyId);
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

