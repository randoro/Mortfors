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

namespace Mortfors.Resenar
{
    /// <summary>
    /// Interaction logic for ResenarWindow.xaml
    /// </summary>
    public partial class ResenarWindow : Window
    {
        AndraResenarBokningWindow andraResenarBokningWindow;
        HanteraMinaBokningarWindow hanteraMinaBokningarWindow;
        AndraMinaUppgifterWindow andraMinaUppgifterWindow;

        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public ResenarWindow()
        {
            InitializeComponent();
            this.Title = "Mortfors - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateBussResor();

        }
        

        public void UpdateBussResor()
        {
            count = DBConnection.CountBussresor();
            lv_bussresor.ItemsSource = DBConnection.SelectBussresor(limit, offset);
            l_visar.Content = "Visar " + offset + " - " + (offset + limit) + " av " + count + ".";
            DisableButtons();
        }

        public void UpdateAllChain()
        {
            this.Title = "Mortfors - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateBussResor();
            if (hanteraMinaBokningarWindow != null && hanteraMinaBokningarWindow.Visibility == Visibility.Visible)
            {
                hanteraMinaBokningarWindow.UpdateBokningar();
            }
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

        private void b_bokamarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_bussresor.SelectedItem != null)
            {

                if (DBConnection.CheckIfHasBokning(((BussresaObject)lv_bussresor.SelectedItem).bussresa_id, ((ResenarObject)Authenticator.currentUser).email))
                {
                    MessageBox.Show("Du har redan en bokning på den här bussresan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                b_bokamarkerad.IsEnabled = false;
                b_redigeramarkerad.IsEnabled = false;
                andraResenarBokningWindow = new AndraResenarBokningWindow(this, (BussresaObject)lv_bussresor.SelectedItem);
                andraResenarBokningWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void b_redigeramarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_bussresor.SelectedItem != null)
            {
                if(!DBConnection.CheckIfHasBokning(((BussresaObject)lv_bussresor.SelectedItem).bussresa_id, ((ResenarObject)Authenticator.currentUser).email))
                {
                    MessageBox.Show("Du har ingen bokning på den här bussresan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                BokningObject finderObject = new BokningObject(((BussresaObject)lv_bussresor.SelectedItem).bussresa_id, ((ResenarObject)Authenticator.currentUser).email, -1);
                if (DBConnection.DeleteBokning(finderObject) > 0)
                {
                    UpdateAllChain();
                }
                

            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        

        private void b_minabokningar_Click(object sender, RoutedEventArgs e)
        {
            hanteraMinaBokningarWindow = new HanteraMinaBokningarWindow(this);
            hanteraMinaBokningarWindow.Show();
            b_minabokningar.IsEnabled = false;
        }

        private void b_minauppgifter_Click(object sender, RoutedEventArgs e)
        {
            b_minauppgifter.IsEnabled = false;
            andraMinaUppgifterWindow = new AndraMinaUppgifterWindow(this, ((ResenarObject)Authenticator.currentUser));
            andraMinaUppgifterWindow.Show();
            
        }

        
    }
}
