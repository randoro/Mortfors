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
using Mortfors.Traveller;

namespace Mortfors
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RegisterTravellerWindow registerTravellerWindow;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void b_login_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Tick += timer_Tick;
            dispatcherTimer.Start();
            b_login.IsEnabled = false;
            
            TryLogin(tb_username.Text, pb_password.Password);
            
        }

        private void b_register_Click(object sender, RoutedEventArgs e)
        {
            b_register.IsEnabled = false;
            registerTravellerWindow = new RegisterTravellerWindow(this);
            registerTravellerWindow.ShowDialog();
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
                if (Authenticator.currentUser.GetType() == typeof(TravellerObject))
                {
                    TravellerWindow travellerWindow = new TravellerWindow();
                    travellerWindow.Show();
                    this.Close();
                }
                else if (Authenticator.currentUser.GetType() == typeof(EmployeeObject))
                {
                    EmployeeWindow employeeWindow = new EmployeeWindow();
                    employeeWindow.Show();
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
            TryLogin("199301120000", "test123");
        }

        private void b_cheat2_Click(object sender, RoutedEventArgs e)
        {
            TryLogin("199301121111", "test123");
        }

        private void b_cheat3_Click(object sender, RoutedEventArgs e)
        {
            TryLogin("randoro93@gmail.com", "test123");
        }

        
    }
}
