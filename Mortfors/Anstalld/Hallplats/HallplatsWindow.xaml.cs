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
    /// Interaction logic for HallplatsWindow.xaml
    /// </summary>
    public partial class HallplatsWindow : Window
    {
        AnstalldWindow parent;

        public HallplatsWindow(AnstalldWindow _parent)
        {
            InitializeComponent();
            parent = _parent;
        }

        void HallplatsWindow_Closing(object sender, CancelEventArgs e)
        {
            parent.b_hanterahallplatser.IsEnabled = true;
        }

        private void b_forra_Click(object sender, RoutedEventArgs e)
        {

        }

        private void b_nasta_Click(object sender, RoutedEventArgs e)
        {

        }

        private void b_uppdatera_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
