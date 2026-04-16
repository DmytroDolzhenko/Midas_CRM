using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Midas.Core.Contacts
{
    public class Contact
    {
        public int Id { get; }
        public string Value { get; private set; }
        public bool IsDeleted { get; private set; }
        private Contact(int id, string value)
        {
            Id = id;
            Value = value;
        }

        public static Contact Create(string value)
        {
            return new Contact(0, value);
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
