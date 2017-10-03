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

namespace Mortfors.Employee.Bookings
{
    /// <summary>
    /// Interaction logic for ChooseBusrideWindow.xaml
    /// </summary>
    public partial class ChooseBusrideWindow : Window
    {

        public ChangeBookingWindow parentWindow;
        List<BusrideObject> busrideObject;
        

        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public ChooseBusrideWindow(ChangeBookingWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            busrideObject = new List<BusrideObject>();
            this.Title = "Choose Booking - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateBusrides();

        }

        void ChooseBusrideWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_change_busride_id.IsEnabled = true;
            parentWindow.b_change_traveller.IsEnabled = true;
        }

        public void UpdateBusrides()
        {

            count = DBConnection.CountBusrides();
            busrideObject = DBConnection.SelectBusrides(limit, offset);
            lv_lista.ItemsSource = busrideObject;
            l_visar.Content = "Showing " + offset + " - " + (offset + limit) + " av " + count + ".";
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

            UpdateBusrides();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateBusrides();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateBusrides();
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_choose_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                BusrideObject selected = (BusrideObject)lv_lista.SelectedItem;
                
                    parentWindow.tb_busride_id.Text = selected.busride_id.ToString();
                
                Close();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
