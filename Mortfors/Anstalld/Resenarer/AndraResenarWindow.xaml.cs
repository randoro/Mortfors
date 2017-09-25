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
    /// Interaction logic for AndraResenar.xaml
    /// </summary>
    public partial class AndraResenarWindow : Window
    {
        public HanteraResenarWindow parentWindow;
        readonly ResenarObject oldObject;
        bool newresenar;

        public AndraResenarWindow(HanteraResenarWindow _parent)
        {
            InitializeComponent();
            Title = "Ny Resenär - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newresenar = true;
        }

        public AndraResenarWindow(HanteraResenarWindow _parent, ResenarObject _oldObject)
        {
            InitializeComponent();
            Title = "Redigera Resenär - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_email.Text = oldObject.email;
            pb_losenord.Password = "";
            tb_namn.Text = oldObject.namn;
            tb_adress.Text = oldObject.adress;
            tb_telefon.Text = oldObject.telefon;
            parentWindow = _parent;
            newresenar = false;
        }

        void AndraResenarWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_nyresenar.IsEnabled = true;
            parentWindow.b_redigeramarkerad.IsEnabled = true;
            parentWindow.b_tabortmarkerad.IsEnabled = true;
        }

        private void b_spara_Click(object sender, RoutedEventArgs e)
        {
            if (tb_email.Text == "" || pb_losenord.Password == "" || tb_namn.Text == "" || tb_adress.Text == "" || tb_telefon.Text == "")
            {
                MessageBox.Show("Tomma fält är inte tillåtna.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            ResenarObject newObject = new ResenarObject(tb_email.Text, SimpleHash.GenerateHashedPassword(tb_email.Text, pb_losenord.Password), tb_namn.Text, tb_adress.Text, tb_telefon.Text);

            if (newresenar)
            {
                DBConnection.InsertResenar(newObject);
            }
            else
            {
                DBConnection.UpdateResenar(newObject, oldObject);
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
