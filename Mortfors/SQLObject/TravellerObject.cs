using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class TravellerObject : UserObject
    {
        public string email { get; set; }

        public TravellerObject(string _email, string _hashedPassword, string _name, string _address, string _phone)
            : base(_hashedPassword, _name, _address, _phone)
        {
            email = _email;
        }
    }
}