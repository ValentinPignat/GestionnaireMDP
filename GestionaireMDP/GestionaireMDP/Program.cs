using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GestionaireMDP
{
    internal class Program
    {

        static void Main(string[] args)
        {
            // Separator used in the txt file
            const char SEPARATOR = ' ';

            // Main menu
            const string MENU = "*******************************************\n"+
                                "Sélectionnez une action\n" +
                                "1: Consulter un mot de passe\n" +
                                "2: Ajouter un mot de passe\n" +
                                "3: Supprimer un mot de passe\n" +
                                "4: Modifier un mot de passe\n" +
                                "5: Quitter le programme\n" +
                                "*******************************************\n\n" +
                                "Faites votre choix :";

            // Modify menu
            const string MODIFY_MENU = "*******************************************\n" +
                                "Sélectionnez une action\n" +
                                "1: Modifier le site\n" +
                                "2: Modifier le username\n" +
                                "3: Modifier le mot de passe\n" +
                                "4: Retour au menu principal\n" +
                                "*******************************************\n\n";

            // Index of each element in an entry string[site, username, password]
            const int SITE_INDEX = 0;
            const int USERNAME_INDEX = 1;
            const int PASSWORD_INDEX = 2;

            // user input variable
            ConsoleKey input;

            // Ensemble du contenu du fichier text
            string content = "";

            // List of lines and list of passwords string[site, username, password]
            List<string> lines = new List<string>();
            List<string[]> entries = new List<string[]>();

            
            // File Path
            const string PATH = @"C:\Users\pe41oyl\Desktop\passwords.txt";

            // If file doesn't exist create it
            while (!File.Exists(PATH))
            {
                try
                {
                    File.Create(PATH);
                }
                // If the path is incorect, warning and exit
                catch {
                        Console.WriteLine("Le chemin de fichier par défaut n'existe pas");
                        Console.ReadKey();
                        Environment.Exit(0);
                
                }
            }

            // Get txt file content as a string and then split it into lines 
            content = File.ReadAllText(PATH);
            lines = content.Split('\n').ToList();

            // For each of the lines split it in 3 strings (site, username, password) using the SEPARATOR
            foreach (string line in lines)
            {
                string[] fullEntry = line.TrimEnd().Split(SEPARATOR);

                // If an entry isn't conform, it'll not be saved in the entries and thus removed at the next update
                if (fullEntry.Length == 3) {
                    Console.WriteLine(line);
                    entries.Add(fullEntry);
                }
            }

            // Main menu loop - Start
            while (true)
            {

                // Clear, displays menu and take input
                Console.Clear();
                Console.WriteLine(MENU);
                input = Console.ReadKey().Key;

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
                Update();
            }
            // Main menu loop - End

            void AddPassword() {
                string site;
                string username;
                string password;

                Console.WriteLine("Veuillez entrer un site: ");
                site = Console.ReadLine();
                if (CheckExisting() >= 0) {
                    Console.WriteLine("Ce site a déjà un enregistrement");
                    return;
                };

                Console.WriteLine("Veuillez entrer un identifiant / username: ");
                username = Console.ReadLine();
                Console.WriteLine("Veuillez entrer un mot de passe: ");
                password = Console.ReadLine();
                entries.Add(new string[] { site, username, password } );
            }

            void RemovePassword() {
                Console.WriteLine("Veuillez entrer le site que vous voulez supprimer:");
                int index = CheckExisting();
                if (index >= 0)
                {
                    Console.WriteLine(DisplayEntry(index));
                    entries.RemoveAt(index);
                }
                else
                {
                    Console.WriteLine("Aucune entrée pour ce site n'a été trouvée");
                }
                Console.WriteLine("Appuyer sur Enter pour revenir au menu");
                do { input = Console.ReadKey().Key; } while (input != ConsoleKey.Enter);

            }

            void ModifyPassword() {
                Console.Clear();
                Console.WriteLine("Veuillez entrer le site que vous voulez modifier:");
                int index = CheckExisting();
                if (index >= 0)
                {
                    while (true) {
                        Console.Clear();
                        Console.WriteLine(DisplayEntry(index));
                        Console.WriteLine(MODIFY_MENU);
                        input = Console.ReadKey().Key;
                        switch (input)
                        {
                            case ConsoleKey.D1:
                                Console.WriteLine("Veuillez entrer le nouveau site: ");
                                string site = Console.ReadLine();
                                entries[index][SITE_INDEX] = site;
                                break;
                            case ConsoleKey.D2:
                                Console.WriteLine("Veuillez entrer le nouveau username: ");
                                string username = Console.ReadLine();
                                entries[index][USERNAME_INDEX] = username;
                                break;
                            case ConsoleKey.D3:
                                Console.WriteLine("Veuillez entrer le nouveau mot de passe: ");
                                string password = Console.ReadLine();
                                entries[index][PASSWORD_INDEX] = password;
                                break;
                            case ConsoleKey.D4:
                                return;
                                break;
                            default:
                                break;
                        }
                        Update();
                    }
                    
                }
                else
                {
                    Console.WriteLine("Aucune entrée pour ce site n'a été trouvée");

                }
                Console.WriteLine("Appuyer sur Enter pour revenir au menu");
                do { input = Console.ReadKey().Key; } while (input != ConsoleKey.Enter);

            }

            void Update() {
                content = "";
                foreach (string[] entry in entries) {

                    foreach (string item in entry) {
                        content += item;
                        content += SEPARATOR;
                    }
                    content += "\n";
                    
                }
                File.WriteAllText(PATH, content);
            }

            int CheckExisting() {
                string site = Console.ReadLine();
                int index = 0;
                foreach (string[] entry in entries) {
                    if (entry[SITE_INDEX] == site) {
                        return entries.IndexOf(entry);
                    }
                }
                return -1;
            }

            void GetPassword() {

                int index = CheckExisting();
                if (index >= 0)
                {
                    Console.WriteLine(DisplayEntry(index));
                }
                else { 
                    Console.WriteLine("Aucune entrée pour ce site n'a été trouvée");
                
                }
                Console.WriteLine("Appuyer sur Enter pour revenir au menu");
                do { input = Console.ReadKey().Key; } while (input != ConsoleKey.Enter);

            }

            string DisplayEntry(int index) { 
                return "Site: " + entries[index][SITE_INDEX] + "\n" +
                    "Username : " + entries[index][USERNAME_INDEX] + "\n" +
                    "Mot de passe: " + entries[index][PASSWORD_INDEX] + "\n";
            
            }
        }
    }
}
