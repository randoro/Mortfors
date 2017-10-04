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

namespace Mortfors.Employee.Travellers
{
    /// <summary>
    /// Interaction logic for ChangeTraveller.xaml
    /// </summary>
    public partial class ChangeTravellerWindow : Window
    {
        public HandleTravellerWindow parentWindow;
        readonly TravellerObject oldObject;
        bool newtraveller;

        public ChangeTravellerWindow(HandleTravellerWindow _parent)
        {
            InitializeComponent();
            Title = "New Traveller - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newtraveller = true;
        }

        public ChangeTravellerWindow(HandleTravellerWindow _parent, TravellerObject _oldObject)
        {
            InitializeComponent();
            Title = "Edit Traveller - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_email.Text = oldObject.email;
            pb_password.Password = "";
            tb_name.Text = oldObject.name;
            tb_address.Text = oldObject.address;
            tb_phone.Text = oldObject.phone;
            parentWindow = _parent;
            newtraveller = false;
        }

        void ChangeTravellerWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_new.IsEnabled = true;
            parentWindow.b_editselected.IsEnabled = true;
            parentWindow.b_deleteselected.IsEnabled = true;
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            if (tb_email.Text == "" || pb_password.Password == "" || tb_name.Text == "" || tb_address.Text == "" || tb_phone.Text == "")
            {
                MessageBox.Show("Empty fields are not allowed.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            TravellerObject newObject = new TravellerObject(tb_email.Text, SimpleHash.GenerateHashedPassword(tb_email.Text, pb_password.Password), tb_name.Text, tb_address.Text, tb_phone.Text);
            int rowsChanged = -1;

            if (newtraveller)
            {
                rowsChanged = DBConnection.InsertTraveller(newObject);
            }
            else
            {
                rowsChanged = DBConnection.UpdateTraveller(newObject, oldObject);
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
