using System;
using System.Collections.Generic;
using System.Linq;

namespace FritzBoxAPI.Model.Entities
{
    public class PhoneBook
    {
        public Int32 Id { set; get; }
        public String Name { set; get; }
        public List<PhoneBookEntry> PhoneBookEntries { set; get; }
        public PhoneBook()
        {
            PhoneBookEntries = new List<PhoneBookEntry>();
        }
    }
}
