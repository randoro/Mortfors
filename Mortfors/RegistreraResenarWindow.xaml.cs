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
    /// Interaction logic for RegistreraResenarWindow.xaml
    /// </summary>
    public partial class RegistreraResenarWindow : Window
    {
        public MainWindow parentWindow;
        readonly ResenarObject oldObject;


        public RegistreraResenarWindow(MainWindow _parent)
        {
            InitializeComponent();
            Title = "Registrera som ny resenär";
            parentWindow = _parent;
        }

        void RegistreraResenarWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_registrera.IsEnabled = true;
        }

        private void b_spara_Click(object sender, RoutedEventArgs e)
        {
            if (tb_email.Text == "" || pb_losenord.Password == "" || pb_bekrafta_losenord.Password == "" || tb_namn.Text == "" || tb_adress.Text == "" || tb_telefon.Text == "")
            {
                MessageBox.Show("Tomma fält är inte tillåtna.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(pb_losenord.Password == pb_bekrafta_losenord.Password))
            {
                MessageBox.Show("Lösenorden i lösenordsfält och bekräftat lösenordsfält stämmer inte överens.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            ResenarObject newObject = new ResenarObject(tb_email.Text, SimpleHash.GenerateHashedPassword(tb_email.Text, pb_losenord.Password), tb_namn.Text, tb_adress.Text, tb_telefon.Text);



            if (DBConnection.InsertResenar(newObject) > 0)
            {
                Authenticator.currentUser = newObject;
                parentWindow.TryLogin(tb_email.Text, pb_losenord.Password);
            }


            Close();
            //TODO: check if ok
        }

        private void b_avbryt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
