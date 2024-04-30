/// ETML
/// Author : Valentin Pignat 
/// Date (creation): 23.04.2024
/// Description:
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestVigenere")]

namespace GestionaireMDP
{
    internal class PWManager
    {

        /// <summary>
        /// Size of ASCII extended table
        /// </summary>
        const int ACII_EXT_SIZE = 256;
        /// <summary>
        /// Index of PW index
        /// </summary>
        const int SITE_INDEX = 0;

        /// <summary>
        /// Index of PW username
        /// </summary>
        const int USERNAME_INDEX = 1;

        /// <summary>
        /// Index of PW password
        /// </summary>
        const int PASSWORD_INDEX = 2;

        /// <summary>
        /// Path of the text file
        /// </summary>
        const string PATH = @"C:\Users\pe41oyl\Desktop\passwords.txt";

        /// <summary>
        /// Seperator used in the file
        /// </summary>
        const char SEPARATOR = '\t';


        /// <summary>
        /// List of printable characters 
        /// </summary>
        private List<Char> _printableChars = new List<char>();

        /// <summary>
        /// List of entries [Index, Username, Password]
        /// </summary>
        private List<string[]> entries = new List<string[]>();

        /// <summary>
        /// master password used to encrypt/decrypt 
        /// </summary>
        readonly string _masterPW;


        /// <summary>
        /// PWManager constructor with given master password
        /// </summary>
        /// <param name="masterPassword">Password used for en/decrypting</param>
        public PWManager(string masterPassword) {
            _printableChars = GetPrintableChars();

            _masterPW = masterPassword;

            // If file doesn't exist, exit TODO
            if (!FindFile())
            {
                Environment.Exit(0);
            }
            else {
                ImportPW();
            }
        }

        /// <summary>
        /// PWManager construtor that promps for master password in console
        /// </summary>
        public PWManager()
        {
            _printableChars = GetPrintableChars();

            _masterPW = PromptMasterPW();

            // If file doesn't exist, exit TODO
            if (!FindFile())
            {
                Environment.Exit(0);
            }
            else
            {
                ImportPW();
            }
        }

        /// <summary>
        /// Get a master password from user
        /// </summary>
        /// <returns></returns>
        private string PromptMasterPW()
        {
            Console.WriteLine("Veuillez entrer votre mot de passe: ");
            string masterPW = Console.ReadLine();
            return masterPW;
        }

        /// <summary>
        /// Return a list of printable ASCII characters
        /// Source: https://stackoverflow.com/questions/887377/how-do-i-get-a-list-of-all-the-printable-characters-in-c
        /// Adapted with the help of Sebastien Tille : Avoid looping to char.MaxValue(0xFFFF), instead loop to ASCII_EXT_SIZE.
        /// </summary>
        /// <returns>List of printable characters</returns>
        public List<char> GetPrintableChars() {

                
                        List<char> pChars = new List<Char>();

                        // For each char..
                        for (int i = 0; i < ACII_EXT_SIZE; i++)
                        {
                            char c = Convert.ToChar(i);

                            // ..if it isnt't a Control..
                            if (!char.IsControl(c))
                            {
                                // ..add it to the list
                                pChars.Add(c);
                            }
                        }
                        return pChars;
        }

        /// <summary>
        /// Check if the file path exists, else try to create it
        /// </summary>
        /// <returns>false if the file wasn't created</returns>
        private bool FindFile() {
            // If file doesn't exist ...
            while (!File.Exists(PATH))
            {
                // ... try to create it
                try
                {
                    // https://stackoverflow.com/questions/5156254/closing-a-file-after-file-create
                    // https://stackoverflow.com/questions/66537978/c-sharp-system-io-ioexception-file-cannot-be-accessed-because-it-is-accessed-by
                    // Create and CLOSE file to avoid used by other process error when reading content
                    FileStream file = File.Create(PATH);
                    file.Close();
                }
                // If the path is incorect, warning and exit eith false
                catch
                {
                    Console.WriteLine("Le chemin de fichier par défaut n'existe pas");
                    Console.ReadKey();
                    return false;
                }
            }

            // If file found or created return true
            return true;
        }

