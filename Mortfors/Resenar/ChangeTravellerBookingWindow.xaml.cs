using Mortfors.Login;
using Mortfors.SQLObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mortfors.Traveller
{
    /// <summary>
    /// Interaction logic for ChangeTravellerBookingWindow.xaml
    /// </summary>
    public partial class ChangeTravellerBookingWindow : Window
    {
        public TravellerWindow parentWindow;
        readonly BusrideObject oldObject;
        

        public ChangeTravellerBookingWindow(TravellerWindow _parent, BusrideObject _oldObject)
        {
            InitializeComponent();
            Title = "New Booking - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            l_departure.Content = "Departure: " + Environment.NewLine +  oldObject.departure_date.ToString("dd-MMM-yyyy HH:mm:ss") + Environment.NewLine + oldObject.departure_address + Environment.NewLine + oldObject.departure_city + Environment.NewLine + oldObject.departure_country;
            l_arrival.Content = "Arrival: " + Environment.NewLine + oldObject.arrival_date.ToString("dd-MMM-yyyy HH:mm:ss") + Environment.NewLine + oldObject.arrival_address + Environment.NewLine + oldObject.arrival_city + Environment.NewLine + oldObject.arrival_country;
            tb_seats.Text = "";


            parentWindow = _parent;
        }

        void ChangeBookingWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_bookselected.IsEnabled = true;
            parentWindow.b_cancelselected.IsEnabled = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            if (tb_seats.Text == "")
            {
                MessageBox.Show("Empty fields are not allowed.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }



            BookingObject newObject = new BookingObject(oldObject.busride_id, ((TravellerObject)Authenticator.currentUser).email, Int32.Parse(tb_seats.Text));

            if (DBConnection.CheckIfBookingAllowed(newObject))
            {
                if (DBConnection.InsertBooking(newObject) > 0)
                {
                    parentWindow.UpdateAllChain();
                    Close();
                }
                
            }
            else
            {
                MessageBox.Show("Not enough free seats on the busride.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
