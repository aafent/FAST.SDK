using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;


namespace FAST.Types
{

    // Summary:
    //     Represents a 32-bit signed integer.
    [Serializable]
        [ComVisible(true)]
        public struct countType : IComparable, IFormattable, IConvertible, IComparable<int>, IEquatable<int>
        {

            // Summary:
            //     Represents the largest possible value of an System.Int32. This field is constant.
            public const int MaxValue = 2147483647;
            //
            // Summary:
            //     Represents the smallest possible value of System.Int32. This field is constant.
            public const int MinValue = -2147483648;

            private Int32 underlyingValue;


            public static implicit operator countType(int value)
            {
                return new countType() { underlyingValue = value };
            }
            public static countType operator +(countType first, countType second)
            {
                return new countType() { underlyingValue = (first.underlyingValue + second.underlyingValue) };
            }
            public static countType operator -(countType first, countType second)
            {
                return new countType() { underlyingValue = (first.underlyingValue - second.underlyingValue) };
            }

            public static countType operator ++(countType first)
            {
                return new countType() { underlyingValue = (first.underlyingValue ++) };
            }
            public static countType operator --(countType first)
            {
                return new countType() { underlyingValue = (first.underlyingValue--) };
            }
            public static bool operator <(countType e1, countType e2)
            {
                return e1.CompareTo(e2) < 0;
            }
            public static bool operator >(countType e1, countType e2)
            {
                return e1.CompareTo(e2) > 0;
            }

            // Summary:
            //     Compares this instance to a specified 32-bit signed integer and returns an
            //     indication of their relative values.
            //
            // Parameters:
            //   value:
            //     An integer to compare.
            //
            // Returns:
            //     A signed number indicating the relative values of this instance and value.Return
            //     Value Description Less than zero This instance is less than value. Zero This
            //     instance is equal to value. Greater than zero This instance is greater than
            //     value.
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            public int CompareTo(int value)
            {
                return underlyingValue.CompareTo(value);
            }


            //
            // Summary:
            //     Compares this instance to a specified object and returns an indication of
            //     their relative values.
            //
            // Parameters:
            //   value:
            //     An object to compare, or null.
            //
            // Returns:
            //     A signed number indicating the relative values of this instance and value.Return
            //     Value Description Less than zero This instance is less than value. Zero This
            //     instance is equal to value. Greater than zero This instance is greater than
            //     value.-or- value is null.
            //
            // Exceptions:
            //   System.ArgumentException:
            //     value is not an System.Int32.
            public int CompareTo(object value)
            {
                return underlyingValue.CompareTo(value);
            }


            //
            // Summary:
            //     Returns a value indicating whether this instance is equal to a specified
            //     System.Int32 value.
            //
            // Parameters:
            //   obj:
            //     An System.Int32 value to compare to this instance.
            //
            // Returns:
            //     true if obj has the same value as this instance; otherwise, false.
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            public bool Equals(int obj)
            {
                return underlyingValue.Equals(obj);
            }


            //
            // Summary:
            //     Returns a value indicating whether this instance is equal to a specified
            //     object.
            //
            // Parameters:
            //   obj:
            //     An object to compare with this instance.
            //
            // Returns:
            //     true if obj is an instance of System.Int32 and equals the value of this instance;
            //     otherwise, false.
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            public override bool Equals(object obj)
            {
                return underlyingValue.Equals(obj);
            }


            //
            // Summary:
            //     Returns the hash code for this instance.
            //
            // Returns:
            //     A 32-bit signed integer hash code.
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            public override int GetHashCode()
            {
                return underlyingValue.GetHashCode();
            }


            //
            // Summary:
            //     Returns the System.TypeCode for value type System.Int32.
            //
            // Returns:
            //     The enumerated constant, System.TypeCode.Int32.
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            public TypeCode GetTypeCode()
            {
                return underlyingValue.GetTypeCode();
            }


            //
            // Summary:
            //     Converts the numeric value of this instance to its equivalent string representation.
            //
            // Returns:
            //     The string representation of the value of this instance, consisting of a
            //     negative sign if the value is negative, and a sequence of digits ranging
            //     from 0 to 9 with no leading zeroes.
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            [SecuritySafeCritical]
            public override string ToString()
            {
                return underlyingValue.ToString();
            }


            //
            // Summary:
            //     Converts the numeric value of this instance to its equivalent string representation
            //     using the specified culture-specific format information.
            //
            // Parameters:
            //   provider:
            //     An object that supplies culture-specific formatting information.
            //
            // Returns:
            //     The string representation of the value of this instance as specified by provider.
            [SecuritySafeCritical]
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            public string ToString(IFormatProvider provider)
            {
                return underlyingValue.ToString(provider);
            }


            //
            // Summary:
            //     Converts the numeric value of this instance to its equivalent string representation,
            //     using the specified format.
            //
            // Parameters:
            //   format:
            //     A standard or custom numeric format string.
            //
            // Returns:
            //     The string representation of the value of this instance as specified by format.
            //
            // Exceptions:
            //   System.FormatException:
            //     format is invalid or not supported.
            [SecuritySafeCritical]
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            public string ToString(string format)
            {
                return underlyingValue.ToString(format);
            }


            //
            // Summary:
            //     Converts the numeric value of this instance to its equivalent string representation
            //     using the specified format and culture-specific format information.
            //
            // Parameters:
            //   format:
            //     A standard or custom numeric format string.
            //
            //   provider:
            //     An object that supplies culture-specific formatting information.
            //
            // Returns:
            //     The string representation of the value of this instance as specified by format
            //     and provider.
            //
            // Exceptions:
            //   System.FormatException:
            //     format is invalid or not supported.
            [SecuritySafeCritical]
            public string ToString(string format, IFormatProvider provider)
            {
                return underlyingValue.ToString(format,provider);
            }



            public bool ToBoolean(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public byte ToByte(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public char ToChar(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public DateTime ToDateTime(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public decimal ToDecimal(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public double ToDouble(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public short ToInt16(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public int ToInt32(IFormatProvider provider)
            {
                return underlyingValue;
            }

            public long ToInt64(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public sbyte ToSByte(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public float ToSingle(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public object ToType(Type conversionType, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public ushort ToUInt16(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public uint ToUInt32(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public ulong ToUInt64(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }
        }

}
