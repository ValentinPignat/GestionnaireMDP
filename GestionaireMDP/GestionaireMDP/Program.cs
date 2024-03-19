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
            const string MENU = "*******************************************\n"+
                                "Sélectionnez une action\n" +
                                "1: Consulter un mot de passe\n" +
                                "2: Ajouter un mot de passe\n" +
                                "3: Supprimer un mot de passe\n" +
                                "4: Quitter le programme\n" +
                                "*******************************************\n\n" +
                                "Faites votre choix :";
            ConsoleKey input;
            int parsed;
            string content = "";

            const string PATH = @"C:\Users\pe41oyl\Desktop\passwords.txt";
            //StreamWriter sw = new StreamWriter(PATH, true, Encoding.ASCII);

            // While used to avoid continuing unit file creation is complete
            while (!File.Exists(PATH))
            {
                File.Create(PATH);
            }
            
            content = File.ReadAllText(PATH);
            Console.WriteLine(content);
            

            
            


            while (true)
            {
                //Console.Clear();
                Console.WriteLine(MENU);
                input = Console.ReadKey().Key;
                 switch (input) { 
                    case ConsoleKey.D1:
                        AddPassword();
                        break;
                    case ConsoleKey.D2:
                        break;
                    case ConsoleKey.D3:
                        break;
                    case ConsoleKey.D4:
                        break;
                    default:
                        break;


                }
                
                
            }
            void AddPassword() {
                content += "Login test\n" +
                            "PW test\n";
                File.WriteAllText(PATH, content);
                
                
            }
        }
    }
}
