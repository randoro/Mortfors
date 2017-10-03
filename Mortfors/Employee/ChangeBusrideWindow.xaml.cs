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

namespace Mortfors.Employee
{
    /// <summary>
    /// Interaction logic for ChangeBusrideWindow.xaml
    /// </summary>
    public partial class ChangeBusrideWindow : Window
    {
        public EmployeeWindow parentWindow;
        readonly BusrideObject oldObject;
        bool newbusride;
        ChooseStationWindow chooseStationWindow;
        ChooseEmployeeWindow chooseEmployeeWindow;

        public string departure_address;
        public string departure_city;
        public string departure_country;
        
        public string arrival_address;
        public string arrival_city;
        public string arrival_country;

        public ChangeBusrideWindow(EmployeeWindow _parent)
        {
            InitializeComponent();
            Title = "New Busride - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newbusride = true;
        }

        public ChangeBusrideWindow(EmployeeWindow _parent, BusrideObject _oldObject)
        {
            InitializeComponent();
            Title = "Edit Busride - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_busride_id.Text = oldObject.busride_id.ToString();
            tb_departure.Text = oldObject.departure_address+", "+ oldObject.departure_city+", "+oldObject.departure_country;
            dtp_departuredate.Value = oldObject.departure_date;
            tb_arrival.Text = oldObject.arrival_address + ", " + oldObject.arrival_city + ", " + oldObject.arrival_country;
            dtp_arrivaldate.Value = oldObject.arrival_date;
            tb_cost.Text = oldObject.cost.ToString();
            tb_max_seats.Text = oldObject.max_seats.ToString();
            tb_driver_id.Text = (oldObject.driver_id == null) ? "" : oldObject.driver_id.ToString();

            departure_address = oldObject.departure_address;
            departure_city = oldObject.departure_city;
            departure_country = oldObject.departure_country;

            arrival_address = oldObject.arrival_address;
            arrival_city = oldObject.arrival_city;
            arrival_country = oldObject.arrival_country;
            

            parentWindow = _parent;
            newbusride = false;
        }

        void ChangeBusrideWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_newresa.IsEnabled = true;
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
            if (tb_busride_id.Text == "" || departure_address == "" || departure_city == "" || departure_country == "" || dtp_departuredate.Value == null || arrival_address == "" || arrival_city == "" || arrival_country == "" || dtp_arrivaldate.Value == null || tb_cost.Text == "" || tb_max_seats.Text == "")
            {
                MessageBox.Show("Empty fields are not allowed. (Förutom Driver ID)", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            BusrideObject newObject = new BusrideObject(Int32.Parse(tb_busride_id.Text), departure_address, departure_city, departure_country, (DateTime)dtp_departuredate.Value, arrival_address, arrival_city, arrival_country, (DateTime)dtp_arrivaldate.Value, Int32.Parse(tb_cost.Text), Int32.Parse(tb_max_seats.Text), tb_driver_id.Text);

            if (newbusride)
            {
                DBConnection.InsertBusride(newObject);
            }
            else
            {
                DBConnection.UpdateBusride(newObject, oldObject);
            }

            parentWindow.UpdateAllChain();

            Close();
            //TODO: check if ok
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        

        private void b_change_departure_Click(object sender, RoutedEventArgs e)
        {
            b_change_departure.IsEnabled = false;
            b_change_arrival.IsEnabled = false;
            b_change_driver_id.IsEnabled = false;
            chooseStationWindow = new ChooseStationWindow(this, true);
            chooseStationWindow.ShowDialog();
        }

        private void b_change_arrival_Click(object sender, RoutedEventArgs e)
        {
            b_change_departure.IsEnabled = false;
            b_change_arrival.IsEnabled = false;
            b_change_driver_id.IsEnabled = false;
            chooseStationWindow = new ChooseStationWindow(this, false);
            chooseStationWindow.ShowDialog();
        }

        private void b_change_driver_id_Click(object sender, RoutedEventArgs e)
        {
            b_change_departure.IsEnabled = false;
            b_change_arrival.IsEnabled = false;
            b_change_driver_id.IsEnabled = false;
            chooseEmployeeWindow = new ChooseEmployeeWindow(this);
            chooseEmployeeWindow.ShowDialog();
        }
    }
}
