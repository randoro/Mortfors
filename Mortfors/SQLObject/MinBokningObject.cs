using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    class MinBokningObject
    {
        public BussresaObject bussresa { get; set; }
        public string resenar { get; set; }
        public int antal_platser { get; set; }

        public MinBokningObject(BussresaObject _bussresa, string _resenar, int _antal_platser)
        {
            bussresa = _bussresa;
            resenar = _resenar;
            antal_platser = _antal_platser;
        }
    }
}
