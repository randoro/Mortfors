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
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class HandleStationWindow : Window
    {
        public EmployeeWindow parentWindow;
        ChangeStationWindow changeStation;
        List<StationObject> stationObject;

        
        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public HandleStationWindow(EmployeeWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            stationObject = new List<StationObject>();
            this.Title = "Handle Stations - Welcome " + Authenticator.GetUserInfo() + ".";
            UpdateStations();

        }

        void HandleStationWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_hanterastations.IsEnabled = true;
        }

        public void UpdateStations()
        {
            
            count = DBConnection.CountStations();
            stationObject = DBConnection.SelectStations(limit, offset);
            lv_lista.ItemsSource = stationObject;
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

        private void b_new_Click(object sender, RoutedEventArgs e)
        {
            b_new.IsEnabled = false;
            b_editselected.IsEnabled = false;
            b_deleteselected.IsEnabled = false;
            changeStation = new ChangeStationWindow(this);
            changeStation.ShowDialog();
        }

        private void b_editselected_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                b_new.IsEnabled = false;
                b_editselected.IsEnabled = false;
                b_deleteselected.IsEnabled = false;
                changeStation = new ChangeStationWindow(this, (StationObject)lv_lista.SelectedItem);
                changeStation.ShowDialog();
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
                if (DBConnection.DeleteStation((StationObject)lv_lista.SelectedItem) > 0)
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
