using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class AnstalldObject : UserObject
    {
        public string personNummer { get; set; }
        public bool isAdmin { get; set; }


        public AnstalldObject(string _personNummer, string _hashedPassword, bool _isAdmin, string _namn, string _adress, string _telefon)
            : base(_hashedPassword, _namn, _adress, _telefon)
        {
            personNummer = _personNummer;
            isAdmin = _isAdmin;
        }
    }
}
