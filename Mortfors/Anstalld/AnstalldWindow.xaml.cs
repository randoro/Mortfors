using Mortfors.Anstalld.Anstallda;
using Mortfors.Anstalld.Hallplatser;
using Mortfors.Anstalld.Resenarer;
using Mortfors.Login;
using Mortfors.SQLObject;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AnstalldWindow.xaml
    /// </summary>
    public partial class AnstalldWindow : Window
    {


        HanteraHallplatsWindow hallplatsWindow;
        HanteraResenarWindow resenarWindow;
        HanteraAnstalldWindow anstalldWindow;

        const int limit = 2;
        public int offset = 0;
        public int count = 0;

        public AnstalldWindow()
        {
            InitializeComponent();
            SetPermissions();
            this.Title = "Mortfors - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateBussResor();

        }

        private void SetPermissions()
        {
            if (Authenticator.authenticated)
            {
                if (((AnstalldObject)Authenticator.currentUser).isAdmin)
                {
                    b_kormarkerad.IsEnabled = false;
                    b_avanmalkorningavmarkerad.IsEnabled = false;

                    b_nyresa.IsEnabled = true;
                    b_redigeramarkerad.IsEnabled = true;
                    b_tabortmarkerad.IsEnabled = true;
                    b_hanterahallplatser.IsEnabled = true;
                    b_hanteraresenarer.IsEnabled = true;
                    b_hanterabokningar.IsEnabled = true;
                    b_hanteraanstallda.IsEnabled = true;
                }
                else
                {
                    b_kormarkerad.IsEnabled = true;
                    b_avanmalkorningavmarkerad.IsEnabled = true;

                    b_nyresa.IsEnabled = false;
                    b_redigeramarkerad.IsEnabled = false;
                    b_tabortmarkerad.IsEnabled = false;
                    b_hanterahallplatser.IsEnabled = false;
                    b_hanteraresenarer.IsEnabled = false;
                    b_hanterabokningar.IsEnabled = false;
                    b_hanteraanstallda.IsEnabled = false;
                }
            }
        }

        public void UpdateBussResor()
        {
            count = DBConnection.CountBussResor();
            lv_bussresor.ItemsSource = DBConnection.SelectBussResor(limit, offset);
            l_visar.Content = "Visar "+ offset + " - "+ (offset+limit) + " av " + count + ".";
            DisableButtons();
        }

        public void UpdateAllChain()
        {
            UpdateBussResor();
            if (hallplatsWindow != null && hallplatsWindow.Visibility == Visibility.Visible)
            {
                hallplatsWindow.UpdateHallplatser();
            }
            if (resenarWindow != null && resenarWindow.Visibility == Visibility.Visible)
            {
                resenarWindow.UpdateResenarer();
            }
            if (anstalldWindow != null && anstalldWindow.Visibility == Visibility.Visible)
            {
                anstalldWindow.UpdateAnstallda();
            }
        }

        public void DisableButtons()
        {
            if(offset == 0)
            {
                b_forra.IsEnabled = false;
            }
            else
            {
                b_forra.IsEnabled = true;
            }

            if (offset+limit >= count)
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

            UpdateBussResor();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateBussResor();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateBussResor();
        }

        private void b_hanterahallplatser_Click(object sender, RoutedEventArgs e)
        {
            hallplatsWindow = new HanteraHallplatsWindow(this);
            hallplatsWindow.Show();
            b_hanterahallplatser.IsEnabled = false;
        }

        private void b_hanteraresenarer_Click(object sender, RoutedEventArgs e)
        {
            resenarWindow = new HanteraResenarWindow(this);
            resenarWindow.Show();
            b_hanteraresenarer.IsEnabled = false;
        }

        private void b_hanterabokningar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void b_hanteraanstallda_Click(object sender, RoutedEventArgs e)
        {
            anstalldWindow = new HanteraAnstalldWindow(this);
            anstalldWindow.Show();
            b_hanteraanstallda.IsEnabled = false;
        }
    }
}
