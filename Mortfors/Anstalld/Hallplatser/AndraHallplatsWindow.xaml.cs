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

namespace Mortfors.Anstalld.Hallplatser
{
    /// <summary>
    /// Interaction logic for AndraHallplats.xaml
    /// </summary>
    public partial class AndraHallplatsWindow : Window
    {
        public HanteraHallplatsWindow parentWindow;
        readonly HallplatsObject oldObject;
        bool newhallplats;

        public AndraHallplatsWindow(HanteraHallplatsWindow _parent)
        {
            InitializeComponent();
            Title = "Ny Hållplats - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newhallplats = true;
        }

        public AndraHallplatsWindow(HanteraHallplatsWindow _parent, HallplatsObject _oldObject)
        {
            InitializeComponent();
            Title = "Redigera Hållplats - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_gatuadress.Text = oldObject.gatu_adress;
            tb_stad.Text = oldObject.stad;
            tb_land.Text = oldObject.land;
            parentWindow = _parent;
            newhallplats = false;
        }

        void AndraHallplatsWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_nyhallplats.IsEnabled = true;
            parentWindow.b_redigeramarkerad.IsEnabled = true;
            parentWindow.b_tabortmarkerad.IsEnabled = true;
        }

        private void b_spara_Click(object sender, RoutedEventArgs e)
        {
            if(tb_gatuadress.Text == "" || tb_stad.Text == "" || tb_land.Text == "")
            {
                MessageBox.Show("Tomma fält är inte tillåtna.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            HallplatsObject newObject = new HallplatsObject(tb_gatuadress.Text, tb_stad.Text, tb_land.Text);

            if(newhallplats)
            {
                DBConnection.InsertHallplats(newObject);
            }
            else
            {
                DBConnection.UpdateHallplats(newObject, oldObject);
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
