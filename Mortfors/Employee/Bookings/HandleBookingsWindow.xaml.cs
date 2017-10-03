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
    /// Interaction logic for HandleBookingWindow.xaml
    /// </summary>
    public partial class HandleBookingWindow : Window
    {
        public EmployeeWindow parentWindow;
        ChangeBookingWindow changeBooking;
        List<BookingObject> bookingObject;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public HandleBookingWindow(EmployeeWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            bookingObject = new List<BookingObject>();
            this.Title = "Handle Bookings - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateBookings();

        }

        void HandleBookingWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_hanterabookings.IsEnabled = true;
        }

        public void UpdateBookings()
        {

            count = DBConnection.CountBookings();
            bookingObject = DBConnection.SelectBookings(limit, offset);
            lv_lista.ItemsSource = bookingObject;
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

        private void b_new_Click(object sender, RoutedEventArgs e)
        {
            b_new.IsEnabled = false;
            b_editselected.IsEnabled = false;
            b_deleteselected.IsEnabled = false;
            changeBooking = new ChangeBookingWindow(this);
            changeBooking.ShowDialog();
        }

        private void b_editselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                b_new.IsEnabled = false;
                b_editselected.IsEnabled = false;
                b_deleteselected.IsEnabled = false;
                changeBooking = new ChangeBookingWindow(this, (BookingObject)lv_lista.SelectedItem);
                changeBooking.ShowDialog();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_deleteselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                if (DBConnection.DeleteBooking((BookingObject)lv_lista.SelectedItem) > 0)
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
