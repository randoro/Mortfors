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

namespace Mortfors.Traveller
{
    /// <summary>
    /// Interaction logic for TravellerWindow.xaml
    /// </summary>
    public partial class TravellerWindow : Window
    {
        ChangeTravellerBookingWindow changeTravellerBookingWindow;
        HandleMyBookingsWindow hanteraMyBookingsWindow;
        ChangeMyInformationWindow changeMyInformationWindow;

        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public TravellerWindow()
        {
            InitializeComponent();
            this.Title = "Mortfors - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateBussResor();

        }
        

        public void UpdateBussResor()
        {
            count = DBConnection.CountBusrides();
            lv_busrides.ItemsSource = DBConnection.SelectBusrides(limit, offset);
            l_visar.Content = "Showing " + offset + " - " + (offset + limit) + " av " + count + ".";
            DisableButtons();
        }

        public void UpdateAllChain()
        {
            this.Title = "Mortfors - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateBussResor();
            if (hanteraMyBookingsWindow != null && hanteraMyBookingsWindow.Visibility == Visibility.Visible)
            {
                hanteraMyBookingsWindow.UpdateBookings();
            }
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

        private void b_bookselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_busrides.SelectedItem != null)
            {

                if (DBConnection.CheckIfHasBooking(((BusrideObject)lv_busrides.SelectedItem).busride_id, ((TravellerObject)Authenticator.currentUser).email))
                {
                    MessageBox.Show("Du har redan en booking på den här busriden.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                b_bookselected.IsEnabled = false;
                b_cancelselected.IsEnabled = false;
                changeTravellerBookingWindow = new ChangeTravellerBookingWindow(this, (BusrideObject)lv_busrides.SelectedItem);
                changeTravellerBookingWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_cancelselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_busrides.SelectedItem != null)
            {
                if(!DBConnection.CheckIfHasBooking(((BusrideObject)lv_busrides.SelectedItem).busride_id, ((TravellerObject)Authenticator.currentUser).email))
                {
                    MessageBox.Show("Du har ingen booking på den här busriden.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                BookingObject finderObject = new BookingObject(((BusrideObject)lv_busrides.SelectedItem).busride_id, ((TravellerObject)Authenticator.currentUser).email, -1);
                if (DBConnection.DeleteBooking(finderObject) > 0)
                {
                    UpdateAllChain();
                }
                

            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        

        private void b_mybookings_Click(object sender, RoutedEventArgs e)
        {
            hanteraMyBookingsWindow = new HandleMyBookingsWindow(this);
            hanteraMyBookingsWindow.Show();
            b_mybookings.IsEnabled = false;
        }

        private void b_myinformation_Click(object sender, RoutedEventArgs e)
        {
            b_myinformation.IsEnabled = false;
            changeMyInformationWindow = new ChangeMyInformationWindow(this, ((TravellerObject)Authenticator.currentUser));
            changeMyInformationWindow.Show();
            
        }

        
    }
}
