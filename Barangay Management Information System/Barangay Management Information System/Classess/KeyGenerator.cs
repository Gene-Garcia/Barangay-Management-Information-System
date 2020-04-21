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

        private static Random rnd = new Random();

        public static string GenerateId(string textString = "abcdefghijklmnopqrz")
        {

            string[,] id = { { "", "", "", "" }, { "", "", "", "" }, { "", "", "", "" }, { "", "", "", "" } };

            fillDate(ref id);
            fillTime(ref id);
            fill(ref id, HASH1_LOC, textString);
            fill(ref id, HASH2_LOC, textString);

            return createId(id);
        }

        private static string createId(string[,] id)
        {
            string finalId = "";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    finalId += id[i, j];
                }

                if (i >= 3)
                {

                } else
                {
                    finalId += "-";
                }
            }

            return finalId;
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