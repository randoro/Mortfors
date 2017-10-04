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

namespace Mortfors.Employee
{
    /// <summary>
    /// Interaction logic for ChooseEmployeeWindow.xaml
    /// </summary>
    public partial class ChooseEmployeeWindow : Window
    {
        public ChangeBusrideWindow parentWindow;
        List<EmployeeObject> employeeObject;
        

        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public ChooseEmployeeWindow(ChangeBusrideWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            employeeObject = new List<EmployeeObject>();
            this.Title = "Choose Driver - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateStations();

        }

        void ChooseEmployeeWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_change_departure.IsEnabled = true;
            parentWindow.b_change_arrival.IsEnabled = true;
            parentWindow.b_change_driver_id.IsEnabled = true;
        }

        public void UpdateStations()
        {

            count = DBConnection.CountStations();
            employeeObject = DBConnection.SelectChafforer(limit, offset);
            lv_lista.ItemsSource = employeeObject;
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

            UpdateStations();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateStations();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateStations();
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_choose_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                EmployeeObject selected = (EmployeeObject)lv_lista.SelectedItem;
                parentWindow.tb_driver_id.Text = selected.personNummer;
               
                Close();
            }
            else
            {
                MessageBox.Show("Nothing selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