        /// <summary>
        /// Get the content of the password file and put its content in Lists
        /// </summary>
        private void ImportPW()
        {
            string _content = "";
            List<string> lines = new List<string>();

            // Get txt file content as a string and then split it into lines 
            _content = File.ReadAllText(PATH);
            lines = _content.Split('\n').ToList();

            // For each of the lines split it in 3 strings (site, username, password) using the SEPARATOR
            foreach (string line in lines)
            {
                string[] fullEntry = line.TrimEnd().Split(SEPARATOR);

                // If an entry isn't conform, it'll not be saved in the entries and thus removed at the next update
                if (fullEntry.Length == 3)
                {
                    Console.WriteLine(line);
                    entries.Add(fullEntry);
                }
            }
        }

        /// <summary>
        /// Encrypt string with a key
        /// </summary>
        /// <param name="toVig">string to encrypt</param>
        /// <param name="reversed">true to decrypt</param>
        /// <param name="key">key to encrypt </param>
        /// <returns></returns>
        public string Vigenere(string toVig, bool reversed = false)
        {
            string key = _masterPW;

            // If we want to decrypt, reverse key
            if (reversed) {
                key = ReverseKey(key);
            }

            string crypted = "";



            for (int i = 0; i < toVig.Length; i++)
            {
                crypted += _printableChars[(_printableChars.IndexOf(toVig[i]) + _printableChars.IndexOf(key[i % key.Length])) % (_printableChars.Count - 1)];
            }
            return crypted;
        }

        /// <summary>
        /// Reverse given string (complement to last character)
        /// </summary>
        /// <param name="key">key to reverse</param>
        /// <returns>reversed key</returns>
        public string ReverseKey(string key)
        {
            string reversed = "";

            // Foreach char, replace by char(Max index - char index)
            foreach (char c in key)
            {
                // Count -1 for last Index
                reversed += _printableChars[_printableChars.Count - 1 - _printableChars.IndexOf(c)];
            }

            return reversed;

        }

        public void Update()
        {
            string updatedContent = "";
            foreach (string[] entry in entries)
            {

                foreach (string item in entry)
                {
                    updatedContent += item;
                    updatedContent += SEPARATOR;
                }
                updatedContent += "\n";

            }
            File.WriteAllText(PATH, updatedContent);
        }

        public List<string> GetSiteList() {
            List<string> sites = new List<string>();
            for (int i = 0; i < entries.Count; i++)
            {
                sites.Add(entries[i][SITE_INDEX]);
            }
            return sites;
        }

        public int IndexFromSite(string site)
        {
            foreach (string[] entry in entries)
            {
                if (entry[SITE_INDEX] == site)
                {
                    return entries.IndexOf(entry);
                }
            }
            return GestionaireMDP.Program.INDEX_NOT_FOUND;
        }

        public void AddPW(string site, string username, string password){
            entries.Add(new string[] { site, Vigenere(toVig: username), Vigenere(toVig: password) }) ;
        }

        public void RemovePW (int index) {  
            entries.RemoveAt(index) ; 
        }

        public string DisplayEntry(int index)
        {
            return "Site: " + entries[index][SITE_INDEX] + "\n" +
                "Username : " + Vigenere(toVig: entries[index][USERNAME_INDEX], reversed: true) + "\n" +
                "Mot de passe: " + Vigenere(toVig: entries[index][PASSWORD_INDEX], reversed: true) + "\n";

        }

        public void ChangePassword (int index, string newValue){
            entries[index][PASSWORD_INDEX] = Vigenere(toVig: newValue);
        }
        public void ChangeUsername(int index, string newValue) {
            entries[index][USERNAME_INDEX] = Vigenere(toVig: newValue);
        }

        public void ChangeSite(int index, string newValue) {
            entries[index][SITE_INDEX] = newValue;
        }
    }
}
