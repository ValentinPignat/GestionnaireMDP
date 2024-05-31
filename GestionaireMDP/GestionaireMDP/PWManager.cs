/// ETML
/// Author : Valentin Pignat 
/// Date (creation): 23.04.2024
/// Description: Password manager class.
///     - Authentificate user on creation
///     - Vigenere crypting.
///     - Save crypted passwords in a text file
///     - Crypt/Decrypt using password entered by user at start
///     - Allows to manage passwords (Remove/Change entries)

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("TestVigenere")]

namespace GestionaireMDP
{
    internal class PWManager
    {

        /// <summary>
        /// Regex uses for master password. 
        /// Minimum eight characters, at least one letter and one number:
        /// Source : https://stackoverflow.com/questions/19605150/regex-for-password-must-contain-at-least-eight-characters-at-least-one-number-a
        /// </summary>
        const string MPW_REGEX = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";

        /// <summary>
        /// Maximum login attemps
        /// </summary>
        const int MAX_ATTEMPS = 3;

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
        const string PW_PATH = @"..\..\passwords.txt";

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
        private string _masterPW;

        /// <summary>
        /// Path of the config filr
        /// </summary>
        const string CONFIG_PATH = @"..\..\config.txt";

        /// <summary>
        /// Master Password
        /// </summary>
        private string _cryptedMPW;

        /// <summary>
        /// User readkey input
        /// </summary>
        private ConsoleKey _userInput;

        /// <summary>
        /// PWManager constructor with given master password
        /// </summary>
        /// <param name="masterPassword">Password used for en/decrypting</param>
        public PWManager(string masterPassword) {

            _printableChars = GetPrintableChars();

            _masterPW = masterPassword;

            // If file doesn't exist, exit
            if (!FindFile(PW_PATH))
            {
                Console.Clear();
                Console.WriteLine("The file path is incorect");
                Console.ReadLine();
                Environment.Exit(0);
            }
            else {
                ImportEntries();
            }
        }

        /// <summary>
        /// PWManager construtor that promps for master password in console
        /// </summary>
        public PWManager()
        {
            _printableChars = GetPrintableChars();

            // If file doesn't exist, exit 
            if (!FindFile(CONFIG_PATH))
            {
                Console.Clear();
                Console.WriteLine("The file path is incorect :" + CONFIG_PATH);
                Console.ReadLine();
                Environment.Exit(0);
            }
            else
            {
                _cryptedMPW = ImportMPW();

                // if no password was found
                if (_cryptedMPW == "") {
                    _masterPW = PromptMasterPW();
                }

                // if a password was found get to login
                else
                {
             
                    bool logged = false;
                    int attemps = 0;

                    do {
                        Console.Clear();

                        // If user failt to log on afte MAX_ATTEMPS attemps, display message and exit
                        if (attemps == MAX_ATTEMPS)
                        {
                            Console.WriteLine("Failed to login after " + attemps + " attemps" + "\nShutting down...");
                            Console.ReadLine ();
                            Environment.Exit(0);    
                        }

                        // Get a password from user and Vigenere crypted password with revered password as key
                        string loginPW = PromptLogin();

                        // If the result equals the password given by user login
                        if (Vigenere(toVig: _cryptedMPW, reversed: true, key: loginPW) == loginPW)
                        { 
                            logged = true;
                            _masterPW = loginPW;
                        }

                        attemps++;

                    } while (!logged);

                }
            }

            // If file doesn't exist, exit 
            if (!FindFile(PW_PATH))
            {
                Console.Clear();
                Console.WriteLine("The file path is incorect :" + PW_PATH);
                Console.ReadLine();
                Environment.Exit(0);
            }
            else
            {
                ImportEntries();
            }
        }

        /// <summary>
        /// Get a master password from user
        /// </summary>
        /// <returns>New master password</returns>
        private string PromptMasterPW()
        {
            string masterPW;

            // Loop until password is returned
            do {

                // Loop as long as REGEX doesn't match
                do
                {
                    Console.Clear();
                    Console.WriteLine("Veuillez entrer votre nouveau mot de passe maître: ");
                    masterPW = GetHiddenConsoleInput();

                    // Display disclaimer if regex doesn't match
                    if (!Regex.IsMatch(masterPW, MPW_REGEX)){
                        Console.WriteLine("Votre mot de passe doit contenir au moins 8 charactères, au moin une lettre et un chiffre (Appuyez sur Enter)");
                        do { _userInput = Console.ReadKey(intercept: true).Key; } while (_userInput != ConsoleKey.Enter);
                    }
                } while (!Regex.IsMatch(masterPW, MPW_REGEX));

                // Confirm password
                Console.WriteLine("\n[Confirmer] Veuillez entrez à nouveau votre nouveau mot de passe maître: ");
                if (GetHiddenConsoleInput() == masterPW)
                {
                    // Update MPW in file and return new password
                    UpdateMPW(masterPW);
                    return masterPW;
                }
                else {
                    Console.WriteLine("\nLes mots de passes ne sont pas identiques, veuillez réessayer (Appuyez sur Enter)");
                    do { _userInput = Console.ReadKey(intercept: true).Key; } while (_userInput != ConsoleKey.Enter);
                }
            } while (true);
            
            
        }

