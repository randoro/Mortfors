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

namespace Mortfors.Anstalld.Bokningar
{
    /// <summary>
    /// Interaction logic for ValjResenarWindow.xaml
    /// </summary>
    public partial class ValjResenarWindow : Window
    {
        public AndraBokningWindow parentWindow;
        List<ResenarObject> resenarObject;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public ValjResenarWindow(AndraBokningWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            resenarObject = new List<ResenarObject>();
            this.Title = "Välj Resenär - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateResenarer();

        }

        void ValjResenarWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_andra_bussresa_id.IsEnabled = true;
            parentWindow.b_andra_resenar.IsEnabled = true;
        }

        public void UpdateResenarer()
        {

            count = DBConnection.CountResenarer();
            resenarObject = DBConnection.SelectResenarer(limit, offset);
            lv_lista.ItemsSource = resenarObject;
            l_visar.Content = "Visar " + offset + " - " + (offset + limit) + " av " + count + ".";
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

            UpdateResenarer();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateResenarer();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateResenarer();
        }

        private void b_avbryt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_valj_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                ResenarObject selected = (ResenarObject)lv_lista.SelectedItem;
                parentWindow.tb_resenar.Text = selected.email;

                Close();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

