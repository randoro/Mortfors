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

        public static AnstalldObject ConnectVerifyAnstalld(string username, string hashedPassword, out bool boolfound, out string errorMessage)
        {
            boolfound = false;
            errorMessage = "";
            AnstalldObject user = null;
            NpgsqlConnection conn = null;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * from anstalld WHERE pers_nr = :username AND losenord = :hashedPassword;", conn);
                cmd.Parameters.Add(new NpgsqlParameter(":username", username));
                cmd.Parameters.Add(new NpgsqlParameter(":hashedPassword", hashedPassword));
                NpgsqlDataReader dr = cmd.ExecuteReader();

                Console.WriteLine("Executing command: " + cmd.CommandText);
                Console.WriteLine("Executing statements: " + dr.Statements[0]);
                
                using (dr)
                {
                    if (dr.Read())
                    {
                        string personNummer = dr.GetFieldValue<string>(dr.GetOrdinal("pers_nr"));
                        string _hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("losenord"));
                        bool isAdmin = ((dr.GetFieldValue<Int32>(dr.GetOrdinal("admin")) == 0) ? false : true);
                        string namn = dr.GetFieldValue<string>(dr.GetOrdinal("namn"));
                        string address = dr.GetFieldValue<string>(dr.GetOrdinal("address"));
                        string telefon = dr.GetFieldValue<string>(dr.GetOrdinal("hem_telefon"));

                        user = new AnstalldObject(personNummer, _hashedPassword, isAdmin, namn, address, telefon);
                        boolfound = true;
                    }
                    if (boolfound == false)
                    {
                        errorMessage = "Wrong username or password.";
                    }
                }
            } catch (Exception e)
            {
                errorMessage = "Could not connect to database.";
                Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
            }
            finally
            {
                conn.Close();
            }
            return user;
        }

        public static ResenarObject ConnectVerifyResenar(string username, string hashedPassword, out bool boolfound, out string errorMessage)
        {
            boolfound = false;
            errorMessage = "";
            ResenarObject user = null;
            NpgsqlConnection conn = null;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * from resenar WHERE email = :username AND losenord = :hashedPassword;", conn);
                cmd.Parameters.Add(new NpgsqlParameter(":username", username));
                cmd.Parameters.Add(new NpgsqlParameter(":hashedPassword", hashedPassword));
                NpgsqlDataReader dr = cmd.ExecuteReader();

                Console.WriteLine("Executing command: " + cmd.CommandText);
                Console.WriteLine("Executing statements: " + dr.Statements[0]);
                

                using (dr)
                {
                    if (dr.Read())
                    {
                        string email = dr.GetFieldValue<string>(dr.GetOrdinal("email"));
                        string _hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("losenord"));
                        string namn = dr.GetFieldValue<string>(dr.GetOrdinal("namn"));
                        string address = dr.GetFieldValue<string>(dr.GetOrdinal("address"));
                        string telefon = dr.GetFieldValue<string>(dr.GetOrdinal("telefon"));

                        user = new ResenarObject(email, _hashedPassword, namn, address, telefon);
                        boolfound = true;
                    }
                    if (boolfound == false)
                    {
                        errorMessage = "Wrong username or password.";
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = "Could not connect to database.";
                Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
            }
            finally
            {
                conn.Close();
            }
            return user;
        }

        public static List<BussresaObject> ConnectSelectBussResor(int limit, int offset, out bool boolfound, out string errorMessage)
        {
            boolfound = false;
            errorMessage = "";
            List<BussresaObject> resor = null;
            NpgsqlConnection conn = null;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * from bussresa order by bussresa_id limit :limit offset :offset;", conn);
                cmd.Parameters.Add(new NpgsqlParameter(":limit", limit));
                cmd.Parameters.Add(new NpgsqlParameter(":offset", offset));
                NpgsqlDataReader dr = cmd.ExecuteReader();

                Console.WriteLine("Executing command: " + cmd.CommandText);
                Console.WriteLine("Executing statements: " + dr.Statements[0]);
                resor = new List<BussresaObject>();

                using (dr)
                {
                    while (dr.Read())
                    {
                        int bussresa_id = dr.GetFieldValue<int>(dr.GetOrdinal("bussresa_id"));
                        string avgangs_address = dr.GetFieldValue<string>(dr.GetOrdinal("avgangs_address"));
                        string avgangs_stad = dr.GetFieldValue<string>(dr.GetOrdinal("avgangs_stad"));
                        string avgangs_land = dr.GetFieldValue<string>(dr.GetOrdinal("avgangs_land"));
                        DateTime avgangs_datum = dr.GetFieldValue<DateTime>(dr.GetOrdinal("avgangs_datum"));
                        string ankomst_address = dr.GetFieldValue<string>(dr.GetOrdinal("ankomst_address"));
                        string ankomst_stad = dr.GetFieldValue<string>(dr.GetOrdinal("ankomst_stad"));
                        string ankomst_land = dr.GetFieldValue<string>(dr.GetOrdinal("ankomst_land"));
                        DateTime ankomst_datum = dr.GetFieldValue<DateTime>(dr.GetOrdinal("ankomst_datum"));
                        int kostnad = dr.GetFieldValue<int>(dr.GetOrdinal("kostnad"));
                        int max_platser = dr.GetFieldValue<int>(dr.GetOrdinal("max_platser"));
                        string chaffor_id = null;
                        if (!dr.IsDBNull(dr.GetOrdinal("chaffor_id")))
                        {
                            chaffor_id = dr.GetFieldValue<string>(dr.GetOrdinal("chaffor_id"));
                        }
                        
                        resor.Add(new BussresaObject(bussresa_id, avgangs_address, avgangs_stad, avgangs_land, avgangs_datum, 
                            ankomst_address, ankomst_stad, ankomst_land, ankomst_datum, kostnad, max_platser, chaffor_id));
                        boolfound = true;
                    }
                    if (boolfound == false)
                    {
                        //No entries
                        //errorMessage = "Wrong username or password.";
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = "Could not connect to database.";
                Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
            }
            finally
            {
                conn.Close();
            }
            return resor;
        }


        public static int ConnectCountBussResor(out bool boolfound, out string errorMessage)
        {
            boolfound = false;
            errorMessage = "";
            NpgsqlConnection conn = null;
            int count = 0;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT count(*) from bussresa;", conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();

                Console.WriteLine("Executing command: " + cmd.CommandText);
                Console.WriteLine("Executing statements: " + dr.Statements[0]);

                using (dr)
                {
                    while (dr.Read())
                    {
                        count = dr.GetFieldValue<int>(0);
                        boolfound = true;
                    }
                    if (boolfound == false)
                    {
                        //No entries
                        //errorMessage = "Wrong username or password.";
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = "Could not connect to database.";
                Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
            }
            finally
            {
                conn.Close();
            }
            return count;
        }



    }
}
