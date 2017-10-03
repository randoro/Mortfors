using Mortfors.Login;
using Mortfors.SQLObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mortfors.Anstalld.Bokningar
{
    /// <summary>
    /// Interaction logic for AndraBokningWindow.xaml
    /// </summary>
    public partial class AndraBokningWindow : Window
    {

        public HanteraBokningWindow parentWindow;
        readonly BokningObject oldObject;
        bool newbokning;

        ValjBussresaWindow valjBussresaWindow;
        ValjResenarWindow valjResenarWindow;


        public AndraBokningWindow(HanteraBokningWindow _parent)
        {
            InitializeComponent();
            Title = "Ny Bokning - " + Authenticator.GetUserInfo();
            parentWindow = _parent;
            newbokning = true;
        }

        public AndraBokningWindow(HanteraBokningWindow _parent, BokningObject _oldObject)
        {
            InitializeComponent();
            Title = "Redigera Bokning - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            tb_bussresa_id.Text = oldObject.bussresa_id.ToString();
            tb_resenar.Text = oldObject.resenar;
            tb_antal_platser.Text = oldObject.antal_platser.ToString();
            

            parentWindow = _parent;
            newbokning = false;
        }

        void AndraBokningWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_ny.IsEnabled = true;
            parentWindow.b_redigeramarkerad.IsEnabled = true;
            parentWindow.b_tabortmarkerad.IsEnabled = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void b_spara_Click(object sender, RoutedEventArgs e)
        {
            if (tb_bussresa_id.Text == "" || tb_resenar.Text == "" || tb_antal_platser.Text == "")
            {
                MessageBox.Show("Tomma fält är inte tillåtna.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            

            BokningObject newObject = new BokningObject(Int32.Parse(tb_bussresa_id.Text), tb_resenar.Text, Int32.Parse(tb_antal_platser.Text));

            if (DBConnection.CheckIfBokningAllowed(newObject))
            {

                if (newbokning)
                {
                    DBConnection.InsertBokning(newObject);
                }
                else
                {
                    DBConnection.UpdateBokning(newObject, oldObject);
                }

                parentWindow.parentWindow.UpdateAllChain();

                Close();
            }
            else
            {
                MessageBox.Show("Inte tillräckligt många fria platser på bussresan.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //TODO: check if ok
        }

        private void b_avbryt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_andra_bussresa_id_Click(object sender, RoutedEventArgs e)
        {
            b_andra_bussresa_id.IsEnabled = false;
            b_andra_resenar.IsEnabled = false;
            valjBussresaWindow = new ValjBussresaWindow(this);
            valjBussresaWindow.ShowDialog();
        }

        private void b_andra_resenar_Click(object sender, RoutedEventArgs e)
        {
            b_andra_bussresa_id.IsEnabled = false;
            b_andra_resenar.IsEnabled = false;
            valjResenarWindow = new ValjResenarWindow(this);
            valjResenarWindow.ShowDialog();
        }
    }
}
