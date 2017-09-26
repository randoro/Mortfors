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

namespace Mortfors.Anstalld.Resenarer
{
    /// <summary>
    /// Interaction logic for HanteraAnstalld.xaml
    /// </summary>
    public partial class HanteraResenarWindow : Window
    {
        public AnstalldWindow parentWindow;
        AndraResenarWindow andraResenar;
        List<ResenarObject> resenarObject;


        const int limit = 2;
        public int offset = 0;
        public int count = 0;

        public HanteraResenarWindow(AnstalldWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            resenarObject = new List<ResenarObject>();
            this.Title = "Hantera Resenärer - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateResenarer();

        }

        void HanteraResenarWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_hanteraresenarer.IsEnabled = true;
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

        private void b_ny_Click(object sender, RoutedEventArgs e)
        {
            b_ny.IsEnabled = false;
            b_redigeramarkerad.IsEnabled = false;
            b_tabortmarkerad.IsEnabled = false;
            andraResenar = new AndraResenarWindow(this);
            andraResenar.ShowDialog();
        }

        private void b_redigeramarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                b_ny.IsEnabled = false;
                b_redigeramarkerad.IsEnabled = false;
                b_tabortmarkerad.IsEnabled = false;
                andraResenar = new AndraResenarWindow(this, (ResenarObject)lv_lista.SelectedItem);
                andraResenar.ShowDialog();
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
                if(DBConnection.DeleteResenar((ResenarObject)lv_lista.SelectedItem) > 0)
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
