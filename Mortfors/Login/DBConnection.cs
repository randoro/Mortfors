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

        #region errorHandling
        public static string ErrorMessage = "";

        private static void HandleException(PostgresException e)
        {
            ErrorMessage = e.Message;
            switch (e.SqlState)
            {
                case "23503":
                    MessageBox.Show("Something is wrong with a foreign key value. Try refreshing!", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case "23505":
                    MessageBox.Show("The table already has an entry with the same ID or key values. Can't add duplicates!", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                default:
                    MessageBox.Show("Unknown SQL Error! " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }

            Console.WriteLine("Exception Message: " + e.Message);
            Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
        }


        #endregion errorHandling

        #region rawConnections

        /// <summary>
        /// For getting rows, SELECT
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns>Returns Connection</returns>
        private static NpgsqlConnection OpenConnectionAndGetReader(string statement, out NpgsqlDataReader reader, params object[] parameters)
        {
            NpgsqlConnection conn = null;
            reader = null;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(statement, conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.Add(new NpgsqlParameter(":p" + i.ToString(), parameters[i]));
                }
                reader = cmd.ExecuteReader();

                Console.WriteLine("Query (Reader) command executed: " + cmd.CommandText);

            }
            catch (PostgresException e)
            {
                HandleException(e);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR! Other kind of exception.");
                Console.WriteLine("Exception Message: " + e.Message);
                Console.WriteLine("Could not connect to database. Stacktrace:" + e.StackTrace);
            }
            return conn;
        }

        /// <summary>
        /// For getting single values, COUNT, SUM, returns single value int. Is -1 in failure.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns>Returns single value int</returns>
        private static int ExecuteAndGetScalar(string statement, params object[] parameters)
        {
            NpgsqlConnection conn = null;
            int number = -1;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(statement, conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.Add(new NpgsqlParameter(":p" + i.ToString(), parameters[i]));
                }
                number = Convert.ToInt32(cmd.ExecuteScalar());


                Console.WriteLine("Scalar command executed: " + cmd.CommandText);

            }
            catch (PostgresException e)
            {
                HandleException(e);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR! Other kind of exception.");
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
        /// For getting affected rows, INSERT, DELETE, UPDATE, returns number of rows affected. Is -1 in failure.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns>Number of rows affected</returns>
        private static int ExecuteAndGetNonQuery(string statement, params object[] parameters)
        {
            NpgsqlConnection conn = null;
            int number = -1;

            try
            {
                conn = new NpgsqlConnection("Server=" + host + "; Port=" + port + "; UserId = " + userID + "; Password = " + password + "; Database = " + database + "");
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(statement, conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.Add(new NpgsqlParameter(":p" + i.ToString(), parameters[i]));
                }
                number = cmd.ExecuteNonQuery();


                Console.WriteLine("NonQuery command executed: " + cmd.CommandText);

            }
            catch (PostgresException e)
            {
                HandleException(e);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR! Other kind of exception.");
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

        public static EmployeeObject VerifyEmployee(string username, string hashedPassword)
        {
            ErrorMessage = "Wrong username or password.";
            EmployeeObject returnObj = null;

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from employee WHERE social_security_nr = :p0 AND password = :p1;", out dr, username, hashedPassword);
                if (dr.Read())
                {
                    string personNummer = dr.GetFieldValue<string>(dr.GetOrdinal("social_security_nr"));
                    string _hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("password"));
                    bool isAdmin = ((dr.GetFieldValue<Int32>(dr.GetOrdinal("admin")) == 0) ? false : true);
                    string name = dr.GetFieldValue<string>(dr.GetOrdinal("name"));
                    string address = dr.GetFieldValue<string>(dr.GetOrdinal("address"));
                    string phone = dr.GetFieldValue<string>(dr.GetOrdinal("home_phone"));

                    returnObj = new EmployeeObject(personNummer, _hashedPassword, isAdmin, name, address, phone);
                    ErrorMessage = "";
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static TravellerObject VerifyTraveller(string username, string hashedPassword)
        {
            ErrorMessage = "Wrong username or password.";
            TravellerObject returnObj = null;

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from traveller WHERE email = :p0 AND password = :p1;", out dr, username, hashedPassword);
                if (dr.Read())
                {
                    string email = dr.GetFieldValue<string>(dr.GetOrdinal("email"));
                    string _hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("password"));
                    string name = dr.GetFieldValue<string>(dr.GetOrdinal("name"));
                    string address = dr.GetFieldValue<string>(dr.GetOrdinal("address"));
                    string phone = dr.GetFieldValue<string>(dr.GetOrdinal("phone"));

                    returnObj = new TravellerObject(email, _hashedPassword, name, address, phone);
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

        #region booking

        public static int CountBookings()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from booking;");
            return count;
        }

        public static List<BookingObject> SelectBookings(int limit, int offset)
        {
            List<BookingObject> returnObj = new List<BookingObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from booking order by busride_id, lower(traveller) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    int busride_id = dr.GetFieldValue<int>(dr.GetOrdinal("busride_id"));
                    string traveller = dr.GetFieldValue<string>(dr.GetOrdinal("traveller"));
                    int seats = dr.GetFieldValue<int>(dr.GetOrdinal("seats"));

                    returnObj.Add(new BookingObject(busride_id, traveller, seats));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int InsertBooking(BookingObject newObject)
        {
            return ExecuteAndGetNonQuery("INSERT INTO booking (busride_id, traveller, seats) values (:p0, :p1, :p2);", newObject.busride_id, newObject.traveller, newObject.seats);


        }

        public static int UpdateBooking(BookingObject newObject, BookingObject oldObject)
        {
            return ExecuteAndGetNonQuery("UPDATE booking SET busride_id = :p0, traveller = :p1, seats = :p2 WHERE busride_id = :p3 AND traveller = :p4;", newObject.busride_id, newObject.traveller, newObject.seats, oldObject.busride_id, oldObject.traveller);
        }

        public static int DeleteBooking(BookingObject oldObject)
        {
            
            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("Do you wish to remove this booking?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM booking WHERE busride_id = :p0 AND traveller = :p1;", oldObject.busride_id, oldObject.traveller);
            }
            return affectedRows;
        }

        public static bool CheckIfBookingAllowed(BookingObject newObject)
        {
            int max_seats = ExecuteAndGetScalar("SELECT max_seats FROM busride WHERE busride_id = :p0;", newObject.busride_id);

            int currently_booked = ExecuteAndGetScalar("SELECT COALESCE(sum(seats),0) FROM booking WHERE busride_id = :p0;", newObject.busride_id);
            
            int possibly_current_Selected = ExecuteAndGetScalar("SELECT seats FROM booking WHERE busride_id = :p0 AND traveller = :p1;", newObject.busride_id, newObject.traveller);
            if(possibly_current_Selected == -1)
            {
                possibly_current_Selected = 0;
            }

            if (currently_booked - possibly_current_Selected + newObject.seats <= max_seats)
            {
                return true;
            }
            
            return false;
        }

        public static bool CheckIfHasBooking(int busride_id, string traveller)
        {
            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * FROM booking WHERE busride_id = :p0 AND traveller = :p1;", out dr, busride_id, traveller);
                if (dr.Read())
                {
                    return true;
                }

            }
            finally
            {
                conn.Close();
            }
            return false;
            
        }

        public static List<MyBookingObject> SelectBookingsJoinBusride(string _traveller, int limit, int offset)
        {
            List<MyBookingObject> returnObj = new List<MyBookingObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * FROM (booking JOIN busride ON booking.busride_id = busride.busride_id) WHERE traveller = :p0 order by booking.busride_id, lower(traveller) limit :p1 offset :p2;", out dr, _traveller, limit, offset);
                while (dr.Read())
                {
                    int busride_id = dr.GetFieldValue<int>(dr.GetOrdinal("busride_id"));
                    string departure_address = dr.GetFieldValue<string>(dr.GetOrdinal("departure_address"));
                    string departure_city = dr.GetFieldValue<string>(dr.GetOrdinal("departure_city"));
                    string departure_country = dr.GetFieldValue<string>(dr.GetOrdinal("departure_country"));
                    DateTime departure_date = dr.GetFieldValue<DateTime>(dr.GetOrdinal("departure_date"));
                    string arrival_address = dr.GetFieldValue<string>(dr.GetOrdinal("arrival_address"));
                    string arrival_city = dr.GetFieldValue<string>(dr.GetOrdinal("arrival_city"));
                    string arrival_country = dr.GetFieldValue<string>(dr.GetOrdinal("arrival_country"));
                    DateTime arrival_date = dr.GetFieldValue<DateTime>(dr.GetOrdinal("arrival_date"));
                    int cost = dr.GetFieldValue<int>(dr.GetOrdinal("cost"));
                    int max_seats = dr.GetFieldValue<int>(dr.GetOrdinal("max_seats"));
                    string driver_id = null;
                    if (!dr.IsDBNull(dr.GetOrdinal("driver_id")))
                    {
                        driver_id = dr.GetFieldValue<string>(dr.GetOrdinal("driver_id"));
                    }
                    BusrideObject buss = new BusrideObject(busride_id, departure_address, departure_city, departure_country, departure_date,
                        arrival_address, arrival_city, arrival_country, arrival_date, cost, max_seats, driver_id);

                    string traveller = dr.GetFieldValue<string>(dr.GetOrdinal("traveller"));
                    int seats = dr.GetFieldValue<int>(dr.GetOrdinal("seats"));

                    returnObj.Add(new MyBookingObject(buss, traveller, seats));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        #endregion booking

        #region busride

        public static int CountBusrides()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from busride;");
            return count;
        }

        public static List<BusrideObject> SelectBusrides(int limit, int offset)
        {
            List<BusrideObject> returnObj = new List<BusrideObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from busride order by busride_id limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    int busride_id = dr.GetFieldValue<int>(dr.GetOrdinal("busride_id"));
                    string departure_address = dr.GetFieldValue<string>(dr.GetOrdinal("departure_address"));
                    string departure_city = dr.GetFieldValue<string>(dr.GetOrdinal("departure_city"));
                    string departure_country = dr.GetFieldValue<string>(dr.GetOrdinal("departure_country"));
                    DateTime departure_date = dr.GetFieldValue<DateTime>(dr.GetOrdinal("departure_date"));
                    string arrival_address = dr.GetFieldValue<string>(dr.GetOrdinal("arrival_address"));
                    string arrival_city = dr.GetFieldValue<string>(dr.GetOrdinal("arrival_city"));
                    string arrival_country = dr.GetFieldValue<string>(dr.GetOrdinal("arrival_country"));
                    DateTime arrival_date = dr.GetFieldValue<DateTime>(dr.GetOrdinal("arrival_date"));
                    int cost = dr.GetFieldValue<int>(dr.GetOrdinal("cost"));
                    int max_seats = dr.GetFieldValue<int>(dr.GetOrdinal("max_seats"));
                    string driver_id = null;
                    if (!dr.IsDBNull(dr.GetOrdinal("driver_id")))
                    {
                        driver_id = dr.GetFieldValue<string>(dr.GetOrdinal("driver_id"));
                    }

                    returnObj.Add(new BusrideObject(busride_id, departure_address, departure_city, departure_country, departure_date,
                        arrival_address, arrival_city, arrival_country, arrival_date, cost, max_seats, driver_id));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int InsertBusride(BusrideObject newObject)
        {
            int affectedRows = -1;
            if (newObject.driver_id == "")
            {
                affectedRows = ExecuteAndGetNonQuery("INSERT INTO busride (busride_id, departure_address, departure_city, departure_country, departure_date, arrival_address, arrival_city, arrival_country, arrival_date, cost, max_seats) values (:p0, :p1, :p2, :p3, :p4, :p5, :p6, :p7, :p8, :p9, :p10);", newObject.busride_id, newObject.departure_address, newObject.departure_city, newObject.departure_country, newObject.departure_date, newObject.arrival_address, newObject.arrival_city, newObject.arrival_country, newObject.arrival_date, newObject.cost, newObject.max_seats);
            }
            else
            {
                affectedRows = ExecuteAndGetNonQuery("INSERT INTO busride (busride_id, departure_address, departure_city, departure_country, departure_date, arrival_address, arrival_city, arrival_country, arrival_date, cost, max_seats, driver_id) values (:p0, :p1, :p2, :p3, :p4, :p5, :p6, :p7, :p8, :p9, :p10, :p11);", newObject.busride_id, newObject.departure_address, newObject.departure_city, newObject.departure_country, newObject.departure_date, newObject.arrival_address, newObject.arrival_city, newObject.arrival_country, newObject.arrival_date, newObject.cost, newObject.max_seats, newObject.driver_id);
            }
            return affectedRows;


        }

        public static int UpdateBusride(BusrideObject newObject, BusrideObject oldObject)
        {
            int affectedRows = -1;
            if(newObject.driver_id == "")
            {
                affectedRows = ExecuteAndGetNonQuery("UPDATE busride SET busride_id = :p0, departure_address = :p1, departure_city = :p2, departure_country = :p3, departure_date = :p4, arrival_address = :p5, arrival_city = :p6, arrival_country = :p7, arrival_date = :p8, cost = :p9, max_seats = :p10, driver_id = NULL WHERE busride_id = :p11;", newObject.busride_id, newObject.departure_address, newObject.departure_city, newObject.departure_country, newObject.departure_date, newObject.arrival_address, newObject.arrival_city, newObject.arrival_country, newObject.arrival_date, newObject.cost, newObject.max_seats, oldObject.busride_id);

            }
            else
            {
                affectedRows = ExecuteAndGetNonQuery("UPDATE busride SET busride_id = :p0, departure_address = :p1, departure_city = :p2, departure_country = :p3, departure_date = :p4, arrival_address = :p5, arrival_city = :p6, arrival_country = :p7, arrival_date = :p8, cost = :p9, max_seats = :p10, driver_id = :p11 WHERE busride_id = :p12;", newObject.busride_id, newObject.departure_address, newObject.departure_city, newObject.departure_country, newObject.departure_date, newObject.arrival_address, newObject.arrival_city, newObject.arrival_country, newObject.arrival_date, newObject.cost, newObject.max_seats, newObject.driver_id, oldObject.busride_id);

            }
            return affectedRows;
        }

        public static int DeleteBusride(BusrideObject oldObject)
        {
            int bookingsCount = 0;
            bookingsCount = ExecuteAndGetScalar("SELECT count(*) FROM booking WHERE booking.busride_id = :p0;", oldObject.busride_id);
            
            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("If you remove this busride a total of " + bookingsCount + " bookings will also be removed! Do you still wish to remove this busride?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("DELETE FROM booking WHERE busride_id = :p0;", oldObject.busride_id);
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM busride WHERE busride_id = :p0;", oldObject.busride_id);
            }
            return affectedRows;
        }

        public static int SetBusrideChaffor(BusrideObject oldObject, string driver_id)
        {
            if(oldObject.driver_id == null || oldObject.driver_id == "")
            {
                BusrideObject newObject = new BusrideObject(oldObject.busride_id, oldObject.departure_address, oldObject.departure_city, oldObject.departure_country, oldObject.departure_date, oldObject.arrival_address, oldObject.arrival_city, oldObject.arrival_country, oldObject.arrival_date, oldObject.cost, oldObject.max_seats, driver_id);
                return UpdateBusride(newObject, oldObject);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("You can only apply to drive a busride that currently has no driver.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            return -1;
        }

        public static int RemoveBusrideChaffor(BusrideObject oldObject, string driver_id)
        {
            if (oldObject.driver_id == null || oldObject.driver_id == driver_id)
            {
                BusrideObject newObject = new BusrideObject(oldObject.busride_id, oldObject.departure_address, oldObject.departure_city, oldObject.departure_country, oldObject.departure_date, oldObject.arrival_address, oldObject.arrival_city, oldObject.arrival_country, oldObject.arrival_date, oldObject.cost, oldObject.max_seats, "");
                return UpdateBusride(newObject, oldObject);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("You can only cancel the driver on a busride where you are the driver.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            return -1;
        }

        #endregion busride;

        #region station

        public static int CountStations()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from station;");        
            return count;
        }

        public static List<StationObject> SelectStations(int limit, int offset)
        {
            List<StationObject> returnObj = new List<StationObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from station order by lower(street_address), lower(city), lower(country) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    string street_address = dr.GetFieldValue<string>(dr.GetOrdinal("street_address"));
                    string city = dr.GetFieldValue<string>(dr.GetOrdinal("city"));
                    string country = dr.GetFieldValue<string>(dr.GetOrdinal("country"));

                    returnObj.Add(new StationObject(street_address, city, country));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int InsertStation(StationObject newObject)
        {
            return ExecuteAndGetNonQuery("INSERT INTO station (street_address, city, country) VALUES (:p0, :p1, :p2);", newObject.street_address, newObject.city, newObject.country);
        }

        public static int UpdateStation(StationObject newObject, StationObject oldObject)
        {
            return ExecuteAndGetNonQuery("UPDATE station SET street_address = :p0, city = :p1, country = :p2 WHERE street_address = :p3 AND city = :p4 AND country = :p5;", newObject.street_address, newObject.city, newObject.country, oldObject.street_address, oldObject.city, oldObject.country);
        }

        public static int DeleteStation(StationObject oldObject)
        {
            int bookingsCount = 0;
            bookingsCount = ExecuteAndGetScalar("SELECT count(*) FROM booking WHERE booking.busride_id IN (SELECT busride.busride_id FROM busride WHERE (busride.departure_address = :p0 AND busride.departure_city = :p1 AND busride.departure_country = :p2) OR (busride.arrival_address = :p0 AND busride.arrival_city = :p1 AND busride.arrival_country = :p2));", oldObject.street_address, oldObject.city, oldObject.country);

            int busridesCount = 0;
            busridesCount = ExecuteAndGetScalar("SELECT count(*) FROM busride WHERE (busride.departure_address = :p0 AND busride.departure_city = :p1 AND busride.departure_country = :p2) OR (busride.arrival_address = :p0 AND busride.arrival_city = :p1 AND busride.arrival_country = :p2);", oldObject.street_address, oldObject.city, oldObject.country);

            int affectedRows = -1;

           MessageBoxResult result = MessageBox.Show("If you remove this station a total of " + busridesCount+ " busrides and "+bookingsCount+ " bookings will also be removed! Do you still wish to remove this station?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("DELETE FROM booking WHERE booking.busride_id IN (SELECT busride.busride_id FROM busride WHERE (busride.departure_address = :p0 AND busride.departure_city = :p1 AND busride.departure_country = :p2) OR (busride.arrival_address = :p0 AND busride.arrival_city = :p1 AND busride.arrival_country = :p2));", oldObject.street_address, oldObject.city, oldObject.country);
                ExecuteAndGetNonQuery("DELETE FROM busride WHERE (busride.departure_address = :p0 AND busride.departure_city = :p1 AND busride.departure_country = :p2) OR (busride.arrival_address = :p0 AND busride.arrival_city = :p1 AND busride.arrival_country = :p2);", oldObject.street_address, oldObject.city, oldObject.country);
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM station WHERE street_address = :p0 AND city = :p1 AND country = :p2;", oldObject.street_address, oldObject.city, oldObject.country);
            }
            return affectedRows;
        }

        #endregion station

        #region traveller

        public static int CountTravellers()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from traveller;");
            return count;
        }

        public static List<TravellerObject> SelectTravellers(int limit, int offset)
        {
            List<TravellerObject> returnObj = new List<TravellerObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from traveller order by lower(email) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    string email = dr.GetFieldValue<string>(dr.GetOrdinal("email"));
                    string hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("password"));
                    string name = dr.GetFieldValue<string>(dr.GetOrdinal("name"));
                    string address = dr.GetFieldValue<string>(dr.GetOrdinal("address"));
                    string phone = dr.GetFieldValue<string>(dr.GetOrdinal("phone"));

                    returnObj.Add(new TravellerObject(email, hashedPassword, name, address, phone));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int InsertTraveller(TravellerObject newObject)
        {
            return ExecuteAndGetNonQuery("INSERT INTO traveller (email, password, name, address, phone) values (:p0, :p1, :p2, :p3, :p4);", newObject.email, newObject.hashedPassword, newObject.name, newObject.address, newObject.phone);


        }

        public static int UpdateTraveller(TravellerObject newObject, TravellerObject oldObject)
        {
            return ExecuteAndGetNonQuery("UPDATE traveller SET email = :p0, password = :p1, name = :p2, address = :p3, phone = :p4 WHERE email = :p5;", newObject.email, newObject.hashedPassword, newObject.name, newObject.address, newObject.phone, oldObject.email);
        }

        public static int DeleteTraveller(TravellerObject oldObject)
        {
            int bookingsCount = 0;
            bookingsCount = ExecuteAndGetScalar("SELECT count(*) FROM booking WHERE booking.traveller = :p0;", oldObject.email);
            
            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("If you remove this traveller a total of " + bookingsCount + " bookings will also be removed! Do you still wish to remove this traveller?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("DELETE FROM booking WHERE booking.traveller = :p0;", oldObject.email);
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM traveller WHERE email = :p0;", oldObject.email);
            }
            return affectedRows;
        }

        #endregion traveller

        #region employee

        public static int CountEmployees()
        {
            int count = 0;
            count = ExecuteAndGetScalar("SELECT count(*) from employee;");
            return count;
        }

        public static List<EmployeeObject> SelectEmployees(int limit, int offset)
        {
            List<EmployeeObject> returnObj = new List<EmployeeObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from employee order by lower(social_security_nr) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    string social_security_nr = dr.GetFieldValue<string>(dr.GetOrdinal("social_security_nr"));
                    string hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("password"));
                    bool isAdmin = ((dr.GetFieldValue<Int32>(dr.GetOrdinal("admin")) == 0) ? false : true);
                    string name = dr.GetFieldValue<string>(dr.GetOrdinal("name"));
                    string address = dr.GetFieldValue<string>(dr.GetOrdinal("address"));
                    string phone = dr.GetFieldValue<string>(dr.GetOrdinal("home_phone"));

                    returnObj.Add(new EmployeeObject(social_security_nr, hashedPassword, isAdmin, name, address, phone));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static List<EmployeeObject> SelectChafforer(int limit, int offset)
        {
            List<EmployeeObject> returnObj = new List<EmployeeObject>();

            NpgsqlConnection conn = null;
            NpgsqlDataReader dr = null;

            try
            {
                conn = OpenConnectionAndGetReader("SELECT * from employee where admin = 0 order by lower(social_security_nr) limit :p0 offset :p1;", out dr, limit, offset);
                while (dr.Read())
                {
                    string social_security_nr = dr.GetFieldValue<string>(dr.GetOrdinal("social_security_nr"));
                    string hashedPassword = dr.GetFieldValue<string>(dr.GetOrdinal("password"));
                    bool isAdmin = ((dr.GetFieldValue<Int32>(dr.GetOrdinal("admin")) == 0) ? false : true);
                    string name = dr.GetFieldValue<string>(dr.GetOrdinal("name"));
                    string address = dr.GetFieldValue<string>(dr.GetOrdinal("address"));
                    string phone = dr.GetFieldValue<string>(dr.GetOrdinal("home_phone"));

                    returnObj.Add(new EmployeeObject(social_security_nr, hashedPassword, isAdmin, name, address, phone));
                }

            }
            finally
            {
                conn.Close();
            }
            return returnObj;
        }

        public static int InsertEmployee(EmployeeObject newObject)
        {
            return ExecuteAndGetNonQuery("INSERT INTO employee (social_security_nr, password, admin, name, address, home_phone) values (:p0, :p1, :p2, :p3, :p4, :p5);", newObject.personNummer, newObject.hashedPassword, newObject.isAdmin ? 1 : 0, newObject.name, newObject.address, newObject.phone);


        }

        public static int UpdateEmployee(EmployeeObject newObject, EmployeeObject oldObject)
        {
            return ExecuteAndGetNonQuery("UPDATE employee SET social_security_nr = :p0, password = :p1, admin = :p2, name = :p3, address = :p4, home_phone = :p5 WHERE social_security_nr = :p6;", newObject.personNummer, newObject.hashedPassword, newObject.isAdmin ? 1 : 0, newObject.name, newObject.address, newObject.phone, oldObject.personNummer);
        }

        public static int DeleteEmployee(EmployeeObject oldObject)
        {
            int busrideCount = 0;
            busrideCount = ExecuteAndGetScalar("SELECT count(*) FROM busride WHERE busride.driver_id = :p0;", oldObject.personNummer);

            int affectedRows = -1;

            MessageBoxResult result = MessageBox.Show("If you remove this empolyee the employee also disappears as driver on a total of " + busrideCount + " busrides! (But the busrides remain and are not removed.) Do you still wish to remove this employee?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ExecuteAndGetNonQuery("UPDATE busride SET driver_id = NULL WHERE driver_id = :p0;", oldObject.personNummer);
                affectedRows = ExecuteAndGetNonQuery("DELETE FROM employee WHERE social_security_nr = :p0;", oldObject.personNummer);
            }
            return affectedRows;
        }

        #endregion employee

    }
}
