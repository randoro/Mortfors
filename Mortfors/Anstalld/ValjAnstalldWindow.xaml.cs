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

namespace Mortfors.Anstalld
{
    /// <summary>
    /// Interaction logic for ValjAnstalldWindow.xaml
    /// </summary>
    public partial class ValjAnstalldWindow : Window
    {
        public AndraBussresaWindow parentWindow;
        List<AnstalldObject> anstalldObject;
        

        const int limit = 2;
        public int offset = 0;
        public int count = 0;

        public ValjAnstalldWindow(AndraBussresaWindow _parent)
        {
            InitializeComponent();
            parentWindow = _parent;
            anstalldObject = new List<AnstalldObject>();
            this.Title = "Välj Chafför - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateHallplatser();

        }

        void ValjAnstalldWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_andra_avgang.IsEnabled = true;
            parentWindow.b_andra_ankomst.IsEnabled = true;
            parentWindow.b_andra_chaffor_id.IsEnabled = true;
        }

        public void UpdateHallplatser()
        {

            count = DBConnection.CountHallplatser();
            anstalldObject = DBConnection.SelectChafforer(limit, offset);
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

            UpdateHallplatser();
        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {
            offset += limit;
            UpdateHallplatser();
        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {
            UpdateHallplatser();
        }

        private void b_avbryt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_valj_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lista.SelectedItem != null)
            {
                AnstalldObject selected = (AnstalldObject)lv_lista.SelectedItem;
                parentWindow.tb_chaffor_id.Text = selected.personNummer;
               
                Close();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
