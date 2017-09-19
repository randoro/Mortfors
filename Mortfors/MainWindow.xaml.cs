using Npgsql;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mortfors
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SimpleHash.HashTest();



        //DBConnection.ConnectAndSelect("Timmermansgatan");
        }

        private void b_login_Click(object sender, RoutedEventArgs e)
        {
            Authenticator.Login(tb_username.Text, pb_password.Password);
        }
    }
}
