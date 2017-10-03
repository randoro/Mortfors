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

namespace Mortfors.Anstalld.Bokningar
{
    /// <summary>
    /// Interaction logic for HanteraBokningWindow.xaml
    /// </summary>
    public partial class HanteraBokningWindow : Window
    {
        public AnstalldWindow parentWindow;
        AndraBokningWindow andraBokning;
        List<BokningObject> bokningObject;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public HanteraBokningWindow(AnstalldWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            bokningObject = new List<BokningObject>();
            this.Title = "Hantera Bokningar - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateBokningar();

        }

        void HanteraBokningWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_hanterabokningar.IsEnabled = true;
        }

        public void UpdateBokningar()
        {

            count = DBConnection.CountBokningar();
            bokningObject = DBConnection.SelectBokningar(limit, offset);
            lv_lista.ItemsSource = bokningObject;
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

            UpdateBokningar();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateBokningar();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateBokningar();
        }

        private void b_ny_Click(object sender, RoutedEventArgs e)
        {
            b_ny.IsEnabled = false;
            b_redigeramarkerad.IsEnabled = false;
            b_tabortmarkerad.IsEnabled = false;
            andraBokning = new AndraBokningWindow(this);
            andraBokning.ShowDialog();
        }

        private void b_redigeramarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                b_ny.IsEnabled = false;
                b_redigeramarkerad.IsEnabled = false;
                b_tabortmarkerad.IsEnabled = false;
                andraBokning = new AndraBokningWindow(this, (BokningObject)lv_lista.SelectedItem);
                andraBokning.ShowDialog();
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
                if (DBConnection.DeleteBokning((BokningObject)lv_lista.SelectedItem) > 0)
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
