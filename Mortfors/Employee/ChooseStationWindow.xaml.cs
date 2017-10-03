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

namespace Mortfors.Employee
{
    /// <summary>
    /// Interaction logic for ChooseStationWindow.xaml
    /// </summary>
    public partial class ChooseStationWindow : Window
    {
        public ChangeBusrideWindow parentWindow;
        List<StationObject> stationObject;

        bool isAvgang;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public ChooseStationWindow(ChangeBusrideWindow _parent, bool _isAvgang)
        {
            InitializeComponent();
            parentWindow = _parent;
            isAvgang = _isAvgang;
            stationObject = new List<StationObject>();
            this.Title = "Choose Station - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateStations();

        }

        void ChooseStationWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_change_departure.IsEnabled = true;
            parentWindow.b_change_arrival.IsEnabled = true;
            parentWindow.b_change_driver_id.IsEnabled = true;
        }

        public void UpdateStations()
        {

            count = DBConnection.CountStations();
            stationObject = DBConnection.SelectStations(limit, offset);
            lv_lista.ItemsSource = stationObject;
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

            UpdateStations();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateStations();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateStations();
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_choose_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                StationObject selected = (StationObject)lv_lista.SelectedItem;

                if (isAvgang)
                {
                    parentWindow.departure_address = selected.street_address;
                    parentWindow.departure_city = selected.city;
                    parentWindow.departure_country = selected.country;
                    parentWindow.tb_departure.Text = selected.street_address + ", " + selected.city + ", " + selected.country;
                }
                else
                {
                    parentWindow.arrival_address = selected.street_address;
                    parentWindow.arrival_city = selected.city;
                    parentWindow.arrival_country = selected.country;
                    parentWindow.tb_arrival.Text = selected.street_address + ", " + selected.city + ", " + selected.country;
                }
                Close();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
