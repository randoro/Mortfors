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
        public string namn { get; set; }
        public string adress { get; set; }
        public string telefon { get; set; }

        public UserObject(string _hashedPassword, string _namn, string _adress, string _telefon)
        {
            hashedPassword = _hashedPassword;
            namn = _namn;
            adress = _adress;
            telefon = _telefon;
    }
    }
}
