using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class HallplatsObject
    {
        public string gatu_adress { get; set; }
        public string stad { get; set; }
        public string land { get; set; }

        public HallplatsObject(string _gatu_adress, string _stad, string _land)
        {
            gatu_adress = _gatu_adress;
            stad = _stad;
            land = _land;
        }

    }
}
