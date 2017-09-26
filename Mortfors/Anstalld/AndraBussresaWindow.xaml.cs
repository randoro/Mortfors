using Mortfors.Login;
using Mortfors.SQLObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AndraBussresaWindow.xaml
    /// </summary>
    public partial class AndraBussresaWindow : Window
    {
        public AnstalldWindow parentWindow;
        readonly BussresaObject oldObject;
        bool newbussresa;
        ValjHallplatsWindow valjHallplatsWindow;
        ValjAnstalldWindow valjAnstalldWindow;

        public HallplatsObject avgangsObject;
        public HallplatsObject ankomstObject;

        public AndraBussresaWindow(AnstalldWindow _parent)
        {
            InitializeComponent();
            Title = "Ny Bussresa - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newbussresa = true;
        }

        public AndraBussresaWindow(AnstalldWindow _parent, BussresaObject _oldObject)
        {
            InitializeComponent();
            Title = "Redigera Bussresa - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_bussresa_id.Text = oldObject.bussresa_id.ToString();
            tb_avgang.Text = oldObject.avgangs_adress+", "+ oldObject.avgangs_stad+", "+oldObject.avgangs_land;
            dtp_avgangsdatum.Value = oldObject.avgangs_datum;
            tb_ankomst.Text = oldObject.ankomst_adress + ", " + oldObject.ankomst_stad + ", " + oldObject.ankomst_land;
            dtp_ankomstdatum.Value = oldObject.ankomst_datum;
            tb_kostnad.Text = oldObject.kostnad.ToString();
            tb_max_platser.Text = oldObject.max_platser.ToString();
            tb_chaffor_id.Text = (oldObject.chaffor_id == null) ? "" : oldObject.chaffor_id.ToString();

            avgangsObject = new HallplatsObject(oldObject.avgangs_adress, oldObject.avgangs_stad, oldObject.avgangs_land);
            ankomstObject = new HallplatsObject(oldObject.ankomst_adress, oldObject.ankomst_stad, oldObject.ankomst_land);

            parentWindow = _parent;
            newbussresa = false;
        }

        void AndraBussresaWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_nyresa.IsEnabled = true;
            parentWindow.b_redigeramarkerad.IsEnabled = true;
            parentWindow.b_tabortmarkerad.IsEnabled = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void b_spara_Click(object sender, RoutedEventArgs e)
        {
            if (tb_bussresa_id.Text == "" || avgangsObject == null || tb_avgang.Text == "" || dtp_avgangsdatum.Value == null || ankomstObject == null || tb_ankomst.Text == "" || dtp_ankomstdatum.Value == null || tb_kostnad.Text == "" || tb_max_platser.Text == "")
            {
                MessageBox.Show("Tomma fält är inte tillåtna. (Förutom Chafför ID)", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            BussresaObject newObject = new BussresaObject(Int32.Parse(tb_bussresa_id.Text), avgangsObject.gatu_adress, avgangsObject.stad, avgangsObject.land, (DateTime)dtp_avgangsdatum.Value, ankomstObject.gatu_adress, ankomstObject.stad, ankomstObject.land, (DateTime)dtp_ankomstdatum.Value, Int32.Parse(tb_kostnad.Text), Int32.Parse(tb_max_platser.Text), tb_chaffor_id.Text);

            if (newbussresa)
            {
                DBConnection.InsertBussresa(newObject);
            }
            else
            {
                DBConnection.UpdateBussresa(newObject, oldObject);
            }

            parentWindow.UpdateAllChain();

            Close();
            //TODO: check if ok
        }

        private void b_avbryt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        

        private void b_andra_avgang_Click(object sender, RoutedEventArgs e)
        {
            b_andra_avgang.IsEnabled = false;
            b_andra_ankomst.IsEnabled = false;
            b_andra_chaffor_id.IsEnabled = false;
            valjHallplatsWindow = new ValjHallplatsWindow(this, true);
            valjHallplatsWindow.ShowDialog();
        }

        private void b_andra_ankomst_Click(object sender, RoutedEventArgs e)
        {
            b_andra_avgang.IsEnabled = false;
            b_andra_ankomst.IsEnabled = false;
            b_andra_chaffor_id.IsEnabled = false;
            valjHallplatsWindow = new ValjHallplatsWindow(this, false);
            valjHallplatsWindow.ShowDialog();
        }

        private void b_andra_chaffor_id_Click(object sender, RoutedEventArgs e)
        {
            b_andra_avgang.IsEnabled = false;
            b_andra_ankomst.IsEnabled = false;
            b_andra_chaffor_id.IsEnabled = false;
            valjAnstalldWindow = new ValjAnstalldWindow(this);
            valjAnstalldWindow.ShowDialog();
        }
    }
}
