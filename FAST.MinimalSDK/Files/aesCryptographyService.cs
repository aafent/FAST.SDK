using System.Security.Cryptography;

namespace FAST.Files
{

    public class aesCryptography
    {
        /// <summary>
        /// https://stackoverflow.com/questions/53653510/c-sharp-aes-encryption-byte-array
        /// </summary>
        /// 

        private byte[] defaultKey = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private byte[] defaultIV = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


        public byte[] encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return performCryptography(data, encryptor);
                }
            }
        }
        public byte[] encrypt(byte[] data)
        {
            return encrypt(data, defaultKey, defaultIV);
        }

        public byte[] decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return performCryptography(data, decryptor);
                }
            }
        }
        public byte[] decrypt(byte[] data)
        {
            return decrypt(data, defaultKey, defaultIV);
        }


        private byte[] performCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }

        //private void testCode()
        //{
        //    var input = new byte[16] { 0x1E, 0xA0, 0x35, 0x3A, 0x7D, 0x29, 0x47, 0xD8, 0xBB, 0xC6, 0xAD, 0x6F, 0xB5, 0x2F, 0xCA, 0x84 };

        //    var crypto = new aesCryptography();

        //    var encrypted = crypto.encrypt(input, defaultKey, defaultIV);
        //    var str = BitConverter.ToString(encrypted).Replace("-", "");
        //    Console.WriteLine(str);
        //}

    }



}
