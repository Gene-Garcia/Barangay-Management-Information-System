using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barangay_Management_Information_System.Classess
{
    public class CeasarCipher
    {

        private static char cipher(char ch, int key)
        {
            if (!char.IsLetter(ch))
            {

                return ch;
            }

            char d = char.IsUpper(ch) ? 'A' : 'a';
            return (char)((((ch + key) - d) % 26) + d);


        }


        public static string Encipher(string text, int key)
        {
            string hashed = string.Empty;

            foreach (char letter in text)
                hashed += cipher(letter, key);

            return hashed;
        }


    }
}