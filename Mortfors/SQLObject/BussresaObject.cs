using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.SQLObject
{
    public class BussresaObject
    {
        public int bussresa_id { get; set; }
        public string avgangs_adress { get; set; }
        public string avgangs_stad { get; set; }
        public string avgangs_land { get; set; }
        public DateTime avgangs_datum { get; set; }
        public string ankomst_adress { get; set; }
        public string ankomst_stad { get; set; }
        public string ankomst_land { get; set; }
        public DateTime ankomst_datum { get; set; }
        public int kostnad { get; set; }
        public int max_platser { get; set; }
        public string chaffor_id { get; set; }

        public BussresaObject(int _bussresa_id, string _avgangs_adress, string _avgangs_stad, string _avgangs_land, DateTime _avgangs_datum,
            string _ankomst_adress, string _ankomst_stad, string _ankomst_land, DateTime _ankomst_datum, int _kostnad, int _max_platser, string _chaffor_id)
        {
            bussresa_id = _bussresa_id;
            avgangs_adress = _avgangs_adress;
            avgangs_stad = _avgangs_stad;
            avgangs_land = _avgangs_land;
            avgangs_datum = _avgangs_datum;
            ankomst_adress = _ankomst_adress;
            ankomst_stad = _ankomst_stad;
            ankomst_land = _ankomst_land;
            ankomst_datum = _ankomst_datum;
            kostnad = _kostnad;
            max_platser = _max_platser;
            chaffor_id = _chaffor_id;
    }

    }
}
