using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors
{
    static class DBConnection
    {
        const string host = "pgserver.mah.se";
        const string port = "5432";
        const string userID = "ae7076";
        const string password = "xvkkumqd";
        const string database = "ae7076";

        public static bool ConnectAndGetVersion()
        {
            bool boolfound = false;
            using (NpgsqlConnection conn = new NpgsqlConnection("Server="+ host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + ""))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT version(); ", conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();

                using (dr)
                {
                    if (dr.Read())
                    {
                        boolfound = true;
                        Console.Write("Connection Established with version ");
                        Console.WriteLine("{0}", dr[0]);
                    }
                    if (boolfound == false)
                    {
                        Console.WriteLine("Data does not exist");
                    }
                }
            }

            return boolfound;
        }

        public static bool ConnectVerifyUser(string selectStatement, string username, string hashedPassword)
        {
            bool boolfound = false;
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + ""))
            {
                conn.Open();
                //NpgsqlCommand cmd = new NpgsqlCommand("SELECT version(); ", conn);
                //"Select * from hallplats WHERE gatu_address = :username;"
                NpgsqlCommand cmd = new NpgsqlCommand(selectStatement, conn);
                cmd.Parameters.Add(new NpgsqlParameter(":username", username));
                cmd.Parameters.Add(new NpgsqlParameter(":hashedPassword", hashedPassword));
                NpgsqlDataReader dr = cmd.ExecuteReader();



                Console.WriteLine("Executing command: " + cmd.CommandText);

                Console.WriteLine("Executing statements: " + dr.Statements[0]);

                using (dr)
                {
                    if (dr.Read())
                    {
                        boolfound = true;
                        Console.Write("Connection Established with version ");
                        Console.WriteLine("{0}", dr[0]);
                    }
                    if (boolfound == false)
                    {
                        Console.WriteLine("Data does not exist");
                    }
                }
            }
            return boolfound;
        }



        public static bool Execute()
        {
            bool boolfound = false;
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + ""))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT version(); ", conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();


                if (dr.Read())
                {
                    boolfound = true;
                    Console.Write("Connection Established with version ");
                    Console.WriteLine("{0}", dr[0]);
                }
                if (boolfound == false)
                {
                    Console.WriteLine("Data does not exist");
                }
                dr.Close();
            }

            return boolfound;
        }
    }
}
