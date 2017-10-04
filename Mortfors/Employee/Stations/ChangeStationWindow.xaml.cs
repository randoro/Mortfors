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

namespace Mortfors.Employee.Stations
{
    /// <summary>
    /// Interaction logic for ChangeStation.xaml
    /// </summary>
    public partial class ChangeStationWindow : Window
    {
        public HandleStationWindow parentWindow;
        readonly StationObject oldObject;
        bool newstation;

        public ChangeStationWindow(HandleStationWindow _parent)
        {
            InitializeComponent();
            Title = "New Station - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newstation = true;
        }

        public ChangeStationWindow(HandleStationWindow _parent, StationObject _oldObject)
        {
            InitializeComponent();
            Title = "Edit Station - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_gatuaddress.Text = oldObject.street_address;
            tb_city.Text = oldObject.city;
            tb_country.Text = oldObject.country;
            parentWindow = _parent;
            newstation = false;
        }

        void ChangeStationWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_new.IsEnabled = true;
            parentWindow.b_editselected.IsEnabled = true;
            parentWindow.b_deleteselected.IsEnabled = true;
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            if(tb_gatuaddress.Text == "" || tb_city.Text == "" || tb_country.Text == "")
            {
                MessageBox.Show("Empty fields are not allowed.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            StationObject newObject = new StationObject(tb_gatuaddress.Text, tb_city.Text, tb_country.Text);
            int rowsChanged = -1;

            if(newstation)
            {
                rowsChanged = DBConnection.InsertStation(newObject);
            }
            else
            {
                rowsChanged = DBConnection.UpdateStation(newObject, oldObject);
            }

            if (rowsChanged > 0)
            {
                parentWindow.parentWindow.UpdateAllChain();
                Close();
            }
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
