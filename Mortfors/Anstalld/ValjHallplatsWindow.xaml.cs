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
    /// Interaction logic for ValjHallplatsWindow.xaml
    /// </summary>
    public partial class ValjHallplatsWindow : Window
    {
        public AndraBussresaWindow parentWindow;
        List<HallplatsObject> hallplatsObject;

        bool isAvgang;


        const int limit = 2;
        public int offset = 0;
        public int count = 0;

        public ValjHallplatsWindow(AndraBussresaWindow _parent, bool _isAvgang)
        {
            InitializeComponent();
            parentWindow = _parent;
            isAvgang = _isAvgang;
            hallplatsObject = new List<HallplatsObject>();
            this.Title = "Välj Hållplats - Välkommen " + Authenticator.GetUserInfo() + ".";
            UpdateHallplatser();

        }

        void ValjHallplatsWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_andra_avgang.IsEnabled = true;
            parentWindow.b_andra_ankomst.IsEnabled = true;
            parentWindow.b_andra_chaffor_id.IsEnabled = true;
        }

        public void UpdateHallplatser()
        {

            count = DBConnection.CountHallplatser();
            hallplatsObject = DBConnection.SelectHallplatser(limit, offset);
            lv_lista.ItemsSource = hallplatsObject;
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
                HallplatsObject selected = (HallplatsObject)lv_lista.SelectedItem;

                if (isAvgang)
                {
                    parentWindow.avgangsObject = selected;
                    parentWindow.tb_avgang.Text = selected.gatu_adress + ", " + selected.stad + ", " + selected.land;
                }
                else
                {
                    parentWindow.ankomstObject = selected;
                    parentWindow.tb_ankomst.Text = selected.gatu_adress + ", " + selected.stad + ", " + selected.land;
                }
                Close();
            }
            else
            {
                MessageBox.Show("Inget markerat.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
