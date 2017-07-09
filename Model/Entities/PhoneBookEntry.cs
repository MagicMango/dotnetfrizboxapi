using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FritzBoxAPI.Model.Entities
{
    public class PhoneBookEntry
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Number { get; set; }
        public String Type { get; set; }
    }
}
