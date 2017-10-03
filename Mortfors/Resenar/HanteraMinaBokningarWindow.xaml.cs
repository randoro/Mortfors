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

namespace Mortfors.Resenar
{
    /// <summary>
    /// Interaction logic for HanteraMinaBokningarWindow.xaml
    /// </summary>
    public partial class HanteraMinaBokningarWindow : Window
    {
        public ResenarWindow parentWindow;
        List<MinBokningObject> bokningObject;


        const int limit = 10;
        public int offset = 0;
        public int count = 0;

        public HanteraMinaBokningarWindow(ResenarWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            bokningObject = new List<MinBokningObject>();
            this.Title = "Hantera Bokningar - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateBokningar();

        }

        void HanteraMinaBokningarWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_minabokningar.IsEnabled = true;
        }

        public void UpdateBokningar()
        {
            this.Title = "Hantera Bokningar - Välkommen " + Authenticator.GetUserInfo() + ".";
            count = DBConnection.CountBokningar();
            bokningObject = DBConnection.SelectBokningarJoinBussresa(((ResenarObject)Authenticator.currentUser).email, limit, offset);
            lv_lista.ItemsSource = bokningObject;
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

            UpdateBokningar();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateBokningar();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateBokningar();
        }

        private void b_tabortmarkerad_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                MinBokningObject minBokning = (MinBokningObject)lv_lista.SelectedItem;

                BokningObject bokning = new BokningObject(minBokning.bussresa.bussresa_id, minBokning.resenar, minBokning.antal_platser);
                if (DBConnection.DeleteBokning(bokning) > 0)
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
