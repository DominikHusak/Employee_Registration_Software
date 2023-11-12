using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVprojekt
{
    internal class SupervisorAccountManager
    {
        public SupervisorAccountManager(SqlConnection connection)
        {

        }

        //Zobrazení profilu vedoucího
        public void DisplaySupervisorProfile(string username)
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

        //Změna hesla vedoucího
        public void ChangeSupervisorPassword(string username)
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

        //Úprava uživatele s rolí guest(pouze plat, oddělení a role)
        public void UpdateGuestAccount()
        {
            String connectionString = Setting.connectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("Zadejte jméno uživatele, kterého chcete aktualizovat:");
                    string jmeno = Console.ReadLine();

                    string query = "SELECT * FROM UserAccount WHERE jmeno = @Jmeno AND user_role = 'guest'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Jmeno", jmeno);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Jméno: {reader["jmeno"]}");
                                    Console.WriteLine($"Plat: {reader["plat"]}");
                                    Console.WriteLine($"Oddělení: {reader["oddeleni"]}");
                                    Console.WriteLine($"Role: {reader["user_role"]}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Uživatel nebyl nalezen.");
                                return;
                            }
                        }
                    }

                    Console.WriteLine("Zadejte vlastnost, kterou chcete aktualizovat (plat, oddeleni, user_role):");
                    string vlastnost = Console.ReadLine();
                    Console.WriteLine("Zadejte novou hodnotu:");
                    string novaHodnota = Console.ReadLine();

                    if (vlastnost != "plat" && vlastnost != "oddeleni" && vlastnost != "user_role")
                    {
                        Console.WriteLine("Neplatná vlastnost pro aktualizaci.");
                        return;
                    }

                    query = $"UPDATE UserAccount SET {vlastnost} = @NovaHodnota WHERE jmeno = @Jmeno AND user_role = 'guest'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NovaHodnota", novaHodnota);
                        command.Parameters.AddWithValue("@Jmeno", jmeno);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Uživatelský účet byl úspěšně aktualizován.");
                        }
                        else
                        {
                            Console.WriteLine("Aktualizace uživatelského účtu se nezdařila.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při aktualizaci uživatelského účtu: " + ex.Message);
            }
        }

        //Přidání směny
        public void AddShift()
        {
            //Získání parametrů pro přidání směny
            String connectionString = Setting.connectionString;
            Console.WriteLine("Zadejte jméno uživatele:");
            string jmeno = Console.ReadLine();
            Console.WriteLine("Zadejte číslo skladu:");
            int cisloSkladu = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Zadejte datum směny (formát: yyyy-MM-dd):");
            string shiftDate = Console.ReadLine();
            Console.WriteLine("Zadejte typ směny (formát: HH:MM:SS):");
            string shiftTime = Console.ReadLine();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Smena (user_id, cislo_skladu, den_smeny, cas_smeny) VALUES ((SELECT id FROM UserAccount WHERE jmeno = @Jmeno), @CisloSkladu, @ShiftDate, @ShiftTime)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        //Vložení parametrů do dotazu
                        command.Parameters.AddWithValue("@Jmeno", jmeno);
                        command.Parameters.AddWithValue("@CisloSkladu", cisloSkladu);
                        command.Parameters.AddWithValue("@ShiftDate", shiftDate);
                        command.Parameters.AddWithValue("@ShiftTime", shiftTime);
                        command.ExecuteNonQuery();
                        Console.WriteLine("Směna byla přidána uživateli.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Při přidávání směny došlo k chybě: " + ex.Message);
            }
        }

        //Odebrání směny
        public void RemoveShift()
        {
            //Získávání paramatru(id) pro smazání směny
            String connectionString = Setting.connectionString;
            Console.WriteLine("Zadejte ID směny:");
            int shiftId = Convert.ToInt32(Console.ReadLine());

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Smena WHERE id = @ShiftId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        //Vložení parametru(id) do dotazu pro smazání směny
                        command.Parameters.AddWithValue("@ShiftId", shiftId);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Směna byla úspěšně odebrána.");
                        }
                        else
                        {
                            Console.WriteLine("Směnu se nepodařilo odebrat.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Při odebírání směny došlo k chybě: " + ex.Message);
            }
        }
    }
}
