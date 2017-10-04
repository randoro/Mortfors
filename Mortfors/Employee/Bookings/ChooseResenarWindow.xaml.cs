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
    /// Interaction logic for ChooseTravellerWindow.xaml
    /// </summary>
    public partial class ChooseTravellerWindow : Window
    {
        public ChangeBookingWindow parentWindow;
        List<TravellerObject> travellerObject;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public ChooseTravellerWindow(ChangeBookingWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            travellerObject = new List<TravellerObject>();
            this.Title = "Choose Traveller - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateTravellers();

        }

        void ChooseTravellerWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_change_busride_id.IsEnabled = true;
            parentWindow.b_change_traveller.IsEnabled = true;
        }

        public void UpdateTravellers()
        {

            count = DBConnection.CountTravellers();
            travellerObject = DBConnection.SelectTravellers(limit, offset);
            lv_lista.ItemsSource = travellerObject;
            l_visar.Content = "Showing " + offset + " - " + (offset + limit) + " of " + count + ".";
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

            UpdateTravellers();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateTravellers();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateTravellers();
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_choose_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                TravellerObject selected = (TravellerObject)lv_lista.SelectedItem;
                parentWindow.tb_traveller.Text = selected.email;

                Close();
            }
            else
            {
                MessageBox.Show("Nothing selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

