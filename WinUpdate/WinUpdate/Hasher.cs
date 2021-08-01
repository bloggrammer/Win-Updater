using System;
using System.IO;
using System.Security.Cryptography;

namespace WinUpdate
{
    internal static class Hasher
    {
        internal static string HashFile(string filePath, HashType algo)
        {
            switch (algo)
            {
                case HashType.MD5:
                    return MakeHash(MD5.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));
                case HashType.SHA1:
                    return MakeHash(SHA1.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));
                case HashType.SHA512:
                    return MakeHash(SHA512.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));
                default:
                    return string.Empty;
            }
        }

        private static string MakeHash(byte[] hash)=> BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    internal enum HashType { MD5, SHA1, SHA512}
}
