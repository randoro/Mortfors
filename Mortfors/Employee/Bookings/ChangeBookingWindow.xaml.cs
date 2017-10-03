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

namespace Mortfors.Employee.Bookings
{
    /// <summary>
    /// Interaction logic for ChangeBookingWindow.xaml
    /// </summary>
    public partial class ChangeBookingWindow : Window
    {

        public HandleBookingWindow parentWindow;
        readonly BookingObject oldObject;
        bool newbooking;

        ChooseBusrideWindow chooseBusrideWindow;
        ChooseTravellerWindow chooseTravellerWindow;


        public ChangeBookingWindow(HandleBookingWindow _parent)
        {
            InitializeComponent();
            Title = "New Booking - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newbooking = true;
        }

        public ChangeBookingWindow(HandleBookingWindow _parent, BookingObject _oldObject)
        {
            InitializeComponent();
            Title = "Edit Booking - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_busride_id.Text = oldObject.busride_id.ToString();
            tb_traveller.Text = oldObject.traveller;
            tb_seats.Text = oldObject.seats.ToString();
            

            parentWindow = _parent;
            newbooking = false;
        }

        void ChangeBookingWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_new.IsEnabled = true;
            parentWindow.b_editselected.IsEnabled = true;
            parentWindow.b_deleteselected.IsEnabled = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            if (tb_busride_id.Text == "" || tb_traveller.Text == "" || tb_seats.Text == "")
            {
                MessageBox.Show("Empty fields are not allowed.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            

            BookingObject newObject = new BookingObject(Int32.Parse(tb_busride_id.Text), tb_traveller.Text, Int32.Parse(tb_seats.Text));

            if (DBConnection.CheckIfBookingAllowed(newObject))
            {

                if (newbooking)
                {
                    DBConnection.InsertBooking(newObject);
                }
                else
                {
                    DBConnection.UpdateBooking(newObject, oldObject);
                }

                parentWindow.parentWindow.UpdateAllChain();

                Close();
            }
            else
            {
                MessageBox.Show("Inte tillräckligt många fria platser på busriden.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //TODO: check if ok
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_change_busride_id_Click(object sender, RoutedEventArgs e)
        {
            b_change_busride_id.IsEnabled = false;
            b_change_traveller.IsEnabled = false;
            chooseBusrideWindow = new ChooseBusrideWindow(this);
            chooseBusrideWindow.ShowDialog();
        }

        private void b_change_traveller_Click(object sender, RoutedEventArgs e)
        {
            b_change_busride_id.IsEnabled = false;
            b_change_traveller.IsEnabled = false;
            chooseTravellerWindow = new ChooseTravellerWindow(this);
            chooseTravellerWindow.ShowDialog();
        }
    }
}
