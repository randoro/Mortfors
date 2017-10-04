using Mortfors.Employee;
using Mortfors.Employee.Employees;
using Mortfors.Employee.Bookings;
using Mortfors.Employee.Stations;
using Mortfors.Employee.Travellers;
using Mortfors.Login;
using Mortfors.SQLObject;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {


        HandleStationWindow stationWindow;
        HandleTravellerWindow travellerWindow;
        HandleEmployeeWindow employeeWindow;
        HandleBookingWindow bookingWindow;

        ChangeBusrideWindow changeBusrideWindow;

        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public EmployeeWindow()
        {
            InitializeComponent();
            SetPermissions();
            this.Title = "Mortfors - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateBussResor();

        }

        private void SetPermissions()
        {
            if (Authenticator.authenticated)
            {
                if (((EmployeeObject)Authenticator.currentUser).isAdmin)
                {
                    b_korselected.IsEnabled = false;
                    b_avanmalkorningavselected.IsEnabled = false;

                    b_newresa.IsEnabled = true;
                    b_editselected.IsEnabled = true;
                    b_deleteselected.IsEnabled = true;
                    b_hanterastations.IsEnabled = true;
                    b_hanteratravellers.IsEnabled = true;
                    b_hanterabookings.IsEnabled = true;
                    b_hanteraemployees.IsEnabled = true;
                }
                else
                {
                    b_korselected.IsEnabled = true;
                    b_avanmalkorningavselected.IsEnabled = true;

                    b_newresa.IsEnabled = false;
                    b_editselected.IsEnabled = false;
                    b_deleteselected.IsEnabled = false;
                    b_hanterastations.IsEnabled = false;
                    b_hanteratravellers.IsEnabled = false;
                    b_hanterabookings.IsEnabled = false;
                    b_hanteraemployees.IsEnabled = false;
                }
            }
        }

        public void UpdateBussResor()
        {
            count = DBConnection.CountBusrides();
            lv_busrides.ItemsSource = DBConnection.SelectBusrides(limit, offset);
            l_visar.Content = "Showing "+ offset + " - "+ (offset+limit) + " of " + count + ".";
            DisableButtons();
        }

        public void UpdateAllChain()
        {
            UpdateBussResor();
            if (stationWindow != null && stationWindow.Visibility == Visibility.Visible)
            {
                stationWindow.UpdateStations();
            }
            if (travellerWindow != null && travellerWindow.Visibility == Visibility.Visible)
            {
                travellerWindow.UpdateTravellers();
            }
            if (employeeWindow != null && employeeWindow.Visibility == Visibility.Visible)
            {
                employeeWindow.UpdateEmployees();
            }
            if (bookingWindow != null && bookingWindow.Visibility == Visibility.Visible)
            {
                bookingWindow.UpdateBookings();
            }
        }

        public void DisableButtons()
        {
            if(offset == 0)
            {
                b_forra.IsEnabled = false;
            }
            else
            {
                b_forra.IsEnabled = true;
            }

            if (offset+limit >= count)
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

            UpdateBussResor();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateBussResor();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateBussResor();
        }

        private void b_newresa_Click(object sender, RoutedEventArgs e)
        {
            b_newresa.IsEnabled = false;
            b_editselected.IsEnabled = false;
            b_deleteselected.IsEnabled = false;
            changeBusrideWindow = new ChangeBusrideWindow(this);
            changeBusrideWindow.ShowDialog();
        }

        private void b_editselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_busrides.SelectedItem != null)
            {
                b_newresa.IsEnabled = false;
                b_editselected.IsEnabled = false;
                b_deleteselected.IsEnabled = false;
                changeBusrideWindow = new ChangeBusrideWindow(this, (BusrideObject)lv_busrides.SelectedItem);
                changeBusrideWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Nothing selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_deleteselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_busrides.SelectedItem != null)
            {
                if (DBConnection.DeleteBusride((BusrideObject)lv_busrides.SelectedItem) > 0)
                {
                    UpdateAllChain();
                }

            }
            else
            {
                MessageBox.Show("Nothing selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_korselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_busrides.SelectedItem != null)
            {
                if (DBConnection.SetBusrideChaffor((BusrideObject)lv_busrides.SelectedItem, ((EmployeeObject)Authenticator.currentUser).personNummer) > 0)
                {
                    UpdateAllChain();
                }

            }
            else
            {
                MessageBox.Show("Nothing selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_avanmalkorningavselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_busrides.SelectedItem != null)
            {
                if (DBConnection.RemoveBusrideChaffor((BusrideObject)lv_busrides.SelectedItem, ((EmployeeObject)Authenticator.currentUser).personNummer) > 0)
                {
                    UpdateAllChain();
                }

            }
            else
            {
                MessageBox.Show("Nothing selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void b_hanterastations_Click(object sender, RoutedEventArgs e)
        {
            stationWindow = new HandleStationWindow(this);
            stationWindow.Show();
            b_hanterastations.IsEnabled = false;
        }

        private void b_hanteratravellers_Click(object sender, RoutedEventArgs e)
        {
            travellerWindow = new HandleTravellerWindow(this);
            travellerWindow.Show();
            b_hanteratravellers.IsEnabled = false;
        }

        private void b_hanterabookings_Click(object sender, RoutedEventArgs e)
        {
            bookingWindow = new HandleBookingWindow(this);
            bookingWindow.Show();
            b_hanterabookings.IsEnabled = false;
        }

        private void b_hanteraemployees_Click(object sender, RoutedEventArgs e)
        {
            employeeWindow = new HandleEmployeeWindow(this);
            employeeWindow.Show();
            b_hanteraemployees.IsEnabled = false;
        }
    }
}
