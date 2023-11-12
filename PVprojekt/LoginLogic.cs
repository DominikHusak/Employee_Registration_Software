using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PVprojekt
{
    internal class LoginLogic
    {
        private SqlConnection connection;

        public LoginLogic(SqlConnection connection)
        {
            this.connection = connection;
        }

        //Přihlášení uživatele
        public bool Login()
        {
            bool loggedIn = false;
            int failedAttempts = 0;

            try
            {
                connection.Open();
                Console.WriteLine("Připojení k databázi bylo úspěšné.");

                while (!loggedIn)
                {
                    Console.WriteLine("Zadejte uživatelské jméno:");
                    string username = Console.ReadLine();
                    Console.WriteLine("Zadejte heslo:");
                    string password = "";
                    while (true)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                    Console.WriteLine();

                    string query = "SELECT COUNT(*) FROM UserAccount WHERE jmeno = @Username AND heslo = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        int result = (int)command.ExecuteScalar();

                        if (result == 1)
                        {
                            Console.WriteLine("Přihlášení úspěšné.");

                            loggedIn = true;

                            //Kontrola role uživatele
                            //Provádění metod podle role
                            if (IsUserDbSpravce(username))
                            {
                                while (true)
                                {
                                    Console.WriteLine("Vyberte akci:");
                                    Console.WriteLine("1. Vytvořit nový záznam (create)");
                                    Console.WriteLine("2. Smazat uživatelský účet (delete)");
                                    Console.WriteLine("3. Aktualizovat uživatelský účet (update)");
                                    Console.WriteLine("4.Zobrazit všechny účty (showProfiles)");
                                    Console.WriteLine("5. Zobrazit všechny směny (showShifts)");
                                    Console.WriteLine("6. Ukončit (ukoncit)");
                                    string action = Console.ReadLine();
                                    UserAccountManager accountManager = new UserAccountManager(connection);

                                    if (action == "create")
                                    {
                                        accountManager.CreateUserAccount();
                                    }
                                    else if (action == "delete")
                                    {
                                        accountManager.DeleteUserAccountByInput();
                                    }
                                    else if (action == "update")
                                    {
                                        accountManager.UpdateUserAccount();
                                    }
                                    else if (action == "showProfiles")
                                    {
                                        accountManager.ShowAllUserAccounts();
                                    }
                                    else if (action == "showShifts")
                                    {
                                        accountManager.ShowAllShifts();
                                    }
                                    else if (action == "ukoncit")
                                    {
                                        Environment.Exit(0);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Neplatná akce.");
                                    }
                                }
                            }
                            else if (IsUserGuest(username))
                            {
                                while (true)
                                {
                                    Console.WriteLine("Vyberte akci:");
                                    Console.WriteLine("1. Zobrazit profil (profil)");
                                    Console.WriteLine("2. Změnit heslo (heslo)");
                                    Console.WriteLine("3. Zobrazit směny (shifts)");
                                    Console.WriteLine("4. Ukončit (ukoncit)");
                                    string action = Console.ReadLine();
                                    GuestAccountManager guestAaccountManager = new GuestAccountManager(connection);

                                    if (action == "profil")
                                    {
                                        guestAaccountManager.DisplayUserProfile(username);
                                    }
                                    else if (action == "heslo")
                                    {
                                        guestAaccountManager.ChangeGuestPassword(username);
                                    }
                                    else if (action == "shifts")
                                    {
                                        guestAaccountManager.DisplayShift(username);
                                    }
                                    else if (action == "ukoncit")
                                    {
                                        Environment.Exit(0);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Neplatná akce.");
                                    }
                                }
                            }
                            else if (IsUserSupervisor(username))
                            {
                                while (true)
                                {
                                    Console.WriteLine("Vyberte akci:");
                                    Console.WriteLine("1. Zobrazit profil (profil)");
                                    Console.WriteLine("2. Změnit heslo (heslo)");
                                    Console.WriteLine("3. Aktualizovat účet guest (update)");
                                    Console.WriteLine("4. Přidat směnu uživateli (addShift)");
                                    Console.WriteLine("5. Odebrat směnu uživateli (removeShift)");
                                    Console.WriteLine("6. Zobrazit všechny směny (showShifts)");
                                    Console.WriteLine("7. Ukončit (ukoncit)");
                                    string action = Console.ReadLine();
                                    SupervisorAccountManager supervisorAccountManager = new SupervisorAccountManager(connection);
                                    UserAccountManager accountManager = new UserAccountManager(connection);

                                    if (action == "profil")
                                    {
                                        supervisorAccountManager.DisplaySupervisorProfile(username);
                                    }
                                    else if (action == "heslo")
                                    {
                                        supervisorAccountManager.ChangeSupervisorPassword(username);
                                    }
                                    else if (action == "update")
                                    {
                                        supervisorAccountManager.UpdateGuestAccount();
                                    }
                                    else if (action == "addShift")
                                    {
                                        supervisorAccountManager.AddShift();
                                    }
                                    else if (action == "removeShift")
                                    {
                                        supervisorAccountManager.RemoveShift();
                                    }
                                    else if (action == "showShifts")
                                    {
                                        accountManager.ShowAllShifts();
                                    }
                                    else if (action == "ukoncit")
                                    {
                                        Environment.Exit(0);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Neplatná akce.");
                                    }
                                }

                            }
                        }
                        else
                        {
                            Console.WriteLine("Chybné uživatelské jméno nebo heslo, zkuste to znovu.");
                            failedAttempts++;

                            if (failedAttempts >= 3)
                            {
                                Console.WriteLine("Byly zadány 3 špatné pokusy o přihlášení. Zapisuji záznam o neúspěšném pokusu do souboru.");
                                using (StreamWriter sw = File.AppendText("LoginFails.txt"))
                                {
                                    sw.WriteLine($"Uživatel {username} se pokusil příhlásit. Ani jedno ze zadaných hesel nebylo správně. Pokus o přihlášení dne: {DateTime.Now}.");
                                }
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při připojování k databázi: " + ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        //Metody pro ověření zda uživatel má danou roli
        private bool IsUserDbSpravce(string username)
        {
            string query = "SELECT COUNT(*) FROM UserAccount WHERE jmeno = @Username AND user_role = 'dbSpravce'";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                int result = (int)command.ExecuteScalar();

                return result == 1;
            }
        }

        private bool IsUserGuest(string username)
        {
            string query = "SELECT COUNT(*) FROM UserAccount WHERE jmeno = @Username AND user_role = 'guest'";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                int result = (int)command.ExecuteScalar();

                return result == 1;
            }
        }

        private bool IsUserSupervisor(string username)
        {
            string query = "SELECT COUNT(*) FROM UserAccount WHERE jmeno = @Username AND user_role = 'Vedouci'";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                int result = (int)command.ExecuteScalar();

                return result == 1;
            }
        }
    }
}


