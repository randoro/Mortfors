using Mortfors.SQLObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortfors.Login
{
    static class Authenticator
    {
        public static UserObject currentUser;
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
                //traveller login
                currentUser = null;
                currentUser = DBConnection.VerifyTraveller(username, hashedPassword);
                if (currentUser != null)
                {
                    authenticated = true;
                    Console.WriteLine("Traveller " + username + " is now authenticated!");

                    return true;
                }
            }
            else
            {
                //employee login
                currentUser = null;
                currentUser = DBConnection.VerifyEmployee(username, hashedPassword);
                if(currentUser != null)
                {
                    authenticated = true;
                    Console.WriteLine("Employee " + username + " is now authenticated!");



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


        public static string GetUserInfo()
        {
            string info = "";
            if(currentUser.GetType() == typeof(TravellerObject))
            {
                info += "Traveller "+ currentUser.name+ " logged in with email "+((TravellerObject)currentUser).email;
            } else if (currentUser.GetType() == typeof(EmployeeObject))
            {
                info += "Employee ";
                if(((EmployeeObject)currentUser).isAdmin)
                {
                    info += "Admin ";
                }

                info += currentUser.name+ " logged in with social security number " + ((EmployeeObject)currentUser).personNummer;
            }
            return info;
        }



        
    }
}
