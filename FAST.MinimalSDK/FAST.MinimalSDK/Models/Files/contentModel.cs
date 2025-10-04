namespace FAST.Services.Models
{

    public enum contentModelTypes:sbyte
    { // attention: don't change the number values. Only add to the list

        /// <summary>
        /// Type0, Plain text, no encoding, no encryption
        /// </summary>
        plain =0,

        /// <summary>
        /// Type1, encoded to base64
        /// </summary>
        base64=1,

        /// <summary>
        /// Type2, Proprietary Application is responsible for encoding-decoding 
        /// </summary>
        proprietary = 2,

        /// <summary>
        /// Type3, Encrypted and encoded to base64
        /// </summary>
        base64Encrypted=3
    }


    public class contentModel
    {
        public contentModelTypes type { get; set; }
        public string data { get;set; }

    }
}
