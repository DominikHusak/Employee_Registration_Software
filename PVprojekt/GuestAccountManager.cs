using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVprojekt
{
    internal class GuestAccountManager
    {

        public GuestAccountManager(SqlConnection connection)
        {

        }

        //Zobrazení profilu uživatele
        public void DisplayUserProfile(string username)
        {
            String connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT jmeno, prijmeni, datum_narozeni, plat, oddeleni, user_role FROM UserAccount WHERE jmeno = @Username";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader.GetString(0);
                                string lastName = reader.GetString(1);
                                DateTime date = reader.GetDateTime(2);
                                int salary = reader.GetInt32(3);
                                string department = reader.GetString(4);
                                string role = reader.GetString(5);

                                Console.WriteLine("Jméno: " + firstName);
                                Console.WriteLine("Příjmení: " + lastName);
                                Console.WriteLine("Datum narození: " + date.ToString("yyyy.MM.dd"));
                                Console.WriteLine("Plat: " + salary);
                                Console.WriteLine("Oddeleni: " + department);
                                Console.WriteLine("Role: " + role);
                            }
                            else
                            {
                                Console.WriteLine("Uživatel nebyl nalezen.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při zobrazování profilu uživatele: " + ex.Message);
            }
        }

        //Změna hesla
        public void ChangeGuestPassword(string username)
        {
            Console.WriteLine("Zadejte nové heslo:");
            string newPassword = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                newPassword += key.KeyChar;
                Console.Write("*");
            }
            Console.WriteLine();

            try
            {
                string connectionString = Setting.connectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE UserAccount SET heslo = @Password WHERE jmeno = @Username";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Password", newPassword);
                        command.Parameters.AddWithValue("@Username", username);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Heslo bylo úspěšně změněno.");
                        }
                        else
                        {
                            Console.WriteLine("Nepodařilo se změnit heslo.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při změně hesla: " + ex.Message);
            }
        }

        //Zobrazení směn
        public void DisplayShift(string username)
        {
            string connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT s.cislo_skladu, s.den_smeny, s.cas_smeny FROM Smena s INNER JOIN UserAccount u ON s.user_id = u.id WHERE u.jmeno = @Username";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int cisloSkladu = reader.GetInt32(0);
                                DateTime denSmeny = reader.GetDateTime(1);
                                TimeSpan casSmeny = reader.GetTimeSpan(2);

                                Console.WriteLine("Číslo skladu: " + cisloSkladu);
                                Console.WriteLine("Datum směny: " + denSmeny.ToString("yyyy.MM.dd"));
                                Console.WriteLine("Čas směny: " + casSmeny.ToString(@"hh\:mm\:ss"));
                            }
                            else
                            {
                                Console.WriteLine("Směna nebyla nalezena.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při zobrazování detailů směny: " + ex.Message);
            }
        }
    }
}
