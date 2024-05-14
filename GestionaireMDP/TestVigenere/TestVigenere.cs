/// ETML
/// Author: Valentin Pignat
/// Date (creation): 30.04.2024
/// Description : Vigenere test class
/// Test are organised from large test to specific tests

using Microsoft.VisualStudio.TestTools.UnitTesting;
using GestionaireMDP;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestVigenere
{
    [TestClass]
    public class TestVigenere
    {

        /// <summary>
        /// Test that the result of crypting and then decrypting return us the starting value.
        /// Test every char in printable characters as key
        /// </summary>
        [TestMethod]
        public void TestEveryMasterPassword()
        {
            // Arrange

            // Create a password manager with any password to obtain a list of printable characters
            PWManager _pwManager = new PWManager(masterPassword: "");
            const string TO_VIGENERE = "aaa";

            // Act
            List<char> printableChar = _pwManager.GetPrintableChars();
        

            foreach (char c in printableChar)
            {
                _pwManager = new PWManager(masterPassword: c.ToString());
                string crypted = _pwManager.Vigenere(toVig: TO_VIGENERE);
                crypted = _pwManager.Vigenere(toVig: crypted, reversed: true);

                // Assert
                Assert.AreEqual(TO_VIGENERE, crypted);

            }

        }

        /// <summary>
        /// Test that a key reversed two times return the starting value
        /// </summary>
        [TestMethod]
        public void TestReverse() {

            // Arrange

            // Create a password manager with any password to obtain a list of printable characters
            PWManager _pwManager = new PWManager(masterPassword: "asd");

            // Act
            List<char> printableChar = _pwManager.GetPrintableChars();


            // Assert
            foreach (char c in printableChar)
            {
                 
                string reversed = _pwManager.ReverseKey(c.ToString());
                reversed = _pwManager.ReverseKey(reversed);

                Assert.AreEqual(c.ToString(), reversed);
                
            }
        }

        /// <summary>
        /// Test encryption
        /// </summary>
        [TestMethod]
        public void TestEncryption()
        {
            // Arrange 
            PWManager _pwManager = new PWManager(masterPassword: "!!!");
            const string TO_VIGENERE = "aaa";
            const string EXPECTED = "bbb";

            // Act
            string crypted = _pwManager.Vigenere(toVig: TO_VIGENERE);

            // Assert
            Assert.AreEqual(EXPECTED, crypted);
        }

        
        
    }
}
