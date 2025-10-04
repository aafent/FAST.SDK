using FAST.Services.Models;
using System.Text;

namespace FAST.Files
{
    public static class contentHelper
    {
        private static readonly string innerEncryptionPhase="!@34Arni"; // keep it to 8 bytes long, do not change it.

        public static contentModel convertTo(contentModel input, contentModelTypes typeToConvert )
        {
            if ( input == null ) return null;
            if ( input.type == typeToConvert) return input;

            if ( input.type == contentModelTypes.proprietary || typeToConvert == contentModelTypes.proprietary )
            {
                throw new Exception($"Cannot convert contentModel from/to proprietary content type");
            }

            contentModel result = new contentModel();
            result.type = typeToConvert;
            result.data = null;

            // STEP - 1
            // (v) load plain data into result.data
            switch (input.type)
            {
                case contentModelTypes.plain:
                    result.data = input.data;
                    break;
                case contentModelTypes.base64:
                    result.data = Strings.converters.base64Decode(input.data);
                    break;

                case contentModelTypes.base64Encrypted:
                    result.data = Strings.converters.base64Decode(input.data);  // to encoded text
                    result.data = Strings.stringCipher.decrypt(result.data, innerEncryptionPhase); // to clear text
                    break;

                default:
                    throw new NotImplementedException();
            }


            // STEP - 2
            // (v) convert result.data form plain data to the requested type
            switch (typeToConvert)
            {
                case contentModelTypes.plain:
                    // it is already, no further conversion needed.
                    break;
                case contentModelTypes.base64:
                    result.data = Strings.converters.base64Encode(result.data);
                    break;

                case contentModelTypes.base64Encrypted:
                    result.data = Strings.stringCipher.encrypt(result.data, innerEncryptionPhase); // to encoded text
                    result.data = Strings.converters.base64Encode(result.data); // to base64 encoded text
                    break;

                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        public static contentModel createFromPlainInput(string input, contentModelTypes toType)
        {
            if ( input == null ) return null;

            contentModel result = new contentModel();
            result.type = contentModelTypes.plain;
            result.data = input;
            result = convertTo(result, toType);

            return result;
        }
        public static contentModel createFromPlainInput(byte[] input, contentModelTypes toType, Encoding encoding = null)
        {
            if (input == null) return null;

            if (encoding == null) encoding = Encoding.UTF8;
            contentModel result =createFromAnyInput(encoding.GetString(input), toType);

            return result;
        }

        public static contentModel createFromAnyInput(string input, contentModelTypes inputType)
        {
            if(input == null) return null;

            contentModel result = new contentModel();
            result.type = inputType;
            result.data = input;

            return result;
        }

        public static string toString(contentModel input)
        {
            return convertTo(input, contentModelTypes.plain).data;
        }

    }
}
