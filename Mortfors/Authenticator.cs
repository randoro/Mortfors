using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors
{
    static class Authenticator
    {
        private static string username;
        private static string hashedPassword;
        private static bool authenticated;


        public static bool Login(string username, string rawpassword)
        {
            if(username == "" || rawpassword == "")
            {
                authenticated = false;
                return false;
            }

            Authenticator.username = username;
            Authenticator.hashedPassword = SimpleHash.GenerateHashedPassword(username, rawpassword);

            if(IsValidEmail(username))
            {
                //resenar login
                if(DBConnection.ConnectVerifyUser("SELECT * from resenar WHERE email = :username AND losenord = :hashedPassword", username, hashedPassword))
                {
                    Console.WriteLine("User " + username + " is now authenticated!");
                }
            }
            else
            {
                //anstalld login
                if (DBConnection.ConnectVerifyUser("SELECT * from anstalld WHERE pers_nr = :username AND losenord = :hashedPassword", username, hashedPassword))
                {
                    Console.WriteLine("Anstalld " + username + " is now authenticated!");
                }
            }

            return false;
        }


        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }



        
    }
}
