using FAST.Strings;

namespace FAST.Printing
{
    public class escapeSequenceMapper
    {
        public enum asciiSpecialCharacter:byte
        {
                    null_character=0,
                    start_of_heading=1,
                    start_of_text=2,
                    end_of_text=3,
                    end_of_transmission=4,
                    enquiry=5,
                    acknowledge=6,
                    bell=7,
                    backspace=8,
                    horizontal_tab=9,
                    new_line=10,
                    vertical_tab=11,
                    new_page=12,
                    carriage_return=13,
                    shift_out=14,
                    shift_in=15,
                    data_link_escape=16,
                    device_control_1=17,
                    device_control_2=18,
                    device_control_3=19,
                    device_control_4=20,
                    negative_acknowledge=21,
                    synchronous_idle=22,
                    end_of_trans_block=23,
                    cancel=24,
                    end_of_medium=25,
                    substitute=26,
                    escape=27,
                    file_separator=28,
                    group_separator=29,
                    record_separator=30,
                    unit_separator=31,
                    space=32,
                    delete=127,
                    no_break_space=255
        }

        public static char toChar(asciiSpecialCharacter asciiCharacter)
        {
            return Convert.ToChar((int)asciiCharacter);
        }

        public static string map(string input, string beginDelimeter, string endDelimeter, bool removeEscapeCharacters )
        {
            string output = "";
            escapeSequenceMapper mapper = new escapeSequenceMapper();
            var tokens = tokenizer.queueTokens(input, beginDelimeter, endDelimeter, '\\', removeEscapeCharacters);
            foreach (var token in tokens)
            {

                switch (token.type)
                {
                    case tokenizer.tokenType.beginDelimeter:
                    case tokenizer.tokenType.endDelimeter:
                        continue; // by pass delimiters
                    case tokenizer.tokenType.text:
                        output += token.token;
                        break;
                    case tokenizer.tokenType.textBetweenDelimeters:
                        output += mapper.mapASCII(token.token);
                        break;
                    default:
                        break;
                }
            }
            return output;
        }

        public escapeSequenceMapper()
        {
        }

        public string mapASCII(string inputExpression )
        {
            string output="";
            bool translated = false;
            foreach (var token in inputExpression.Split(' '))
            {
                if (token.Length < 1) { continue; } // remove this token!

                #region (+) convert hex number in style of '0x1b' or 'x1b' 
                translated = false;
                if (!translated)
                {
                    if (token[0] == 'x')
                    {
                        output += System.Text.Encoding.Default.GetString(FAST.Strings.converters.hexStringToByteArray(token.Substring(1)));
                        translated = true;
                    }
                }

                if (!translated)
                {
                    if (token.Length > 2)
                    {
                        if (token[0] == '0' && token[1] == 'x')
                        {
                            output += System.Text.Encoding.Default.GetString(FAST.Strings.converters.hexStringToByteArray(token.Substring(2)));
                            translated = true;
                        }
                    }
                }

                if (!translated)
                {
                    if (token[0] == 'd')
                    {
                        byte result=0;
                            if (byte.TryParse(token.Substring(1), out result))
                            {
                                if (result >= 0 && result <= 255)
                                {
                                    output += (char)result;
                                    translated = true;
                                }
                            }
                    }
                }

                if (!translated)
                {
                    if (token[0] == 'b')
                    {
                        int result =  Convert.ToInt32(token.Substring(1), 2);
                        if (result >= 0 && result <= 255)
                        {
                            byte byteResult = (byte)result;
                            output += (char)result;
                            translated = true;
                        }
                    }
                }
                #endregion (+) convert hex number in style of '0x1b' or 'x1b' or decimal 'd100' or binaries 'b00011101'

                #region (+) Symbols like ESC ^[ etc
                if (!translated)
                {
                    translated = true;
                    switch (token)
                    {
                        case "NUL":
                        case "^@":
                            output += "\x00";
                            break;

                        case "SOH":
                        case "^A":
                            output += "\x01";
                            break;

                        case "STX":
                        case "^B":
                            output += "\x02";
                            break;

                        case "ETX":
                        case "^C":
                            output += "\x03";
                            break;

                        case "EOT":
                        case "^D":
                            output += "\x04";
                            break;

                        case "ENQ":
                        case "^E":
                            output += "\x05";
                            break;

                        case "ACK":
                        case "^F":
                            output += "\x06";
                            break;

                        case "BEL":
                        case "^G":
                            output += "\x07";
                            break;

                        case "BS":
                        case "^H":
                            output += "\x08";
                            break;

                        case "HT":
                        case "^I":
                            output += "\x09";
                            break;

                        case "LF":
                        case "^J":
                            output += "\x0A";
                            break;

                        case "VT":
                        case "^K":
                            output += "\x0B";
                            break;

                        case "FF":
                        case "^L":
                            output += "\x0C";
                            break;

                        case "CR":
                        case "^M":
                            output += "\x0D";
                            break;

                        case "SO":
                        case "^N":
                            output += "\x0E";
                            break;

                        case "SI":
                        case "^O":
                            output += "\x0F";
                            break;

                        case "DLE":
                        case "^P":
                            output += "\x10";
                            break;

                        case "DC1":
                        case "^Q":
                            output += "\x11";
                            break;

                        case "DC2":
                        case "^R":
                            output += "\x12";
                            break;

                        case "DC3":
                        case "^S":
                            output += "\x13";
                            break;

                        case "DC4":
                        case "^T":
                            output += "\x14";
                            break;

                        case "NAK":
                        case "^U":
                            output += "\x15";
                            break;

                        case "SYN":
                        case "^V":
                            output += "\x16";
                            break;

                        case "ETB":
                        case "^W":
                            output += "\x17";
                            break;

                        case "CAN":
                        case "^X":
                            output += "\x18";
                            break;

                        case "EM":
                        case "^Y":
                            output += "\x19";
                            break;

                        case "SUB":
                        case "^Z":
                            output += "\x1A";
                            break;

                        case "ESC":
                        case "^[":
                            output += "\x1B";
                            break;

                        case "FS":
                        case "^\\":
                            output += "\x1C";
                            break;

                        case "GS":
                        case "^]":
                            output += "\x1D";
                            break;

                        case "RS":
                        case "^^":
                            output += "\x1E";
                            break;

                        case "US":
                        case "^-":
                            output += "\x1F";
                            break;

                        default:
                            translated = false;
                            break;
                    }
                }
                #endregion (+) Symbols like ESC ^[ etc

                #region (+) Special other words like SPACE, TAB etc
                if (!translated)
                {
                    translated = true;
                    switch (token)
                    {
                        case "SPACE":
                            output += " ";
                            break;
                        case "TAB":
                            output += "\t";
                            break;

                        default:
                            translated = false;
                            break;
                    }
                }
                #endregion (+) Special other words like SPACE, TAB etc

                // (v) finally
                if (!translated)
                {
                    output += token;
                }
            }

            return output;
        }

    }
}
