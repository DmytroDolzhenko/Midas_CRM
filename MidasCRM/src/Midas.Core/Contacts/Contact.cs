using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Midas.Core.Contacts
{
    public class Contact : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }
        public string Value { get; private set; }
        public Guid OwnerId { get; private set; }
        public bool IsDeleted { get; private set; }
        private Contact(int id, string value, Guid ownerId)
        {
            Id = id;
            Value = value;
            OwnerId = ownerId;
        }

        public static Contact Create(string value, Guid ownerId) 
        {
            return new Contact(0, value, ownerId);
        }
        public void Update(string value)
        {
            Value = value;
        }
        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
