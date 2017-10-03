using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public abstract class UserObject
    {
        public string hashedPassword { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }

        public UserObject(string _hashedPassword, string _name, string _address, string _phone)
        {
            hashedPassword = _hashedPassword;
            name = _name;
            address = _address;
            phone = _phone;
    }
    }
}
