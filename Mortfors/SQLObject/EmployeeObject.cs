using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class EmployeeObject : UserObject
    {
        public string personNummer { get; set; }
        public bool isAdmin { get; set; }

        public string adminString { get
            {
                return isAdmin ? "Yes" : "No";
            }
            private set
            {
                
            }
        }


        public EmployeeObject(string _personNummer, string _hashedPassword, bool _isAdmin, string _name, string _address, string _phone)
            : base(_hashedPassword, _name, _address, _phone)
        {
            personNummer = _personNummer;
            isAdmin = _isAdmin;
        }
    }
}
