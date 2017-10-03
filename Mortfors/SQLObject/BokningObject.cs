using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class BokningObject
    {
        public int bussresa_id { get; set; }
        public string resenar { get; set; }
        public int antal_platser { get; set; }

        public BokningObject(int _bussresa_id, string _resenar, int _antal_platser)
        {
            bussresa_id = _bussresa_id;
            resenar = _resenar;
            antal_platser = _antal_platser;
        }
    }
}
