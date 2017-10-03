using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mortfors.SQLObject;
using Mortfors.Login;
using Mortfors.Resenar;

namespace Mortfors
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RegistreraResenarWindow registreraResenarWindow;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

           SimpleHash.HashTest();
            
        }

        private void b_login_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Tick += timer_Tick;
            dispatcherTimer.Start();
            b_login.IsEnabled = false;
            
            TryLogin(tb_username.Text, pb_password.Password);
            
        }

        private void b_registrera_Click(object sender, RoutedEventArgs e)
        {
            b_registrera.IsEnabled = false;
            registreraResenarWindow = new RegistreraResenarWindow(this);
            registreraResenarWindow.ShowDialog();
        }


        void timer_Tick(object sender, System.EventArgs e)
        {
            b_login.IsEnabled = true;
            dispatcherTimer.Stop();
        }

        public void TryLogin(string username, string password)
        {
            l_errormessage.Content = "";
            if (Authenticator.Login(username, password))
            {
                if (Authenticator.currentUser.GetType() == typeof(ResenarObject))
                {
                    ResenarWindow resenarWindow = new ResenarWindow();
                    resenarWindow.Show();
                    this.Close();
                }
                else if (Authenticator.currentUser.GetType() == typeof(AnstalldObject))
                {
                    AnstalldWindow anstalldWindow = new AnstalldWindow();
                    anstalldWindow.Show();
                    this.Close();
                }
            }
            else
            {
                l_errormessage.Content = Authenticator.errorMessage;
            }
        }

        private void b_cheat1_Click(object sender, RoutedEventArgs e)
        {
            TryLogin("199301123395", "test123");
        }

        private void b_cheat2_Click(object sender, RoutedEventArgs e)
        {
            TryLogin("199301123394", "test123");
        }

        private void b_cheat3_Click(object sender, RoutedEventArgs e)
        {
            TryLogin("randoro93@gmail.com", "test123");
        }

        
    }
}