        /// <summary>
        /// Update master password in config file
        /// </summary>
        /// <param name="newMPW"></param>
        private void UpdateMPW(string newMPW)
        {
            File.WriteAllText(CONFIG_PATH, Vigenere(toVig: newMPW, key: newMPW));
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
        private bool FindFile(string path) {
            // If file doesn't exist ...
            while (!File.Exists(path))
            {
                // ... try to create it
                try
                {
                    // https://stackoverflow.com/questions/5156254/closing-a-file-after-file-create
                    // https://stackoverflow.com/questions/66537978/c-sharp-system-io-ioexception-file-cannot-be-accessed-because-it-is-accessed-by
                    // Create and CLOSE file to avoid used by other process error when reading content
                    FileStream file = File.Create(path);
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
        private void ImportEntries()
        {
            string _content = "";
            List<string> lines = new List<string>();

            // Get txt file content as a string and then split it into lines 
            _content = File.ReadAllText(PW_PATH);
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
        public string Vigenere(string toVig, bool reversed = false, string key = "")
        {
            // if no key was given use master password
            if (key == "") {
                key = _masterPW;
            }

            // If we want to decrypt, reverse key
            if (reversed) {
                key = ReverseKey(key);
            }

            string crypted = "";


            // Foreach char add to their index an index based on the key (Vigenere)
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

        /// <summary>
        /// Export content of the text file with entries
        /// </summary>
        public void ExportPW()
        {
            string updatedContent = "";

            // Foreach entry, add site, username and password separated by SEPARATOR
            foreach (string[] entry in entries)
            {
                foreach (string item in entry)
                {
                    updatedContent += item;
                    updatedContent += SEPARATOR;
                }

                // New line for each entry
                updatedContent += "\n";
            }
            File.WriteAllText(PW_PATH, updatedContent);
        }

        /// <summary>
        /// Returns list of sites with an entry
        /// </summary>
        /// <returns>List of sites with an entry</returns>
        public List<string> GetSiteList() {
            List<string> sites = new List<string>();
            for (int i = 0; i < entries.Count; i++)
            {
                sites.Add(entries[i][SITE_INDEX]);
            }
            return sites;
        }

        /// <summary>
        /// Returns index of given site name
        /// </summary>
        /// <param name="site">Site name</param>
        /// <returns>Index of site / INDEX_NOT_FOUND</returns>
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

        /// <summary>
        /// Add a password to the entries and crypt username and password
        /// </summary>
        /// <param name="site">Site</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public void AddPW(string site, string username, string password){
            entries.Add(new string[] { site, Vigenere(toVig: username), Vigenere(toVig: password) }) ;
        }

        /// <summary>
        /// Remove an entry
        /// </summary>
        /// <param name="index">Index site to remove</param>
        public void RemovePW (int index) {  
            entries.RemoveAt(index) ; 
        }

        /// <summary>
        /// Decrypt and return a string with a full entry
        /// </summary>
        /// <param name="index">Index site to display</param>
        /// <returns>String to display</returns>
        public string DisplayEntry(int index)
        {
            return "Site: " + entries[index][SITE_INDEX] + "\n" +
                "Username : " + Vigenere(toVig: entries[index][USERNAME_INDEX], reversed: true) + "\n" +
                "Mot de passe: " + Vigenere(toVig: entries[index][PASSWORD_INDEX], reversed: true) + "\n";

        }

        /// <summary>
        /// Change a password in an entry
        /// </summary>
        /// <param name="index">Entry index</param>
        /// <param name="newValue">New password</param>
        public void ChangePassword (int index, string newValue){
            entries[index][PASSWORD_INDEX] = Vigenere(toVig: newValue);
        }

        /// <summary>
        /// Change a username in an entry
        /// </summary>
        /// <param name="index">Entry index</param>
        /// <param name="newValue">New username</param>
        public void ChangeUsername(int index, string newValue) {
            entries[index][USERNAME_INDEX] = Vigenere(toVig: newValue);
        }

        /// <summary>
        /// Change a site in an entry
        /// </summary>
        /// <param name="index">Entry index</param>
        /// <param name="newValue">New site</param>
        public void ChangeSite(int index, string newValue) {
            entries[index][SITE_INDEX] = newValue;
        }

        /// <summary>
        /// Get a string hidden from user
        /// https://stackoverflow.com/questions/23433980/c-sharp-console-hide-the-input-from-console-window-while-typing
        /// </summary>
        /// <returns>User input string</returns>
        private static string GetHiddenConsoleInput()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }

        /// <summary>
        /// Get the content of the config file
        /// </summary>
        /// <returns>Crypted master password</returns>
        private string ImportMPW()
        {

            // Get txt file content 
            string _content = "";
            _content = File.ReadAllText(CONFIG_PATH);
            return _content;
        }

        /// <summary>
        /// Get a login password from user
        /// </summary>
        /// <returns></returns>
        private string PromptLogin()
        {
            Console.WriteLine("Veuillez entrer votre mot de passe: ");
            string masterPW = GetHiddenConsoleInput();
            return masterPW;
        }

        /// <summary>
        /// Change master password, update password with new key
        /// </summary>
        public void ChangeMPW() {

            // Store old MPW
            string oldPW = _masterPW;

            // Get new MPW, update in config
            _masterPW = PromptMasterPW();
            UpdateMPW(newMPW: _masterPW);

            foreach (var entry in entries)
            {
                // Decrypt with old key
                entry[USERNAME_INDEX] = Vigenere(toVig: entry[USERNAME_INDEX], reversed: true, key: oldPW);
                entry[PASSWORD_INDEX] = Vigenere(toVig: entry[PASSWORD_INDEX], reversed: true, key: oldPW );

                // Encrypt with new key
                entry[USERNAME_INDEX] = Vigenere(toVig: entry[USERNAME_INDEX]);
                entry[PASSWORD_INDEX] = Vigenere(toVig: entry[PASSWORD_INDEX]);
            }

            // Update entries in file
            ExportPW();
        
        }
    }
}
