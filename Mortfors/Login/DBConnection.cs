using Mortfors.SQLObject;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mortfors.Login
{
    static class DBConnection
    {
        #region DBCredentials
        const string host = "pgserver.mah.se";
        const string port = "5432";
        const string userID = "ae7076";
        const string password = "xvkkumqd";
        const string database = "ae7076";

        #endregion DBCredentials

        public static string ErrorMessage = "";

        #region rawConnections

        /// <summary>
        /// For getting rows, SELECT
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static NpgsqlConnection OpenConnectionAndGetReader(string query, out NpgsqlDataReader reader, params object[] parameters)
        {
            NpgsqlConnection conn = null;
            reader = null;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.Add(new NpgsqlParameter(":p" + i.ToString(), parameters[i]));
                }
                reader = cmd.ExecuteReader();

                Console.WriteLine("Query (Reader) command executed: " + cmd.CommandText);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Message: " + e.Message);
                Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
            }
            return conn;
        }

        /// <summary>
        /// For getting single values, COUNT, SUM
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static int ExecuteAndGetScalar(string query, params object[] parameters)
        {
            NpgsqlConnection conn = null;
            int number = -1;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.Add(new NpgsqlParameter(":p" + i.ToString(), parameters[i]));
                }
                number = Convert.ToInt32(cmd.ExecuteScalar());


                Console.WriteLine("Scalar command executed: " + cmd.CommandText);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Message: " + e.Message);
                Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
            }
            finally
            {
                conn.Close();
            }
            return number;
        }


        /// <summary>
        /// For getting affected rows, INSERT, DELETE, UPDATE
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static int ExecuteAndGetNonQuery(string query, params object[] parameters)
        {
            NpgsqlConnection conn = null;
            int number = -1;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.Add(new NpgsqlParameter(":p" + i.ToString(), parameters[i]));
                }
                number = cmd.ExecuteNonQuery();


                Console.WriteLine("NonQuery command executed: " + cmd.CommandText);

            }
            catch (PostgresException e)
            {
                ErrorMessage = e.Message;
                switch (e.SqlState)
                {
                    case "23503":
                        ErrorMessage = "Främmande nyckel fel.";
                        break;
                    default:
                        break;
                }
                
                Console.WriteLine("Exception Message: " + e.Message);
                Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
            }
            finally
            {
                conn.Close();
            }
            return number;
        }

        #endregion rawConnections

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

        public static AnstalldObject VerifyAnstalld(string username, string hashedPassword)
        {
            ErrorMessage = "Wrong username or password.";
            AnstalldObject returnObj = null;

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from anstalld WHERE pers_nr = :p0 AND losenord = :p1;", out dr, username, hashedPassword);
                if (dr.Read())
                {
                    string personNummer = dr.GetFieldValue<string>(dr.GetOrdinal("pers_nr"));
                    string _hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("losenord"));
                    bool isAdmin = ((dr.GetFieldValue<Int32>(dr.GetOrdinal("admin")) == 0) ? false : true);
                    string namn = dr.GetFieldValue<string>(dr.GetOrdinal("namn"));
                    string adress = dr.GetFieldValue<string>(dr.GetOrdinal("adress"));
                    string telefon = dr.GetFieldValue<string>(dr.GetOrdinal("hem_telefon"));

                    returnObj = new AnstalldObject(personNummer, _hashedPassword, isAdmin, namn, adress, telefon);
                    ErrorMessage = "";
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static ResenarObject VerifyResenar(string username, string hashedPassword)
        {
            ErrorMessage = "Wrong username or password.";
            ResenarObject returnObj = null;

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from resenar WHERE email = :p0 AND losenord = :p1;", out dr, username, hashedPassword);
                if (dr.Read())
                {
                    string email = dr.GetFieldValue<string>(dr.GetOrdinal("email"));
                    string _hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("losenord"));
                    string namn = dr.GetFieldValue<string>(dr.GetOrdinal("namn"));
                    string adress = dr.GetFieldValue<string>(dr.GetOrdinal("adress"));
                    string telefon = dr.GetFieldValue<string>(dr.GetOrdinal("telefon"));

                    returnObj = new ResenarObject(email, _hashedPassword, namn, adress, telefon);
                    ErrorMessage = "";
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
            
        }

        public static int CountBussResor()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from bussresa;");
            return count;
        }

        public static List<BussresaObject> SelectBussResor(int limit, int offset)
        {
            List<BussresaObject> returnObj = new List<BussresaObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from bussresa order by bussresa_id limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    int bussresa_id = dr.GetFieldValue<int>(dr.GetOrdinal("bussresa_id"));
                    string avgangs_adress = dr.GetFieldValue<string>(dr.GetOrdinal("avgangs_adress"));
                    string avgangs_stad = dr.GetFieldValue<string>(dr.GetOrdinal("avgangs_stad"));
                    string avgangs_land = dr.GetFieldValue<string>(dr.GetOrdinal("avgangs_land"));
                    DateTime avgangs_datum = dr.GetFieldValue<DateTime>(dr.GetOrdinal("avgangs_datum"));
                    string ankomst_adress = dr.GetFieldValue<string>(dr.GetOrdinal("ankomst_adress"));
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

                    returnObj.Add(new BussresaObject(bussresa_id, avgangs_adress, avgangs_stad, avgangs_land, avgangs_datum,
                        ankomst_adress, ankomst_stad, ankomst_land, ankomst_datum, kostnad, max_platser, chaffor_id));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int CountHallplatser()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from hallplats;");        
            return count;
        }

        public static List<HallplatsObject> SelectHallplatser(int limit, int offset)
        {
            List<HallplatsObject> returnObj = new List<HallplatsObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from hallplats order by lower(gatu_adress), lower(stad), lower(land) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    string gatu_adress = dr.GetFieldValue<string>(dr.GetOrdinal("gatu_adress"));
                    string stad = dr.GetFieldValue<string>(dr.GetOrdinal("stad"));
                    string land = dr.GetFieldValue<string>(dr.GetOrdinal("land"));

                    returnObj.Add(new HallplatsObject(gatu_adress, stad, land));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int InsertHallplats(HallplatsObject newObject)
        {
            int affectedRows = ExecuteAndGetNonQuery("INSERT INTO hallplats (gatu_adress, stad, land) values (:p0, :p1, :p2);", newObject.gatu_adress, newObject.stad, newObject.land);
            return affectedRows;


        }

        public static int UpdateHallplats(HallplatsObject newObject, HallplatsObject oldObject)
        {
            int affectedRows = ExecuteAndGetNonQuery("UPDATE hallplats SET gatu_adress = :p0, stad = :p1, land = :p2 WHERE gatu_adress = :p3 AND stad = :p4 AND land = :p5;", newObject.gatu_adress, newObject.stad, newObject.land, oldObject.gatu_adress, oldObject.stad, oldObject.land);
            return affectedRows;
        }

        public static int DeleteHallplats(HallplatsObject oldObject)
        {
            int bokningarCount = 0;
            bokningarCount = ExecuteAndGetScalar("SELECT count(*) FROM bokning WHERE bokning.bussresa_id IN (SELECT bussresa.bussresa_id FROM bussresa WHERE (bussresa.avgangs_adress = :p0 AND bussresa.avgangs_stad = :p1 AND bussresa.avgangs_land = :p2) OR (bussresa.ankomst_adress = :p0 AND bussresa.ankomst_stad = :p1 AND bussresa.ankomst_land = :p2));", oldObject.gatu_adress, oldObject.stad, oldObject.land);

            int bussresorCount = 0;
            bussresorCount = ExecuteAndGetScalar("SELECT count(*) FROM bussresa WHERE (bussresa.avgangs_adress = :p0 AND bussresa.avgangs_stad = :p1 AND bussresa.avgangs_land = :p2) OR (bussresa.ankomst_adress = :p0 AND bussresa.ankomst_stad = :p1 AND bussresa.ankomst_land = :p2);", oldObject.gatu_adress, oldObject.stad, oldObject.land);

            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("Om du tar bort den här hållplatsen försvinner totalt "+ bussresorCount+ " bussresor och "+bokningarCount+" bokningar! Vill du fortfarande ta bort hållplatsen?", "Varning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("DELETE FROM bokning WHERE bokning.bussresa_id IN (SELECT bussresa.bussresa_id FROM bussresa WHERE (bussresa.avgangs_adress = :p0 AND bussresa.avgangs_stad = :p1 AND bussresa.avgangs_land = :p2) OR (bussresa.ankomst_adress = :p0 AND bussresa.ankomst_stad = :p1 AND bussresa.ankomst_land = :p2));", oldObject.gatu_adress, oldObject.stad, oldObject.land);
                ExecuteAndGetNonQuery("DELETE FROM bussresa WHERE (bussresa.avgangs_adress = :p0 AND bussresa.avgangs_stad = :p1 AND bussresa.avgangs_land = :p2) OR (bussresa.ankomst_adress = :p0 AND bussresa.ankomst_stad = :p1 AND bussresa.ankomst_land = :p2);", oldObject.gatu_adress, oldObject.stad, oldObject.land);
                affectedRows = ExecuteAndGetNonQuery("DELETE from hallplats WHERE gatu_adress = :p0 AND stad = :p1 AND land = :p2;", oldObject.gatu_adress, oldObject.stad, oldObject.land);
            }
            return affectedRows;
        }
        
        public static int CountResenarer()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from resenar;");
            return count;
        }

        public static List<ResenarObject> SelectResenarer(int limit, int offset)
        {
            List<ResenarObject> returnObj = new List<ResenarObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from resenar order by lower(email) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    string email = dr.GetFieldValue<string>(dr.GetOrdinal("email"));
                    string hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("losenord"));
                    string namn = dr.GetFieldValue<string>(dr.GetOrdinal("namn"));
                    string adress = dr.GetFieldValue<string>(dr.GetOrdinal("adress"));
                    string telefon = dr.GetFieldValue<string>(dr.GetOrdinal("telefon"));

                    returnObj.Add(new ResenarObject(email, hashedPassword, namn, adress, telefon));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int InsertResenar(ResenarObject newObject)
        {
            int affectedRows = ExecuteAndGetNonQuery("INSERT INTO resenar (email, losenord, namn, adress, telefon) values (:p0, :p1, :p2, :p3, :p4);", newObject.email, newObject.hashedPassword, newObject.namn, newObject.adress, newObject.telefon);
            return affectedRows;


        }

        public static int UpdateResenar(ResenarObject newObject, ResenarObject oldObject)
        {
            int affectedRows = ExecuteAndGetNonQuery("UPDATE resenar SET email = :p0, losenord = :p1, namn = :p2, adress = :p3, telefon = :p4 WHERE email = :p5;", newObject.email, newObject.hashedPassword, newObject.namn, newObject.adress, newObject.telefon, oldObject.email);
            return affectedRows;
        }

        public static int DeleteResenar(ResenarObject oldObject)
        {
            int bokningarCount = 0;
            bokningarCount = ExecuteAndGetScalar("SELECT count(*) FROM bokning WHERE bokning.resenar = :p0;", oldObject.email);

            //int bussresorCount = 0;
            //bussresorCount = ExecuteAndGetScalar("SELECT count(*) FROM bussresa WHERE (bussresa.avgangs_adress = :p0 AND bussresa.avgangs_stad = :p1 AND bussresa.avgangs_land = :p2) OR (bussresa.ankomst_adress = :p0 AND bussresa.ankomst_stad = :p1 AND bussresa.ankomst_land = :p2);", oldObject.gatu_adress, oldObject.stad, oldObject.land);

            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("Om du tar bort den här resenären försvinner totalt " + bokningarCount + " bokningar! Vill du fortfarande ta bort resenären?", "Varning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("DELETE FROM bokning WHERE bokning.resenar = :p0;", oldObject.email);
                affectedRows = ExecuteAndGetNonQuery("DELETE from resenar WHERE email = :p0;", oldObject.email);
            }
            return affectedRows;
        }

    }
}
