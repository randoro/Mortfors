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
        public AnstalldWindow()
        {
            InitializeComponent();

            List<Bussresa> items = new List<Bussresa>();
            items.Add(new Bussresa(0, "test123test123test123test123test123test123", "test123test123test123test123test123test123test123", "test123test123test123test123test123test123", new DateTime(), "test123test123test123test123test123", "test123test123test123test123test123", "test123test123test123test123test123", new DateTime(), 0, 0, "test123test123test123test123test123"));
            items.Add(new Bussresa(0, "test123", "test123", "test123", new DateTime(), "test123", "test123", "test123", new DateTime(), 0, 0, "test123"));
            items.Add(new Bussresa(0, "test123", "test123", "test123", new DateTime(), "test123", "test123", "test123", new DateTime(), 0, 0, "test123"));
            lv_bussresor.ItemsSource = items;
            bool found = false;
            string error = "";
            lv_bussresor.ItemsSource = DBConnection.ConnectSelectBussResor(2, 2, out found, out error);

        }
    }
}
