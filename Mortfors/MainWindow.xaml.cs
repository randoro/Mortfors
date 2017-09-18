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


            //bool boolfound = false;
            //using (NpgsqlConnection conn = new
            //NpgsqlConnection("Server=pgserver.mah.se; Port=5432; UserId = ae7076; Password = xvkkumqd; Database = ae7076"))
            //{
            //    conn.Open();
            //    NpgsqlCommand cmd = new NpgsqlCommand("SELECT version(); ", conn);
            //    NpgsqlDataReader dr = cmd.ExecuteReader();


            //    if (dr.Read())
            //    {
            //        boolfound = true;
            //        Console.WriteLine("connection established");
            //        Console.WriteLine("{0}", dr[0]);
            //    }
            //    if (boolfound == false)
            //    {
            //        Console.WriteLine("Data does not exist");
            //    }
            //    dr.Close();
            //}
            //Console.ReadLine();

            DBConnection.ConnectAndSelect("Timmermansgatan");
        }




    }
}
