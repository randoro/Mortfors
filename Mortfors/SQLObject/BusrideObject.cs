using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class BusrideObject
    {
        public int busride_id { get; set; }
        public string departure_address { get; set; }
        public string departure_city { get; set; }
        public string departure_country { get; set; }
        public DateTime departure_date { get; set; }
        public string arrival_address { get; set; }
        public string arrival_city { get; set; }
        public string arrival_country { get; set; }
        public DateTime arrival_date { get; set; }
        public int cost { get; set; }
        public int max_seats { get; set; }
        public string driver_id { get; set; }

        public BusrideObject(int _busride_id, string _departure_address, string _departure_city, string _departure_country, DateTime _departure_date,
            string _arrival_address, string _arrival_city, string _arrival_country, DateTime _arrival_date, int _cost, int _max_seats, string _driver_id)
        {
            busride_id = _busride_id;
            departure_address = _departure_address;
            departure_city = _departure_city;
            departure_country = _departure_country;
            departure_date = _departure_date;
            arrival_address = _arrival_address;
            arrival_city = _arrival_city;
            arrival_country = _arrival_country;
            arrival_date = _arrival_date;
            cost = _cost;
            max_seats = _max_seats;
            driver_id = _driver_id;
    }

    }
}
