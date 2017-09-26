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

        #region login

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

        #endregion login

        #region bussresa

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

        public static int InsertBussresa(BussresaObject newObject)
        {
            int affectedRows = -1;
            if (newObject.chaffor_id == "")
            {
                affectedRows = ExecuteAndGetNonQuery("INSERT INTO bussresa (bussresa_id, avgangs_adress, avgangs_stad, avgangs_land, avgangs_datum, ankomst_adress, ankomst_stad, ankomst_land, ankomst_datum, kostnad, max_platser) values (:p0, :p1, :p2, :p3, :p4, :p5, :p6, :p7, :p8, :p9, :p10);", newObject.bussresa_id, newObject.avgangs_adress, newObject.avgangs_stad, newObject.avgangs_land, newObject.avgangs_datum, newObject.ankomst_adress, newObject.ankomst_stad, newObject.ankomst_land, newObject.ankomst_datum, newObject.kostnad, newObject.max_platser);
            }
            else
            {
                affectedRows = ExecuteAndGetNonQuery("INSERT INTO bussresa (bussresa_id, avgangs_adress, avgangs_stad, avgangs_land, avgangs_datum, ankomst_adress, ankomst_stad, ankomst_land, ankomst_datum, kostnad, max_platser, chaffor_id) values (:p0, :p1, :p2, :p3, :p4, :p5, :p6, :p7, :p8, :p9, :p10, :p11);", newObject.bussresa_id, newObject.avgangs_adress, newObject.avgangs_stad, newObject.avgangs_land, newObject.avgangs_datum, newObject.ankomst_adress, newObject.ankomst_stad, newObject.ankomst_land, newObject.ankomst_datum, newObject.kostnad, newObject.max_platser, newObject.chaffor_id);
            }
            return affectedRows;


        }

        public static int UpdateBussresa(BussresaObject newObject, BussresaObject oldObject)
        {
            int affectedRows = -1;
            if(newObject.chaffor_id == "")
            {
                affectedRows = ExecuteAndGetNonQuery("UPDATE bussresa SET bussresa_id = :p0, avgangs_adress = :p1, avgangs_stad = :p2, avgangs_land = :p3, avgangs_datum = :p4, ankomst_adress = :p5, ankomst_stad = :p6, ankomst_land = :p7, ankomst_datum = :p8, kostnad = :p9, max_platser = :p10 WHERE bussresa_id = :p11;", newObject.bussresa_id, newObject.avgangs_adress, newObject.avgangs_stad, newObject.avgangs_land, newObject.avgangs_datum, newObject.ankomst_adress, newObject.ankomst_stad, newObject.ankomst_land, newObject.ankomst_datum, newObject.kostnad, newObject.max_platser, oldObject.bussresa_id);

            }
            else
            {
                affectedRows = ExecuteAndGetNonQuery("UPDATE bussresa SET bussresa_id = :p0, avgangs_adress = :p1, avgangs_stad = :p2, avgangs_land = :p3, avgangs_datum = :p4, ankomst_adress = :p5, ankomst_stad = :p6, ankomst_land = :p7, ankomst_datum = :p8, kostnad = :p9, max_platser = :p10, chaffor_id = :p11 WHERE bussresa_id = :p12;", newObject.bussresa_id, newObject.avgangs_adress, newObject.avgangs_stad, newObject.avgangs_land, newObject.avgangs_datum, newObject.ankomst_adress, newObject.ankomst_stad, newObject.ankomst_land, newObject.ankomst_datum, newObject.kostnad, newObject.max_platser, newObject.chaffor_id, oldObject.bussresa_id);

            }
            return affectedRows;
        }

        public static int DeleteBussresa(BussresaObject oldObject)
        {
            int bokningarCount = 0;
            bokningarCount = ExecuteAndGetScalar("SELECT count(*) FROM bokning WHERE bokning.bussresa_id = :p0;", oldObject.bussresa_id);
            
            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("Om du tar bort den här bussresan försvinner totalt " + bokningarCount + " bokningar! Vill du fortfarande ta bort bussresan?", "Varning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("DELETE FROM bokning WHERE bussresa_id = :p0;", oldObject.bussresa_id);
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM bussresa WHERE bussresa_id = :p0;", oldObject.bussresa_id);
            }
            return affectedRows;
        }

        #endregion bussresa;

        #region hallplats

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
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM hallplats WHERE gatu_adress = :p0 AND stad = :p1 AND land = :p2;", oldObject.gatu_adress, oldObject.stad, oldObject.land);
            }
            return affectedRows;
        }

        #endregion hallplats

        #region resenar

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
            
            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("Om du tar bort den här resenären försvinner totalt " + bokningarCount + " bokningar! Vill du fortfarande ta bort resenären?", "Varning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("DELETE FROM bokning WHERE bokning.resenar = :p0;", oldObject.email);
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM resenar WHERE email = :p0;", oldObject.email);
            }
            return affectedRows;
        }

        #endregion resenar

        #region anstalld

        public static int CountAnstallda()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from anstalld;");
            return count;
        }

        public static List<AnstalldObject> SelectAnstallda(int limit, int offset)
        {
            List<AnstalldObject> returnObj = new List<AnstalldObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from anstalld order by lower(pers_nr) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    string pers_nr = dr.GetFieldValue<string>(dr.GetOrdinal("pers_nr"));
                    string hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("losenord"));
                    bool isAdmin = ((dr.GetFieldValue<Int32>(dr.GetOrdinal("admin")) == 0) ? false : true);
                    string namn = dr.GetFieldValue<string>(dr.GetOrdinal("namn"));
                    string adress = dr.GetFieldValue<string>(dr.GetOrdinal("adress"));
                    string telefon = dr.GetFieldValue<string>(dr.GetOrdinal("hem_telefon"));

                    returnObj.Add(new AnstalldObject(pers_nr, hashedPassword, isAdmin, namn, adress, telefon));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static List<AnstalldObject> SelectChafforer(int limit, int offset)
        {
            List<AnstalldObject> returnObj = new List<AnstalldObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from anstalld where admin = 0 order by lower(pers_nr) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    string pers_nr = dr.GetFieldValue<string>(dr.GetOrdinal("pers_nr"));
                    string hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("losenord"));
                    bool isAdmin = ((dr.GetFieldValue<Int32>(dr.GetOrdinal("admin")) == 0) ? false : true);
                    string namn = dr.GetFieldValue<string>(dr.GetOrdinal("namn"));
                    string adress = dr.GetFieldValue<string>(dr.GetOrdinal("adress"));
                    string telefon = dr.GetFieldValue<string>(dr.GetOrdinal("hem_telefon"));

                    returnObj.Add(new AnstalldObject(pers_nr, hashedPassword, isAdmin, namn, adress, telefon));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int InsertAnstalld(AnstalldObject newObject)
        {
            int affectedRows = ExecuteAndGetNonQuery("INSERT INTO anstalld (pers_nr, losenord, admin, namn, adress, hem_telefon) values (:p0, :p1, :p2, :p3, :p4, :p5);", newObject.personNummer, newObject.hashedPassword, newObject.isAdmin ? 1 : 0, newObject.namn, newObject.adress, newObject.telefon);
            return affectedRows;


        }

        public static int UpdateAnstalld(AnstalldObject newObject, AnstalldObject oldObject)
        {
            int affectedRows = ExecuteAndGetNonQuery("UPDATE anstalld SET pers_nr = :p0, losenord = :p1, admin = :p2, namn = :p3, adress = :p4, hem_telefon = :p5 WHERE pers_nr = :p6;", newObject.personNummer, newObject.hashedPassword, newObject.isAdmin ? 1 : 0, newObject.namn, newObject.adress, newObject.telefon, oldObject.personNummer);
            return affectedRows;
        }

        public static int DeleteAnstalld(AnstalldObject oldObject)
        {
            int bussresaCount = 0;
            bussresaCount = ExecuteAndGetScalar("SELECT count(*) FROM bussresa WHERE bussresa.chaffor_id = :p0;", oldObject.personNummer);

            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("Om du tar bort den här anställda personen försvinner den anställda som chafför på totalt " + bussresaCount + " bussresor! (Men bussresorna finns fortfarande kvar ändå.) Vill du fortfarande ta bort den anställda?", "Varning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("UPDATE bussresa SET chaffor_id = NULL WHERE chaffor_id = :p0;", oldObject.personNummer);
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM anstalld WHERE pers_nr = :p0;", oldObject.personNummer);
            }
            return affectedRows;
        }

        #endregion anstalld

    }
}
