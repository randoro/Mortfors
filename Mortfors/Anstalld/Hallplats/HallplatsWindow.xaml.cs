using Mortfors.Login;
using Mortfors.SQLObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mortfors
{
    /// <summary>
    /// Interaction logic for HallplatsWindow.xaml
    /// </summary>
    public partial class HallplatsWindow : Window
    {
        AnstalldWindow parent;
        AndraHallplats andraHallplats;
        List<HallplatsObject> hallplatsObject;


        public string errorMessage = "";
        const int limit = 2;
        public int offset = 0;
        public int count = 0;

        public HallplatsWindow(AnstalldWindow _parent)
        {
            InitializeComponent();
            parent = _parent;
            hallplatsObject = new List<HallplatsObject>();
            this.Title = "Hantera Hållplatser - Välkommen " + Authenticator.GetUserInfo() + ".";

        }

        void HallplatsWindow_Closing(object sender, CancelEventArgs e)
        {
            parent.b_hanterahallplatser.IsEnabled = true;
        }

        public void UpdateHallplatser()
        {
            bool found = false;
            count = DBConnection.ConnectCountHallplatser(out found, out errorMessage);
            bool found2 = false;
            hallplatsObject = DBConnection.ConnectSelectHallplatser(limit, offset, out found2, out errorMessage);
            lv_hallplatser.ItemsSource = hallplatsObject;
            l_visar.Content = "Visar " + offset + " - " + (offset + limit) + " av " + count + ".";
            DisableButtons();
        }

        public void DisableButtons()
        {
            if (offset == 0)
            {
                b_forra.IsEnabled = false;
            }
            else
            {
                b_forra.IsEnabled = true;
            }

            if (offset + limit >= count)
            {
                b_nasta.IsEnabled = false;
            }
            else
            {
                b_nasta.IsEnabled = true;
            }
        }

        private void b_forra_Click(object sender, RoutedEventArgs e)
        {
            if (offset < limit)
            {
                offset = 0;
            }
            else
            {
                offset -= limit;
            }

            UpdateHallplatser();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateHallplatser();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateHallplatser();
            HallplatsObject a = hallplatsObject[0];
            Console.WriteLine("Gatuadress:" + a.gatu_adress + " Stad:" + a.stad + " Land:" + a.land);
        }

        private void b_nyhallplats_Click(object sender, RoutedEventArgs e)
        {
            andraHallplats = new AndraHallplats(this);
            andraHallplats.Show();
            b_nyhallplats.IsEnabled = false;
            b_redigeramarkerad.IsEnabled = false;
            b_tabortmarkerad.IsEnabled = false;

        }

        private void b_redigeramarkerad_Click(object sender, RoutedEventArgs e)
        {
            andraHallplats = new AndraHallplats(this, new SQLObject.HallplatsObject("", "", "")); //Fix
            andraHallplats.Show();
            b_nyhallplats.IsEnabled = false;
            b_redigeramarkerad.IsEnabled = false;
            b_tabortmarkerad.IsEnabled = false;
        }

        private void b_tabortmarkerad_Click(object sender, RoutedEventArgs e)
        {
            
            b_nyhallplats.IsEnabled = false;
            b_redigeramarkerad.IsEnabled = false;
            b_tabortmarkerad.IsEnabled = false;
            //Fix
        }
    }
}
