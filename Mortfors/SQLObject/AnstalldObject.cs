using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors
{
    class AnstalldObject : UserObject
    {
        public readonly string personNummer;
        public readonly bool isAdmin;


        public AnstalldObject(string _personNummer, string _hashedPassword, bool _isAdmin, string _namn, string _address, string _telefon)
            : base(_hashedPassword, _namn, _address, _telefon)
        {
            personNummer = _personNummer;
            isAdmin = _isAdmin;
        }
    }
}
