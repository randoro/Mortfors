using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class ResenarObject : UserObject
    {
        public string email { get; set; }

        public ResenarObject(string _email, string _hashedPassword, string _namn, string _adress, string _telefon)
            : base(_hashedPassword, _namn, _adress, _telefon)
        {
            email = _email;
        }
    }
}