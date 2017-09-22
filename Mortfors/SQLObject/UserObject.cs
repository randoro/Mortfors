using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors
{
    abstract class UserObject
    {
        public readonly string hashedPassword;
        public readonly string namn;
        public readonly string address;
        public readonly string telefon;
        
        public UserObject(string _hashedPassword, string _namn, string _address, string _telefon)
        {
            hashedPassword = _hashedPassword;
            namn = _namn;
            address = _address;
            telefon = _telefon;
    }
    }
}
