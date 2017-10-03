using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class BookingObject
    {
        public int busride_id { get; set; }
        public string traveller { get; set; }
        public int seats { get; set; }

        public BookingObject(int _busride_id, string _traveller, int _seats)
        {
            busride_id = _busride_id;
            traveller = _traveller;
            seats = _seats;
        }
    }
}
