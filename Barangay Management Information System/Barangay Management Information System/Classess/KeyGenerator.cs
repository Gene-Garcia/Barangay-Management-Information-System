using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barangay_Management_Information_System.Classess
{
    public class KeyGenerator
    {

       public KeyGenerator()
        {

        }

        private static int TIME_LOC = 0;
        private static int DATE_LOC = 1;
        private static int HASH1_LOC = 2;
        private static int HASH2_LOC = 3;
        private static int INTEGER_LOC = 4;

        private static Random rnd = new Random();

        public static string GenerateId(string textString = "abcdefghijklmnopqrz")
        {

            textString = textString.Replace(" ", "");

            string[,] id = { { "", "", "", "", "" }, { "", "", "", "", "" }, { "", "", "", "", "" }, { "", "", "", "", "" }, { "", "", "", "", "" } };

            fillDate(ref id);
            fillTime(ref id);
            fill(ref id, HASH1_LOC, textString);
            fillRandomString(ref id);
            fillRandomInteger(ref id);

            string[] newId = cipher(id);

            string finalID = createId(newId);
            
            return finalID.Replace(" ", new Random().Next().ToString()+"x");
        }

        private static string createId(string[] id)
        {
            string finalId = "";
            for (int i = 0; i < 5; i++)
            {                
                if (id[i] == " ")
                {
                    finalId += "0";
                }
                else
                {
                    finalId += id[i];
                }

                if (i >= 4)
                {

                } 
                else
                {
                    finalId += "-";
                }
            }

            return finalId;
        }

        private static string[] cipher(string[,] id)
        {
            int KEY = 4;

            string[] id1Dimension = { "", "" , "", "", "" };

            for(int i = 0; i < 5; i++)
            {

                string rowValue = "";

                for (int j = 0; j < 5; j++)
                {
                    rowValue += id[i, j];
                }

                id1Dimension[i] = CeasarCipher.Encipher(rowValue, KEY);
            }

            return id1Dimension;
            
        }

        private static void fillRandomInteger(ref string[,] id)
        {
            string strInteger = "0123456789";

            fill(ref id, INTEGER_LOC, strInteger);
        }

        private static void fillRandomString(ref string[,] id)
        {
            string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            fill(ref id, HASH2_LOC, str);
        }

        private static void fillTime(ref string[,] id)
        {
            string time = DateTime.Now.ToLongTimeString();
            time = time.Replace(":", "");

            fill(ref id, TIME_LOC, time);
        }

        private static void fillDate(ref string[,] id)
        {
            string date = DateTime.Now.ToShortDateString();
            date = date.Replace("/", "");

            fill(ref id, DATE_LOC, date);

        }

        private static void fill(ref string[,] id, int location, string key)
        {
           
            // 4 is the size of the array
            for (int i = 0; i < 4; i++)
            {
                int randNum = rnd.Next(key.Length);

                id[location, i] = key[randNum].ToString();
            }
        }
    }
}