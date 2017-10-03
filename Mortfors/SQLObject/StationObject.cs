using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class StationObject
    {
        public string street_address { get; set; }
        public string city { get; set; }
        public string country { get; set; }

        public StationObject(string _street_address, string _city, string _country)
        {
            street_address = _street_address;
            city = _city;
            country = _country;
        }

    }
}
