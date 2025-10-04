using System.Text;
using System.Security.Cryptography;

namespace FAST.Strings
{
    public static class stringCipher
    {
        // This constant is used to determine the key size of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
#if NETCOREAPP
        private const int keysize = 128;
#else
        private const int keysize = 256;
#endif


        // This constant determines the number of iterations for the password bytes generation function.
        private const int derivationIterations = 1000;

        // (v) used by internal encryption functions
        private const string internalPassPhase = "arnakiAsproKaiPaxu";

        /// <summary>
        ///     Encrypt a string using the supplied key. Encoding is done using RSA encryption.
        /// </summary>
        /// <param name="plainText">String that must be encrypted.</param>
        /// <param name="passPhrase">Encryption key</param>
        /// <returns>A string representing a byte array separated by a minus sign.</returns>
        public static string encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is prepended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
#if NETCOREAPP
            var saltStringBytes = generate128BitsOfRandomEntropy();
            var ivStringBytes = generate128BitsOfRandomEntropy();
#else
            var saltStringBytes = generate256BitsOfRandomEntropy();
            var ivStringBytes = generate256BitsOfRandomEntropy();
#endif
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, derivationIterations))
            {
                var keyBytes = password.GetBytes(keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = keysize; 
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        ///     Decrypt a string using the supplied key. Decoding is done using RSA encryption.
        /// </summary>
        /// <param name="cipherText">String that must be decrypted.</param>
        /// <param name="passPhrase">Decryption key.</param>
        /// <returns>The decrypted string or null if decryption failed.</returns>
        public static string decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the salt bytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(keysize / 8).Take(keysize / 8).ToArray();

            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, derivationIterations))
            {
                var keyBytes = password.GetBytes(keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = keysize; // 256 for .NET and 128 for .NET6
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = 0;
                                int bytesRead = 0;
                                int offset = 0;
                                int arrayMaxLength = plainTextBytes.Length;
                                int arrayLength = arrayMaxLength;
                                do
                                {
                                    bytesRead = cryptoStream.Read(plainTextBytes, offset, arrayLength);
                                    decryptedByteCount += bytesRead;
                                    offset=offset += (bytesRead );
                                    arrayLength = (arrayLength - bytesRead); // reduce the length to avoid exception
                                    if ( (offset + arrayLength) > arrayMaxLength ) bytesRead=0; // force break

                                } while (bytesRead > 0 );
                                
                                
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }


        public static string onewayEncrypt(string clearText)
        {
            return "OWENC" + encrypt(createMD5(clearText), internalPassPhase);
        }
        public static bool onewayEncryptIsSame(string clearText, string onewayEncrypted)
        {
            return onewayEncrypt(clearText) == onewayEncrypted;
        }

        public static string createMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static byte[] generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private static byte[] generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; // 16 Bytes will give us 128 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }




}

