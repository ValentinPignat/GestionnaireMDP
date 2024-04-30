/// ETML
/// Author : Valentin Pignat 
/// Date (creation): 19.03.2024
/// Description:


using System;
using System.Collections.Generic;
using System.IO;

// TODO 
// Add Base constructor to key one
// MAIN IN A CLASS 
// Dont take control character input  => multiple input check regex ???
// Manage wrong file path -- alternative
// Add config if time 
// See CDC (check missing)
// Move main to menu ?
// Improve input/ menu fluidity
// Comment test better

namespace GestionaireMDP
{

    internal class Program
    {
        public const int INDEX_NOT_FOUND = -1;

        static void Main(string[] args)
        {
           // Unfound index


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
            ConsoleKey input;

            // Initialize password manager
            PWManager pwManager = new PWManager();

            // TEST
            /*
            while (true) {
                //string test = Console.ReadLine();
                //Console.Write(ReverseKey(test));
                string S = Console.ReadLine();
                string reversed = pwManager.Vigenere(toVig: S);
                string back = pwManager.Vigenere(toVig: S, reversed: true);

            }*/
            
            // Main menu loop - Start
            while (true)
            {

                // Clear, displays menu and take input
                Console.Clear();
                Console.Write(MENU);
                input = Console.ReadKey().Key;
                Console.WriteLine();

                // Switch input and start the designated method or exit
                switch (input)
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
                        break;
                    default:
                        break;

                }
                // File is constantly updated after each loop
                pwManager.Update();
            }
            // Main menu loop - End


            /// <summary>
            /// Get a master password from user
            /// </summary>
            /// <returns></returns>
            string PromptMasterPW()
            {
                Console.WriteLine("Veuillez entrer votre mot de passe: ");
                string masterPW = Console.ReadLine();
                return masterPW;
            }

            void AddPassword() {
                string siteTemp;
                string usernameTemp;
                string passwordTemp;

                Console.WriteLine("Veuillez entrer un site: ");
                siteTemp = Console.ReadLine();
                if (pwManager.IndexFromSite(site:siteTemp) != INDEX_NOT_FOUND) {
                    Console.WriteLine("Ce site a déjà un enregistrement");
                    Console.ReadLine();
                    return;
                };

                Console.WriteLine("Veuillez entrer un identifiant / username: ");
                usernameTemp = Console.ReadLine();
                Console.WriteLine("Veuillez entrer un mot de passe: ");
                passwordTemp = Console.ReadLine();
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
                    Console.WriteLine(pwManager.DisplayEntry(index));
                    pwManager.RemovePW(index: index);
                }

                Console.WriteLine("Appuyer sur Enter pour revenir au menu");
                do { input = Console.ReadKey().Key; } while (input != ConsoleKey.Enter);

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
                        input = Console.ReadKey().Key;
                        Console.WriteLine();

                        switch (input)
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
                
                /*Console.WriteLine("Appuyer sur Enter pour revenir au menu");
                do { input = Console.ReadKey().Key; } while (input != ConsoleKey.Enter);*/

            }





            void GetPassword() {

                int index = SiteSelection();

                if (index == INDEX_NOT_FOUND)
                {
                    return;
                }
                else {
                    Console.WriteLine(pwManager.DisplayEntry(index));
                    Console.WriteLine("Appuyer sur Enter pour revenir au menu");
                    do { input = Console.ReadKey().Key; } while (input != ConsoleKey.Enter);
                }


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
