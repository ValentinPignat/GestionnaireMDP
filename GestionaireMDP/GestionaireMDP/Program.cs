/// ETML
/// Author : Valentin Pignat 
/// Date (creation): 19.03.2024
/// Description:


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// TODO 
// Add Base constructor to key one
// MAIN IN A CLASS ???
// Dont take control character input  => multiple input check regex 
// Manage wrong file path -- alternative
// Add config if time 
//
// Improve input/ menu fluidity
// Return when creating
// Watch another entry

// Visual
//
//

namespace GestionaireMDP
{

    internal class Program
    {
        public const int INDEX_NOT_FOUND = -1;

        static void Main(string[] args)
        {

            // Line
            const string LINE = "*******************************************";
            // Main menu
            const string MENU = "*******************************************\n"+
                                "Sélectionnez une action\n" +
                                "1: Consulter un mot de passe\n" +
                                "2: Ajouter un mot de passe\n" +
                                "3: Supprimer un mot de passe\n" +
                                "4: Modifier un mot de passe\n" +
                                "5: Quitter le programme\n" +
                                "*******************************************\n\n" +
                                "Faites votre choix : ";

            // Modify menu
            const string MODIFY_MENU = "*******************************************\n" +
                                "Sélectionnez une action\n" +
                                "1: Modifier le site\n" +
                                "2: Modifier le username\n" +
                                "3: Modifier le mot de passe\n" +
                                "4: Retour au menu principal\n" +
                                "*******************************************\n\n";

            
            // user input variable
            ConsoleKey userInput;

            // Initialize password manager
            PWManager pwManager = new PWManager();

            
            // Main menu loop - Start
            while (true)
            {

                // Clear, displays menu and take input
                Console.Clear();
                Console.Write(MENU);
                userInput = Console.ReadKey().Key;
                Console.WriteLine();

                // Switch input and start the designated method or exit
                switch (userInput)
                {
                    case ConsoleKey.D1:
                        GetPassword();
                        break;
                    case ConsoleKey.D2:
                        AddPassword();
                        break;
                    case ConsoleKey.D3:
                        RemovePassword();
                        break;
                    case ConsoleKey.D4:
                        ModifyPassword();
                        break;
                    case ConsoleKey.D5:
                        return;
                    default:
                        break;

                }
                // File is constantly updated after each loop
                pwManager.Update();
            }
            // Main menu loop - End


            void AddPassword() {
                string siteTemp;
                string usernameTemp;
                string passwordTemp;

                Console.WriteLine("\nVeuillez entrer un site (vide pour annuler): ");
                siteTemp = Console.ReadLine();
                if (siteTemp == "") {
                    Console.WriteLine("Ajout annulé (Appuyer sur enter)");
                    Console.ReadKey();
                    return;
                }

                if (pwManager.IndexFromSite(site:siteTemp) != INDEX_NOT_FOUND) {
                    Console.WriteLine("Ce site a déjà un enregistrement (Appuyer sur enter)");
                    Console.ReadLine();
                    return;
                };

                Console.WriteLine("\nVeuillez entrer un identifiant / username (vide pour annuler): ");
                usernameTemp = Console.ReadLine();
                if (usernameTemp == "")
                {
                    Console.WriteLine("Ajout annulé (Appuyer sur enter)");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("\nVeuillez entrer un mot de passe (vide pour annuler): ");
                passwordTemp = Console.ReadLine();
                if (passwordTemp == "")
                {
                    Console.WriteLine("Ajout annulé (Appuyer sur enter)");
                    Console.ReadKey();
                    return;
                }
                pwManager.AddPW(site: siteTemp, username: usernameTemp, password: passwordTemp);
            }

            void RemovePassword() {
                int index = SiteSelection();
                if (index == INDEX_NOT_FOUND)
                {
                    return;
                }
                else
                {
                    Console.WriteLine("\n" + pwManager.DisplayEntry(index));
                    pwManager.RemovePW(index: index);
                }

                Console.WriteLine("=> Site retiré\n\nAppuyer sur Enter pour revenir au menu");
                do { userInput = Console.ReadKey().Key; } while (userInput != ConsoleKey.Enter);

            }

            void ModifyPassword() {
               
                Console.Clear();
                int index = SiteSelection();
                if (index == INDEX_NOT_FOUND)
                {
                    return;
                }
                else
                {
                    while (true) {
                        Console.Clear();
                        Console.WriteLine(MODIFY_MENU);
                        Console.WriteLine(pwManager.DisplayEntry(index));
                        Console.Write("Faites votre choix: ");
                        userInput = Console.ReadKey().Key;
                        Console.WriteLine();

                        switch (userInput)
                        {
                            case ConsoleKey.D1:
                                Console.WriteLine("Veuillez entrer le nouveau site: ");
                                string newSite = Console.ReadLine();
                                pwManager.ChangeSite(index: index, newValue: newSite);
                                break;
                            case ConsoleKey.D2:
                                Console.WriteLine("Veuillez entrer le nouveau username: ");
                                string newUsername = Console.ReadLine();
                                pwManager.ChangeUsername(index: index, newValue: newUsername);
                                break;
                            case ConsoleKey.D3:
                                Console.WriteLine("Veuillez entrer le nouveau mot de passe: ");
                                string newPassword = Console.ReadLine();
                                pwManager.ChangePassword(index: index, newValue: newPassword);
                                break;
                            case ConsoleKey.D4:
                                return;
                            default:
                                break;
                        }
                        pwManager.Update();
                    }                  
                }
            }


            void GetPassword() {

                do
                {
                    int index = SiteSelection();

                    if (index == INDEX_NOT_FOUND)
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("\n" + pwManager.DisplayEntry(index));
                        Console.WriteLine("Appuyer sur Enter pour revenir à la selection");
                        userInput = Console.ReadKey().Key; 
                    }
                } while (true);

            }


            int SiteSelection() {
                ConsoleKeyInfo k;
                List <string> sites = new List <string>();

                do {
                    Console.Clear();
                    Console.WriteLine(LINE);
                    sites = pwManager.GetSiteList();
                    for (int i = 0; i < sites.Count; i++)
                    {
                        Console.WriteLine((i+1)+ ". " + sites[i]);
                    }
                    Console.WriteLine(LINE + "\n");
                    Console.Write("Faites votre choix (Enter pour revenir au menu principale): ");
                     k = Console.ReadKey();
                    Console.WriteLine();

                    if (int.TryParse(k.KeyChar.ToString(), out int index))
                    {

                        index--;
                        if (index >= 0 && index < sites.Count)
                        {
                            return index;
                        }
                    }                    
                    
                } while (k.Key != ConsoleKey.Enter);

                return INDEX_NOT_FOUND;   
            }

  


        }
    }
}
