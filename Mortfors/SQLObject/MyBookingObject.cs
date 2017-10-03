using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    class MyBookingObject
    {
        public BusrideObject busride { get; set; }
        public string traveller { get; set; }
        public int seats { get; set; }

        public MyBookingObject(BusrideObject _busride, string _traveller, int _seats)
        {
            busride = _busride;
            traveller = _traveller;
            seats = _seats;
        }
    }
}
