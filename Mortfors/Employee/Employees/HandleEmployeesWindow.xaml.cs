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
    /// Interaction logic for HandleEmployeeWindow.xaml
    /// </summary>
    public partial class HandleEmployeeWindow : Window
    {
        public EmployeeWindow parentWindow;
        ChangeEmployeeWindow changeEmployee;
        List<EmployeeObject> employeeObject;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public HandleEmployeeWindow(EmployeeWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            employeeObject = new List<EmployeeObject>();
            this.Title = "Handle Employees - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateEmployees();

        }

        void HandleEmployeeWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_hanteraemployees.IsEnabled = true;
        }

        public void UpdateEmployees()
        {
            count = DBConnection.CountEmployees();
            employeeObject = DBConnection.SelectEmployees(limit, offset);
            lv_lista.ItemsSource = employeeObject;
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

            UpdateEmployees();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateEmployees();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateEmployees();
        }

        private void b_new_Click(object sender, RoutedEventArgs e)
        {
            b_new.IsEnabled = false;
            b_editselected.IsEnabled = false;
            b_deleteselected.IsEnabled = false;
            changeEmployee = new ChangeEmployeeWindow(this);
            changeEmployee.ShowDialog();
        }

        private void b_editselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                b_new.IsEnabled = false;
                b_editselected.IsEnabled = false;
                b_deleteselected.IsEnabled = false;
                changeEmployee = new ChangeEmployeeWindow(this, (EmployeeObject)lv_lista.SelectedItem);
                changeEmployee.ShowDialog();
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
                if (DBConnection.DeleteEmployee((EmployeeObject)lv_lista.SelectedItem) > 0)
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
