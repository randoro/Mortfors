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

namespace Mortfors.Traveller
{
    /// <summary>
    /// Interaction logic for HandleMyBookingsWindow.xaml
    /// </summary>
    public partial class HandleMyBookingsWindow : Window
    {
        public TravellerWindow parentWindow;
        List<MyBookingObject> bookingObject;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public HandleMyBookingsWindow(TravellerWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            bookingObject = new List<MyBookingObject>();
            this.Title = "Handle Bookings - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateBookings();

        }

        void HandleMyBookingsWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_mybookings.IsEnabled = true;
        }

        public void UpdateBookings()
        {
            this.Title = "Handle Bookings - Welcome " + Authenticator.GetUserInfo() + ".";
            count = DBConnection.CountBookings();
            bookingObject = DBConnection.SelectBookingsJoinBusride(((TravellerObject)Authenticator.currentUser).email, limit, offset);
            lv_lista.ItemsSource = bookingObject;
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

            UpdateBookings();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateBookings();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateBookings();
        }

        private void b_deleteselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                MyBookingObject minBooking = (MyBookingObject)lv_lista.SelectedItem;

                BookingObject booking = new BookingObject(minBooking.busride.busride_id, minBooking.traveller, minBooking.seats);
                if (DBConnection.DeleteBooking(booking) > 0)
                {
                    parentWindow.UpdateAllChain();
                }

            }
            else
            {
                MessageBox.Show("Nothing selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
