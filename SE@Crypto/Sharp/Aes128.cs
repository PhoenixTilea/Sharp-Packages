// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SE.Crypto
{
    /// <summary>
    /// 128 bit Advanced Encryption Standard Algorithm
    /// </summary>
    public static class Aes128
    {
        private static readonly RijndaelManaged aesEncryption = new RijndaelManaged();
        static Aes128()
        {
            aesEncryption.KeySize = 128;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.None;
        }

        /// <summary>
        /// Encrypts a block of data using the provided key
        /// </summary>
        /// <param name="key">A byte array that should act as encryption key</param>
        /// <param name="data">A 16 byte aligned block of data to encrypt</param>
        /// <returns>The encrypted block of data</returns>
        public static byte[] Encrypt(byte[] key, byte[] data)
        {
            byte[] iv = new byte[16];
            byte[] bt = Sha256.Hash(key);
            Array.Copy(bt, 16, iv, 0, 16);

            aesEncryption.IV = iv;
            byte[] aesEncryptionKey = new byte[16];
            Array.Copy(bt, aesEncryptionKey, 16);
            aesEncryption.Key = aesEncryptionKey;

            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
            return crypto.TransformFinalBlock(data, 0, data.Length);
        }
        /// <summary>
        /// Decrypts a block of data using the provided key
        /// </summary>
        /// <param name="key">A byte array that should act as decryption key</param>
        /// <param name="data">A 16 byte aligned block of data to decrypt</param>
        /// <returns>The decrypted block of data</returns>
        public static byte[] Decrypt(byte[] key, byte[] data)
        {
            byte[] iv = new byte[16];
            byte[] bt = Sha256.Hash(key);
            Array.Copy(bt, 16, iv, 0, 16);

            aesEncryption.IV = iv;
            byte[] aesEncryptionKey = new byte[16];
            Array.Copy(bt, aesEncryptionKey, 16);
            aesEncryption.Key = aesEncryptionKey;

            ICryptoTransform crypto = aesEncryption.CreateDecryptor();
            return crypto.TransformFinalBlock(data, 0, data.Length);
        }
    }
}
