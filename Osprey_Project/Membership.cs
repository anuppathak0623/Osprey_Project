using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osprey_Project
{
    class Membership
    {
        public string Id { get; set; }  // MongoDB Unique ID
        public string Name { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }
        public string Plan { get; set; }
    }
}
