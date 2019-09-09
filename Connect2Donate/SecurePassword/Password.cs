using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Connect2Donate.SecurePassword
{
    public class Password
    {
        //Password Hashing Method
        public static List<string> Ecrypt(string plaintextpassword)
        {
            byte[] salt = new byte[16];

            /*Generating Salt*/
            try
            {
                using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
                {
                    csprng.GetBytes(salt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Random number generator not available.",
                    ex
                );
            }

            /*Generating Hash*/
            var pbkdf2 = new Rfc2898DeriveBytes(plaintextpassword, salt, 10000);

            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            List<string> saltAndHash = new List<string>();
            saltAndHash.Add(Convert.ToBase64String(salt));
            saltAndHash.Add(Convert.ToBase64String(hashBytes));
            return saltAndHash;

        }

        //Compare Entered Password to Databas
        //Return type Bool

        public static bool ComparePassword(string enteredPassword, byte[] storedHashBytes, byte[] storedSaltBytes, bool matched=true)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, storedSaltBytes, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (storedHashBytes[i + 16] != hash[i])
                    matched = false;
            }
            return matched;
        }
    }
}