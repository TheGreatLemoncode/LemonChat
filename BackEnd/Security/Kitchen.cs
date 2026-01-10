using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Konscious.Security.Cryptography;

namespace BackEnd.Security
{
    /// <summary>
    /// Internal static class whose role is to hash password, create salts and compare hash with clear string
    /// </summary>
    internal static class Kitchen
    {
        private static RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public static bool CompareClearHashSalt(byte[] Hash, byte[] Salt, string pPassword)
        {
            return Hash.SequenceEqual(HashPassword(pPassword, Salt));
        }

        public static byte[] HashPassword(string pPassword,  byte[] salt)
        {
            Argon2id argon = new Argon2id(Encoding.UTF8.GetBytes(pPassword))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                Iterations = 4,
                MemorySize = 1024 * 1024
            };
            return argon.GetBytes(16);
        }

        public static byte[] CreateSalt()
        {
            byte[] buffer = new byte[16];
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}
