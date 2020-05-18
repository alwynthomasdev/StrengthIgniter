using CodeFluff.Extensions.String;
using System;
using System.Security.Cryptography;

namespace StrengthIgniter.Core.Utils
{
    public interface IHashUtility
    {
        string Generate(string plainText);
        bool Validate(string plainText, string hashText);
        string GenerateFakeHash();
    }

    public class HashUtility : IHashUtility
    {
        const int SaltByteSize = 24;
        const int HashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
        const int IterationIndex = 0;
        const int SaltIndex = 1;
        const int Pbkdf2Index = 2;

        public const int DefaultPbkdf2Iterations = 100000;
        public readonly int Pbkdf2Iterations;

        //for using when validating nothing
        public string GenerateFakeHash()
        {
            return $"{Pbkdf2Iterations}:HNl7lAC/cBOhvfBBFerYde4IRfQvtavq:3xXnU3ba8cqipCYW62QbxQOlhX8=".Base64Encode();
        }

        public HashUtility(int iterations = 0)
        {
            if (iterations > 0)
                Pbkdf2Iterations = iterations;
            else Pbkdf2Iterations = DefaultPbkdf2Iterations;
        }

        public string Generate(string plainString)
        {
            var cryptoProvider = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltByteSize];
            cryptoProvider.GetBytes(salt);

            var hash = GetPbkdf2Bytes(plainString, salt, Pbkdf2Iterations, HashByteSize);

            string strHash = Pbkdf2Iterations + ":" +
                   Convert.ToBase64String(salt) + ":" +
                   Convert.ToBase64String(hash);

            string encHash = strHash.Base64Encode();

            return encHash;
        }

        public bool Validate(string plainString, string hashString)
        {
            hashString = hashString.Base64Decode();
            char[] delimiter = { ':' };
            var split = hashString.Split(delimiter);
            var iterations = int.Parse(split[IterationIndex]);
            var salt = Convert.FromBase64String(split[SaltIndex]);
            var hash = Convert.FromBase64String(split[Pbkdf2Index]);

            var testHash = GetPbkdf2Bytes(plainString ?? "", salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}
