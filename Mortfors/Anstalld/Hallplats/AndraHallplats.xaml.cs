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

namespace Mortfors
{
    /// <summary>
    /// Interaction logic for AndraHallplats.xaml
    /// </summary>
    public partial class AndraHallplats : Window
    {
        HallplatsWindow parent;
        HallplatsObject oldObject;

        public AndraHallplats(HallplatsWindow _parent)
        {
            InitializeComponent();
            Title = "Ny Hållplats - " + Authenticator.GetUserInfo();
            parent = _parent;
        }

        public AndraHallplats(HallplatsWindow _parent, HallplatsObject _oldObject)
        {
            InitializeComponent();
            Title = "Redigera Hållplats - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            parent = _parent;
        }

        void AndraHallplatsWindow_Closing(object sender, CancelEventArgs e)
        {
            parent.b_nyhallplats.IsEnabled = true;
            parent.b_redigeramarkerad.IsEnabled = true;
            parent.b_tabortmarkerad.IsEnabled = true;
        }

        private void b_spara_Click(object sender, RoutedEventArgs e)
        {
            //Fix Check if empty!
            oldObject = new HallplatsObject(tb_gatuadress.Text, tb_stad.Text, tb_land.Text);


            bool boolFound = false;
            string errorMessage = "";
            DBConnection.ConnectInsertHallplats(oldObject, out boolFound, out errorMessage);
            Close(); //Check if ok?
        }

        private void b_avbryt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
