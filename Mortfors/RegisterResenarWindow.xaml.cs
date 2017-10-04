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

namespace Mortfors
{
    /// <summary>
    /// Interaction logic for RegisterTravellerWindow.xaml
    /// </summary>
    public partial class RegisterTravellerWindow : Window
    {
        public MainWindow parentWindow;
        readonly TravellerObject oldObject;


        public RegisterTravellerWindow(MainWindow _parent)
        {
            InitializeComponent();
            Title = "Register as new traveller.";
            parentWindow = _parent;
        }

        void RegisterTravellerWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_register.IsEnabled = true;
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            if (tb_email.Text == "" || pb_password.Password == "" || pb_confirm_password.Password == "" || tb_name.Text == "" || tb_address.Text == "" || tb_phone.Text == "")
            {
                MessageBox.Show("Empty fields are not allowed.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(pb_password.Password == pb_confirm_password.Password))
            {
                MessageBox.Show("The passwords in the passwords fields are not the same.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            TravellerObject newObject = new TravellerObject(tb_email.Text, SimpleHash.GenerateHashedPassword(tb_email.Text, pb_password.Password), tb_name.Text, tb_address.Text, tb_phone.Text);



            if (DBConnection.InsertTraveller(newObject) > 0)
            {
                Authenticator.currentUser = newObject;
                parentWindow.TryLogin(tb_email.Text, pb_password.Password);
                Close();
            }
            
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
