using System;
using System.Security.Cryptography;
using System.Text;

namespace CompositeC1Contrib.Security
{
    public class PasswordHash
    {
        /// <summary>
        /// Hashes a password
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>The hashed password as a 128 character hex string</returns>
        public static string HashPassword(string password)
        {
            string salt = GetRandomSalt();
            string hash = Sha256Hex(salt + password);

            return salt + hash;
        }

        /// <summary>
        /// Validates a password
        /// </summary>
        /// <param name="password">The password to test</param>
        /// <param name="correctHash">The hash of the correct password</param>
        /// <returns>True if password is the correct password, false otherwise</returns>
        public static bool ValidatePassword(string password, string correctHash)
        {
            if (correctHash.Length < 128)
                throw new ArgumentException("Hash must be 128 hex characters!", "correctHash");

            string salt = correctHash.Substring(0, 64);
            string validHash = correctHash.Substring(64, 64);
            string passHash = Sha256Hex(salt + password);

            return String.Compare(validHash, passHash) == 0;
        }

        //returns the SHA256 hash of a string, formatted in hex
        private static string Sha256Hex(string toHash)
        {
            var hash = new SHA256Managed();
            var utf8 = UTF8Encoding.UTF8.GetBytes(toHash);

            return BytesToHex(hash.ComputeHash(utf8));
        }

        //Returns a random 64 character hex string (256 bits)
        private static string GetRandomSalt()
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new byte[32]; //256 bits
            random.GetBytes(salt);

            return BytesToHex(salt);
        }

        //Converts a byte array to a hex string
        private static string BytesToHex(byte[] toConvert)
        {
            var s = new StringBuilder(toConvert.Length * 2);
            foreach (byte b in toConvert)
            {
                s.Append(b.ToString("x2"));
            }

            return s.ToString();
        }
    }
}
