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

namespace Mortfors.Resenar
{
    /// <summary>
    /// Interaction logic for AndraResenarBokningWindow.xaml
    /// </summary>
    public partial class AndraResenarBokningWindow : Window
    {
        public ResenarWindow parentWindow;
        readonly BussresaObject oldObject;
        

        public AndraResenarBokningWindow(ResenarWindow _parent, BussresaObject _oldObject)
        {
            InitializeComponent();
            Title = "Ny Bokning - " + Authenticator.GetUserInfo();
            oldObject = _oldObject;
            l_avgang.Content = "Avgång: " + Environment.NewLine +  oldObject.avgangs_datum.ToString("dd-MMM-yyyy HH:mm:ss") + Environment.NewLine + oldObject.avgangs_adress + Environment.NewLine + oldObject.avgangs_stad + Environment.NewLine + oldObject.avgangs_land;
            l_ankomst.Content = "Ankomst: " + Environment.NewLine + oldObject.ankomst_datum.ToString("dd-MMM-yyyy HH:mm:ss") + Environment.NewLine + oldObject.ankomst_adress + Environment.NewLine + oldObject.ankomst_stad + Environment.NewLine + oldObject.ankomst_land;
            tb_antal_platser.Text = "";


            parentWindow = _parent;
        }

        void AndraBokningWindow_Closing(object sender, CancelEventArgs e)
        {
            parentWindow.b_bokamarkerad.IsEnabled = true;
            parentWindow.b_redigeramarkerad.IsEnabled = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void b_spara_Click(object sender, RoutedEventArgs e)
        {
            if (tb_antal_platser.Text == "")
            {
                MessageBox.Show("Tomma fält är inte tillåtna.", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }



            BokningObject newObject = new BokningObject(oldObject.bussresa_id, ((ResenarObject)Authenticator.currentUser).email, Int32.Parse(tb_antal_platser.Text));

            if (DBConnection.CheckIfBokningAllowed(newObject))
            {
                    DBConnection.InsertBokning(newObject);

                parentWindow.UpdateAllChain();

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
    }
}
