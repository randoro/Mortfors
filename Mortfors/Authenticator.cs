using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors
{
    static class Authenticator
    {
        public static User currentUser;
        public static bool authenticated;

        public static string errorMessage = "";


        public static bool Login(string username, string rawpassword)
        {
            if(username == "" || rawpassword == "")
            {
                errorMessage = "Username or password field is empty.";
                authenticated = false;
                return false;
            }

            string hashedPassword = SimpleHash.GenerateHashedPassword(username, rawpassword);
            currentUser = null;

            if (IsValidEmail(username))
            {
                //resenar login
                bool boolFound = false;
                currentUser = DBConnection.ConnectVerifyResenar(username, hashedPassword, out boolFound, out errorMessage);
                if (currentUser != null && boolFound)
                {
                    authenticated = true;
                    Console.WriteLine("Resenar " + username + " is now authenticated!");

                    return true;
                }
            }
            else
            {
                //anstalld login
                bool boolFound = false;
                currentUser = DBConnection.ConnectVerifyAnstalld(username, hashedPassword, out boolFound, out errorMessage);
                if(currentUser != null && boolFound)
                {
                    authenticated = true;
                    Console.WriteLine("Anstalld " + username + " is now authenticated!");



                    return true;
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
