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

namespace Mortfors.Employee.Employees
{
    /// <summary>
    /// Interaction logic for ChangeEmployeeWindow.xaml
    /// </summary>
    public partial class ChangeEmployeeWindow : Window
    {
        public HandleEmployeeWindow parentWindow;
        readonly EmployeeObject oldObject;
        bool newemployee;

        public ChangeEmployeeWindow(HandleEmployeeWindow _parent)
        {
            InitializeComponent();
            Title = "New Employee - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newemployee = true;
        }

        public ChangeEmployeeWindow(HandleEmployeeWindow _parent, EmployeeObject _oldObject)
        {
            InitializeComponent();
            Title = "Edit Employee - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_social_security_nr.Text = oldObject.personNummer;
            pb_password.Password = "";
            cb_admin.IsChecked = oldObject.isAdmin;
            tb_name.Text = oldObject.name;
            tb_address.Text = oldObject.address;
            tb_home_phone.Text = oldObject.phone;
            parentWindow = _parent;
            newemployee = false;
        }

        void ChangeEmployeeWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_new.IsEnabled = true;
            parentWindow.b_editselected.IsEnabled = true;
            parentWindow.b_deleteselected.IsEnabled = true;
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            if (tb_social_security_nr.Text == "" || pb_password.Password == "" || tb_name.Text == "" || tb_address.Text == "" || tb_home_phone.Text == "")
            {
                MessageBox.Show("Empty fields are not allowed.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            EmployeeObject newObject = new EmployeeObject(tb_social_security_nr.Text, SimpleHash.GenerateHashedPassword(tb_social_security_nr.Text, pb_password.Password), (bool)cb_admin.IsChecked, tb_name.Text, tb_address.Text, tb_home_phone.Text);

            if (newemployee)
            {
                DBConnection.InsertEmployee(newObject);
            }
            else
            {
                DBConnection.UpdateEmployee(newObject, oldObject);
            }

            parentWindow.parentWindow.UpdateAllChain();

            Close();
            //TODO: check if ok
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

