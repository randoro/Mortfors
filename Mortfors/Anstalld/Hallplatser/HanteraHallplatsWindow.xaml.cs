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

namespace Mortfors.Anstalld.Hallplatser
{
    /// <summary>
    /// Interaction logic for HallplatsWindow.xaml
    /// </summary>
    public partial class HanteraHallplatsWindow : Window
    {
        public AnstalldWindow parentWindow;
        AndraHallplatsWindow andraHallplats;
        List<HallplatsObject> hallplatsObject;

        
        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public HanteraHallplatsWindow(AnstalldWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            hallplatsObject = new List<HallplatsObject>();
            this.Title = "Hantera Hållplatser - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateHallplatser();

        }

        void HanteraHallplatsWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_hanterahallplatser.IsEnabled = true;
        }

        public void UpdateHallplatser()
        {
            
            count = DBConnection.CountHallplatser();
            hallplatsObject = DBConnection.SelectHallplatser(limit, offset);
            lv_lista.ItemsSource = hallplatsObject;
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
        }

        private void b_ny_Click(object sender, RoutedEventArgs e)
        {
            b_ny.IsEnabled = false;
            b_redigeramarkerad.IsEnabled = false;
            b_tabortmarkerad.IsEnabled = false;
            andraHallplats = new AndraHallplatsWindow(this);
            andraHallplats.ShowDialog();
        }

        private void b_redigeramarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                b_ny.IsEnabled = false;
                b_redigeramarkerad.IsEnabled = false;
                b_tabortmarkerad.IsEnabled = false;
                andraHallplats = new AndraHallplatsWindow(this, (HallplatsObject)lv_lista.SelectedItem);
                andraHallplats.ShowDialog();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_tabortmarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                if (DBConnection.DeleteHallplats((HallplatsObject)lv_lista.SelectedItem) > 0)
                {
                    parentWindow.UpdateAllChain();
                }

            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
