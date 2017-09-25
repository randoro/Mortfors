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
            lv_resenarer.ItemsSource = resenarObject;
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

        private void b_nyhallplats_Click(object sender, RoutedEventArgs e)
        {
            andraResenar = new AndraResenarWindow(this);
            andraResenar.Show();
            b_nyresenar.IsEnabled = false;
            b_redigeramarkerad.IsEnabled = false;
            b_tabortmarkerad.IsEnabled = false;

        }

        private void b_redigeramarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_resenarer.SelectedItem != null)
            {
                andraResenar = new AndraResenarWindow(this, (ResenarObject)lv_resenarer.SelectedItem);
                andraResenar.Show();
                b_nyresenar.IsEnabled = false;
                b_redigeramarkerad.IsEnabled = false;
                b_tabortmarkerad.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_tabortmarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_resenarer.SelectedItem != null)
            {
                if(DBConnection.DeleteResenar((ResenarObject)lv_resenarer.SelectedItem) > 0)
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
