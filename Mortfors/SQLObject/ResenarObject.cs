using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors
{
    class ResenarObject : UserObject
    {
        public readonly string email;

        public ResenarObject(string _email, string _hashedPassword, string _namn, string _address, string _telefon)
            : base(_hashedPassword, _namn, _address, _telefon)
        {
            email = _email;
        }
    }
}