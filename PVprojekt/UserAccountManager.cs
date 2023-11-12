using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace PVprojekt
{
    internal class UserAccountManager
    {
        public UserAccountManager(SqlConnection connection)
        {

        }

        //Vytvoření nového uživatele
        public bool CreateUserAccount()
        {
            //Získání parametrů od uživatele
            Console.WriteLine("Zadejte jméno:");
            string jmeno = Console.ReadLine();
            Console.WriteLine("Zadejte příjmení:");
            string prijmeni = Console.ReadLine();
            Console.WriteLine("Zadejte datum narození (formát: yyyy-MM-dd):");
            string datumNarozeni = Console.ReadLine();
            Console.WriteLine("Zadejte plat:");
            int plat = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Zadejte oddělení:");
            string oddeleni = Console.ReadLine();
            Console.WriteLine("Zadejte uživatelskou roli:");
            string userRole = Console.ReadLine();
            Console.WriteLine("Zadejte heslo:");
            string heslo = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                heslo += key.KeyChar;
                Console.Write("*");
            }
            Console.WriteLine();

            string connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO UserAccount (jmeno, prijmeni, datum_narozeni, plat, oddeleni, user_role, heslo) VALUES (@Jmeno, @Prijmeni, @DatumNarozeni, @Plat, @Oddeleni, @UserRole, @Heslo)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Přidání parametrů do dotazu
                        command.Parameters.AddWithValue("@Jmeno", jmeno);
                        command.Parameters.AddWithValue("@Prijmeni", prijmeni);
                        command.Parameters.AddWithValue("@DatumNarozeni", datumNarozeni);
                        command.Parameters.AddWithValue("@Plat", plat);
                        command.Parameters.AddWithValue("@Oddeleni", oddeleni);
                        command.Parameters.AddWithValue("@UserRole", userRole);
                        command.Parameters.AddWithValue("@Heslo", heslo);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Uživatelský účet byl úspěšně přidán.");
                        }

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při vytváření nového uživatelského účtu: " + ex.Message);
                return false;
            }
        }

        //Smazání uživateůe
        public bool DeleteUserAccount(string username)
        {
            String connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM UserAccount WHERE jmeno = @Username";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Přidání parametrů do dotazu
                        command.Parameters.AddWithValue("@Username", username);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při mazání uživatelského účtu: " + ex.Message);
                return false;
            }
        }

        //Smazání uživatele dle parametru z metody DeleteUserAccount
        public void DeleteUserAccountByInput()
        {
            String connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("Zadejte jméno uživatele, kterého chcete smazat:");
                    string username = Console.ReadLine();

                    bool success = DeleteUserAccount(username);

                    if (success)
                    {
                        Console.WriteLine("Uživatelský účet byl úspěšně smazán.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při mazání uživatelského účtu: " + ex.Message);
            }
        }

        //Aktualizace uživatelského účtu
        public void UpdateUserAccount()
        {
            String connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("Zadejte jméno uživatele, kterého chcete aktualizovat:");
                    string jmeno = Console.ReadLine();

                    string query = "SELECT * FROM UserAccount WHERE jmeno = @Jmeno";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Přidání parametrů do dotazu
                        command.Parameters.AddWithValue("@Jmeno", jmeno);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();

                                Console.WriteLine("Aktualizujte údaje uživatele:");
                                Console.WriteLine("Příjmení:");
                                string prijmeni = Console.ReadLine();
                                Console.WriteLine("Datum narození (yyyy-MM-dd):");
                                DateTime datumNarozeni = DateTime.Parse(Console.ReadLine());
                                Console.WriteLine("Plat:");
                                decimal plat = decimal.Parse(Console.ReadLine());
                                Console.WriteLine("Oddělení:");
                                string oddeleni = Console.ReadLine();
                                Console.WriteLine("Role:");
                                string userRole = Console.ReadLine();

                                reader.Close();

                                query = "UPDATE UserAccount SET prijmeni = @Prijmeni, datum_narozeni = @DatumNarozeni, plat = @Plat, oddeleni = @Oddeleni, user_role = @UserRole WHERE jmeno = @Jmeno";

                                using (SqlCommand updateCommand = new SqlCommand(query, connection))
                                {
                                    // Přidání parametrů do aktualizačního dotazu
                                    updateCommand.Parameters.AddWithValue("@Prijmeni", prijmeni);
                                    updateCommand.Parameters.AddWithValue("@DatumNarozeni", datumNarozeni);
                                    updateCommand.Parameters.AddWithValue("@Plat", plat);
                                    updateCommand.Parameters.AddWithValue("@Oddeleni", oddeleni);
                                    updateCommand.Parameters.AddWithValue("@UserRole", userRole);
                                    updateCommand.Parameters.AddWithValue("@Jmeno", jmeno);

                                    updateCommand.ExecuteNonQuery();
                                }

                                Console.WriteLine("Uživatelský účet byl aktualizován.");
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
                Console.WriteLine("Chyba při aktualizaci uživatelského účtu: " + ex.Message);
            }
        }

        //Zobrazení všech uživatelských účtů
        public void ShowAllUserAccounts()
        {
            String connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM UserAccount";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("Všechny záznamy v tabulce UserAccount:");

                                while (reader.Read())
                                {
                                    Console.WriteLine($"ID: {reader["id"]}");
                                    Console.WriteLine($"Jméno: {reader["jmeno"]}");
                                    Console.WriteLine($"Příjmení: {reader["prijmeni"]}");
                                    Console.WriteLine($"Datum narození: {reader["datum_narozeni"]}");
                                    Console.WriteLine($"Plat: {reader["plat"]}");
                                    Console.WriteLine($"Oddělení: {reader["oddeleni"]}");
                                    Console.WriteLine($"Role: {reader["user_role"]}");
                                    Console.WriteLine("-");
                                }
                            }
                            else
                            {
                                Console.WriteLine("V tabulce UserAccount nejsou žádné záznamy.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při zobrazování záznamů v tabulce UserAccount: " + ex.Message);
            }
        }

        //Zobrazení všech směn
        public void ShowAllShifts()
        {
            String connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Smena";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("Všechny záznamy v tabulce Smena:");

                                while (reader.Read())
                                {
                                    Console.WriteLine($"ID: {reader["id"]}");
                                    Console.WriteLine($"Datum: {reader["datum"]}");
                                    Console.WriteLine($"Začátek: {reader["zacatek"]}");
                                    Console.WriteLine($"Konec: {reader["konec"]}");
                                    Console.WriteLine("-");
                                }
                            }
                            else
                            {
                                Console.WriteLine("V tabulce Smena nejsou žádné záznamy.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při zobrazování záznamů v tabulce Smena: " + ex.Message);
            }
        }
    }
}