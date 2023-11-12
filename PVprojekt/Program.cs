using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace PVprojekt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Instance třídy connection pomocí řetězce z Settings.cs
            string connectionString = Setting.connectionString;
            SqlConnection connection = new SqlConnection(connectionString);

            //Instance třídy LoginLogic s připojením k databázi na které je volaná metoda Login
            LoginLogic loginLogic = new LoginLogic(connection);
            if (loginLogic.Login())
            {
                Console.ReadKey();
            }
        }
    }
}