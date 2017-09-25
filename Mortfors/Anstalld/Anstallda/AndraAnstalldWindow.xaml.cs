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
    /// Interaction logic for AndraAnstalldWindow.xaml
    /// </summary>
    public partial class AndraAnstalldWindow : Window
    {
        public HanteraAnstalldWindow parentWindow;
        readonly AnstalldObject oldObject;
        bool newanstalld;

        public AndraAnstalldWindow(HanteraAnstalldWindow _parent)
        {
            InitializeComponent();
            Title = "Ny Anställd - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newanstalld = true;
        }

        public AndraAnstalldWindow(HanteraAnstalldWindow _parent, AnstalldObject _oldObject)
        {
            InitializeComponent();
            Title = "Redigera Anställd - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_pers_nr.Text = oldObject.personNummer;
            pb_losenord.Password = "";
            cb_admin.IsChecked = oldObject.isAdmin;
            tb_namn.Text = oldObject.namn;
            tb_adress.Text = oldObject.adress;
            tb_hem_telefon.Text = oldObject.telefon;
            parentWindow = _parent;
            newanstalld = false;
        }

        void AndraAnstalldWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_ny.IsEnabled = true;
            parentWindow.b_redigeramarkerad.IsEnabled = true;
            parentWindow.b_tabortmarkerad.IsEnabled = true;
        }

        private void b_spara_Click(object sender, RoutedEventArgs e)
        {
            if (tb_pers_nr.Text == "" || pb_losenord.Password == "" || tb_namn.Text == "" || tb_adress.Text == "" || tb_hem_telefon.Text == "")
            {
                MessageBox.Show("Tomma fält är inte tillåtna.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            AnstalldObject newObject = new AnstalldObject(tb_pers_nr.Text, SimpleHash.GenerateHashedPassword(tb_pers_nr.Text, pb_losenord.Password), (bool)cb_admin.IsChecked, tb_namn.Text, tb_adress.Text, tb_hem_telefon.Text);

            if (newanstalld)
            {
                DBConnection.InsertAnstalld(newObject);
            }
            else
            {
                DBConnection.UpdateAnstalld(newObject, oldObject);
            }

            parentWindow.parentWindow.UpdateAllChain();

            Close();
            //TODO: check if ok
        }

        private void b_avbryt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

