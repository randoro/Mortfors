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

namespace Mortfors.Anstalld.Anstallda
{
    /// <summary>
    /// Interaction logic for HanteraAnstalldWindow.xaml
    /// </summary>
    public partial class HanteraAnstalldWindow : Window
    {
        public AnstalldWindow parentWindow;
        AndraAnstalldWindow andraAnstalld;
        List<AnstalldObject> anstalldObject;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public HanteraAnstalldWindow(AnstalldWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            anstalldObject = new List<AnstalldObject>();
            this.Title = "Hantera Anstallda - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateAnstallda();

        }

        void HanteraAnstalldWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_hanteraanstallda.IsEnabled = true;
        }

        public void UpdateAnstallda()
        {
            count = DBConnection.CountAnstallda();
            anstalldObject = DBConnection.SelectAnstallda(limit, offset);
            lv_lista.ItemsSource = anstalldObject;
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

            UpdateAnstallda();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateAnstallda();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateAnstallda();
        }

        private void b_ny_Click(object sender, RoutedEventArgs e)
        {
            b_ny.IsEnabled = false;
            b_redigeramarkerad.IsEnabled = false;
            b_tabortmarkerad.IsEnabled = false;
            andraAnstalld = new AndraAnstalldWindow(this);
            andraAnstalld.ShowDialog();
        }

        private void b_redigeramarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                b_ny.IsEnabled = false;
                b_redigeramarkerad.IsEnabled = false;
                b_tabortmarkerad.IsEnabled = false;
                andraAnstalld = new AndraAnstalldWindow(this, (AnstalldObject)lv_lista.SelectedItem);
                andraAnstalld.ShowDialog();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_tabortmarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                if (DBConnection.DeleteAnstalld((AnstalldObject)lv_lista.SelectedItem) > 0)
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
