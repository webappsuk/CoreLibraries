using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Text
{
    /// <summary>
    /// The UnicodeChar struct is analogous to the built-in <see cref="char"/> struct, except it represents all unicode characters,
    /// instead of using a surrogate pair.
    /// </summary>
    /// <seealso cref="IComparable" />
    /// <seealso cref="IConvertible" />
    /// <seealso cref="IComparable{UnicodeChar}" />
    /// <seealso cref="IEquatable{UnicodeChar}" />
    public partial struct UnicodeChar : IComparable, IConvertible, IComparable<UnicodeChar>, IEquatable<UnicodeChar>
    {
        /// <summary>
        /// The code point.
        /// </summary>
        public readonly int CodePoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicodeChar"/> from an integer code point.
        /// </summary>
        /// <param name="codePoint">The code point.</param>
        public UnicodeChar(int codePoint)
        {
            CodePoint = codePoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicodeChar" /> from a <see cref="char" />.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <exception cref="ArgumentException">The character is part of a surrogate pair, so cannot be converted to a UnicodeChar on it's own.</exception>
        public UnicodeChar(char c)
        {
            if (char.IsSurrogate(c))
                throw new ArgumentException(
                    "The character is part of a surrogate pair, so cannot be converted to a UnicodeChar on it's own.",
                    nameof(c));
            CodePoint = c;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicodeChar" /> from two <see cref="char" >characters</see>.
        /// </summary>
        /// <param name="highSurrogate">The high surrogate.</param>
        /// <param name="lowSurrogate">The low surrogate.</param>
        public UnicodeChar(char highSurrogate, char lowSurrogate)
        {
            CodePoint = char.ConvertToUtf32(highSurrogate, lowSurrogate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicodeChar" /> from a character in a <see cref="string" />.
        /// </summary>
        /// <param name="s">The source string.</param>
        /// <param name="index">The index of the first UTF-16 character in the string.</param>
        public UnicodeChar([NotNull]string s, int index=0)
        {
            CodePoint = char.ConvertToUtf32(s, index);
        }
        
        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => CodePoint;

        /// <summary>Returns a value that indicates whether this instance is equal to a specified object.</summary>
        /// <param name="obj">An object to compare with this instance or null. </param>
        /// <returns>true if <paramref name="obj" /> is an instance of <see cref="T:System.Char" /> and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is UnicodeChar)
                return CodePoint == ((UnicodeChar)obj).CodePoint;
            return CodePoint == obj as char?;
        }

        /// <summary>Returns a value that indicates whether this instance is equal to the specified <see cref="T:System.Char" /> object.</summary>
        /// <param name="obj">An object to compare to this instance. </param>
        /// <returns>true if the <paramref name="obj" /> parameter equals the value of this instance; otherwise, false.</returns>
        public bool Equals(UnicodeChar obj) => CodePoint == obj.CodePoint;

        /// <summary>
        /// Compares this instance to a specified object and indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="value">An object to compare this instance to, or null.</param>
        /// <returns>A signed number indicating the position of this instance in the sort order in relation to the <paramref name="value" /> parameter.Return Value Description Less than zero This instance precedes <paramref name="value" />. Zero This instance has the same position in the sort order as <paramref name="value" />. Greater than zero This instance follows <paramref name="value" />.-or- <paramref name="value" /> is null.</returns>
        /// <exception cref="System.ArgumentException">The argument must be a character or unicode character. - value</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="value" /> is not a <see cref="T:System.Char" /> object.</exception>
        public int CompareTo(object value)
        {
            if (value == null)
                return 1;
            if (value is UnicodeChar)
                return CodePoint - ((UnicodeChar)value).CodePoint;
            if (value is char)
                return CodePoint - (char)value;
            throw new ArgumentException("The argument must be a character or unicode character.", nameof(value));
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="T:System.Char" /> object and indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified <see cref="T:System.Char" /> object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A signed number indicating the position of this instance in the sort order in relation to the <paramref name="obj" /> parameter.Return Value Description Less than zero This instance precedes <paramref name="obj" />. Zero This instance has the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" />.</returns>
        public int CompareTo(UnicodeChar obj)
        {
            return CodePoint - obj.CodePoint;
        }

        /// <summary>Converts the value of this instance to its equivalent string representation.</summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString()
        {
            return ToString(this);
        }

        /// <summary>Converts the value of this instance to its equivalent string representation using the specified culture-specific format information.</summary>
        /// <param name="provider">(Reserved) An object that supplies culture-specific formatting information. </param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="provider" />.</returns>
        public string ToString(IFormatProvider provider)
        {
            return ToString(this);
        }

        /// <summary>Converts the specified Unicode character to its equivalent string representation.</summary>
        /// <param name="c">The Unicode character to convert. </param>
        /// <returns>The string representation of the value of <paramref name="c" />.</returns>
        [NotNull]
        public static string ToString(UnicodeChar c) => char.ConvertFromUtf32(c.CodePoint);

        /// <summary>
        /// Performs an implicit conversion from <see cref="UnicodeChar"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(UnicodeChar c) => c.CodePoint;

        /// <summary>
        /// Performs an implicit conversion from <see cref="UnicodeChar"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>The result of the conversion.</returns>
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        public static implicit operator string(UnicodeChar c) => c.ToString();

        /// <summary>
        /// Performs an explicit conversion from <see cref="UnicodeChar"/> to <see cref="System.Char"/>.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <exception cref="ArgumentOutOfRangeException">The unicode character cannot be represented by a single character as it is a surrogate pair.</exception>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator char(UnicodeChar c)
        {
            if (c.CodePoint > 65536)
                throw new ArgumentOutOfRangeException(
                    nameof(c),
                    "The unicode character cannot be represented by a single character as it is a surrogate pair.");
            return (char)c.CodePoint;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="UnicodeChar"/>.
        /// </summary>
        /// <param name="codePoint">The code point.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator UnicodeChar(int codePoint) => new UnicodeChar(codePoint);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Char" /> to <see cref="UnicodeChar" />.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator UnicodeChar(char c) => new UnicodeChar(c);

        /// <summary>Returns the <see cref="T:System.TypeCode" /> for value type <see cref="T:System.Char" />.</summary>
        /// <returns>The enumerated constant, <see cref="F:System.TypeCode.Char" />.</returns>
        public TypeCode GetTypeCode() => TypeCode.Char;

        /// <inheritdoc/>
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException("Cannot cast from UnicodeChar to Boolean.");
        }

        /// <inheritdoc/>
        char IConvertible.ToChar(IFormatProvider provider) => (char)this;

        /// <inheritdoc/>
        sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte(CodePoint);

        /// <inheritdoc/>
        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(CodePoint);

        /// <inheritdoc/>
        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(CodePoint);

        /// <inheritdoc/>
        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(CodePoint);

        /// <inheritdoc/>
        int IConvertible.ToInt32(IFormatProvider provider) => CodePoint;


        /// <inheritdoc/>
        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(CodePoint);


        /// <inheritdoc/>
        long IConvertible.ToInt64(IFormatProvider provider) => CodePoint;


        /// <inheritdoc/>
        ulong IConvertible.ToUInt64(IFormatProvider provider) => (ulong)CodePoint;


        /// <inheritdoc/>
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException("Cannot cast from UnicodeChar to Single.");
        }

        /// <inheritdoc/>
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException("Cannot cast from UnicodeChar to Double.");
        }

        /// <inheritdoc/>
        Decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException("Cannot cast from UnicodeChar to Decimal.");
        }

        /// <inheritdoc/>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException("Cannot cast from UnicodeChar to DateTime.");
        }

        /// <inheritdoc/>
        object IConvertible.ToType(Type type, IFormatProvider provider) => ((IConvertible)CodePoint).ToType(type, provider);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> forms a surrogate pair.
        /// </summary>
        /// <value><c>true</c> if this instance is surrogate pair; otherwise, <c>false</c>.</value>
        /// <param name="c">The c.</param>
        public bool IsSurrogatePair => CodePoint > 65536;

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a decimal digit.
        /// </summary>
        /// <value><c>true</c> if this instance is digit; otherwise, <c>false</c>.</value>
        public bool IsDigit => CodePoint > 65536
            ? char.IsDigit(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsDigit((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a Unicode letter.
        /// </summary>
        /// <value><c>true</c> if this instance is letter; otherwise, <c>false</c>.</value>
        public bool IsLetter => CodePoint > 65536
            ? char.IsLetter(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsLetter((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as white space.
        /// </summary>
        /// <value><c>true</c> if this instance is white space; otherwise, <c>false</c>.</value>
        public bool IsWhiteSpace => CodePoint > 65536
            ? char.IsWhiteSpace(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsWhiteSpace((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" />is categorized as an uppercase letter.
        /// </summary>
        /// <value><c>true</c> if this instance is upper; otherwise, <c>false</c>.</value>
        public bool IsUpper => CodePoint > 65536
            ? char.IsUpper(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsUpper((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a lowercase letter.
        /// </summary>
        /// <value><c>true</c> if this instance is lower; otherwise, <c>false</c>.</value>
        public bool IsLower => CodePoint > 65536
            ? char.IsLower(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsLower((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a punctuation mark.
        /// </summary>
        /// <value><c>true</c> if this instance is punctuation; otherwise, <c>false</c>.</value>
        public bool IsPunctuation => CodePoint > 65536
            ? char.IsPunctuation(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsPunctuation((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a letter or a decimal digit.
        /// </summary>
        /// <value><c>true</c> if this instance is letter or digit; otherwise, <c>false</c>.</value>
        public bool IsLetterOrDigit => CodePoint > 65536
            ? char.IsLetterOrDigit(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsLetterOrDigit((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a control character.
        /// </summary>
        /// <value><c>true</c> if this instance is control; otherwise, <c>false</c>.</value>
        public bool IsControl => CodePoint > 65536
            ? char.IsControl(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsControl((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a number.
        /// </summary>
        /// <value><c>true</c> if this instance is number; otherwise, <c>false</c>.</value>
        public bool IsNumber => CodePoint > 65536
            ? char.IsNumber(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsNumber((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a separator character.
        /// </summary>
        /// <value><c>true</c> if this instance is separator; otherwise, <c>false</c>.</value>
        public bool IsSeparator => CodePoint > 65536
            ? char.IsSeparator(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsSeparator((char)CodePoint);

        /// <summary>
        /// Indicates whether this <see cref="T:UnicodeChar" /> is categorized as a symbol character.
        /// </summary>
        /// <value><c>true</c> if this instance is symbol; otherwise, <c>false</c>.</value>
        public bool IsSymbol => CodePoint > 65536
            ? char.IsSymbol(char.ConvertFromUtf32(CodePoint), 0)
            : char.IsSymbol((char)CodePoint);

        /// <summary>
        /// Categorizes this <see cref="T:UnicodeChar" /> into a group identified by one of the <see cref="T:System.Globalization.UnicodeCategory" /> values.
        /// </summary>
        /// <returns>A <see cref="T:System.Globalization.UnicodeCategory" /> value that identifies the group that contains this <see cref="T:UnicodeChar" />.</returns>
        public UnicodeCategory GetUnicodeCategory() => CodePoint > 65536
            ? char.GetUnicodeCategory(char.ConvertFromUtf32(CodePoint), 0)
            : char.GetUnicodeCategory((char)CodePoint);

        /// <summary>
        /// Converts this <see cref="T:UnicodeChar" /> to a double-precision floating point number.
        /// </summary>
        /// <returns>The numeric value of this <see cref="T:UnicodeChar" /> if the character represents a number; otherwise, -1.0.</returns>
        public double GetNumericValue() => CodePoint > 65536
            ? char.GetNumericValue(char.ConvertFromUtf32(CodePoint), 0)
            : char.GetNumericValue((char)CodePoint);

        /// <summary>
        /// Converts the value of this <see cref="T:UnicodeChar" /> to its uppercase equivalent using specified culture-specific formatting information.
        /// </summary>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>The uppercase equivalent of this <see cref="T:UnicodeChar" />, modified according to <paramref name="culture" />, 
        /// or the unchanged value of this <see cref="T:UnicodeChar" /> if this <see cref="T:UnicodeChar" /> is already uppercase, has no uppercase equivalent, or is not alphabetic.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="culture" /> is null.</exception>
        public UnicodeChar ToUpper([NotNull]CultureInfo culture) => CodePoint > 65536
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            ? Parse(char.ConvertFromUtf32(CodePoint).ToUpper(culture))
            : (UnicodeChar)char.ToUpper((char)CodePoint);

        /// <summary>
        /// Converts the value of this <see cref="T:UnicodeChar" /> to its uppercase equivalent.
        /// </summary>
        /// <returns>The uppercase equivalent of this <see cref="T:UnicodeChar" />, or the unchanged value of this <see cref="T:UnicodeChar" />
        /// if this <see cref="T:UnicodeChar" /> is already uppercase, has no uppercase equivalent, or is not alphabetic.</returns>
        public UnicodeChar ToUpper() => CodePoint > 65536
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            ? Parse(char.ConvertFromUtf32(CodePoint).ToUpper())
            : (UnicodeChar)char.ToUpper((char)CodePoint);

        /// <summary>
        /// Converts the value of this <see cref="T:UnicodeChar" /> to its uppercase equivalent using the casing rules of the invariant culture.
        /// </summary>
        /// <returns>The uppercase equivalent of this <see cref="T:UnicodeChar" /> parameter, or the unchanged value of this <see cref="T:UnicodeChar" />,
        /// if this <see cref="T:UnicodeChar" /> is already uppercase or not alphabetic.</returns>
        public UnicodeChar ToUpperInvariant() => CodePoint > 65536
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            ? Parse(char.ConvertFromUtf32(CodePoint).ToUpperInvariant())
            : (UnicodeChar)char.ToUpperInvariant((char)CodePoint);

        /// <summary>
        /// Converts the value of this <see cref="T:UnicodeChar" /> to its lowercase equivalent using specified culture-specific formatting information.
        /// </summary>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>The lowercase equivalent of this <see cref="T:UnicodeChar" />, modified according to this <see cref="T:UnicodeChar" />,
        /// or the unchanged value of this <see cref="T:UnicodeChar" />, if this <see cref="T:UnicodeChar" /> is already lowercase or not alphabetic.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="culture" /> is null.</exception>
        public UnicodeChar ToLower([NotNull] CultureInfo culture) => CodePoint > 65536
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            ? Parse(char.ConvertFromUtf32(CodePoint).ToLower(culture))
            : (UnicodeChar)char.ToLower((char)CodePoint);

        /// <summary>
        /// Converts the value of this <see cref="T:UnicodeChar" /> to its lowercase equivalent.
        /// </summary>
        /// <returns>The lowercase equivalent of this <see cref="T:UnicodeChar" />, or the unchanged value of this <see cref="T:UnicodeChar" />,
        /// if this <see cref="T:UnicodeChar" /> is already lowercase or not alphabetic.</returns>
        public UnicodeChar ToLower() => CodePoint > 65536
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            ? Parse(char.ConvertFromUtf32(CodePoint).ToLower())
            : (UnicodeChar)char.ToLower((char)CodePoint);

        /// <summary>
        /// Converts the value of this <see cref="T:UnicodeChar" /> to its lowercase equivalent using the casing rules of the invariant culture.
        /// </summary>
        /// <returns>The lowercase equivalent of the this <see cref="T:UnicodeChar" /> parameter, or the unchanged value of this <see cref="T:UnicodeChar" />,
        /// if this <see cref="T:UnicodeChar" /> is already lowercase or not alphabetic.</returns>
        public UnicodeChar ToLowerInvariant() => CodePoint > 65536
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            ? Parse(char.ConvertFromUtf32(CodePoint).ToLowerInvariant())
            : (UnicodeChar)char.ToLowerInvariant((char)CodePoint);

        /// <summary>
        /// Converts the value of the specified string to its equivalent Unicode character.
        /// </summary>
        /// <param name="s">A string that contains one of two characters.</param>
        /// <returns>A Unicode character equivalent to the character(s) in <paramref name="s" />.</returns>
        /// <exception cref="System.ArgumentNullException">s</exception>
        /// <exception cref="System.ArgumentException">
        /// The string must be of length 1.
        /// or
        /// The first character is not a valid low surrogate. - s
        /// or
        /// The string must be of length 2.
        /// or
        /// The second character is not a valid high surrogate. - s
        /// </exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="s" /> is null.</exception>
        /// <exception cref="T:System.FormatException">The length of <paramref name="s" /> is not 1.</exception>
        public static UnicodeChar Parse([NotNull]string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            int a = s[0];
            int num1 = a - 55296;
            if (num1 < 0 || num1 > 2047)
            {
                if (s.Length != 1)
                    throw new ArgumentException("The string must be of length 1.");
                return new UnicodeChar(a);
            }
            if (num1 > 1023)
                throw new ArgumentException(
                    "The first character is not a valid low surrogate.",
                    nameof(s));

            if (s.Length != 2)
                throw new ArgumentException("The string must be of length 2.");
            int num2 = s[1] - 56320;
            if (num2 >= 0 && num2 <= 1023)
                return new UnicodeChar(num1 * 1024 + num2 + 65536);
            throw new ArgumentException(
                "The second character is not a valid high surrogate.",
                nameof(s));
        }

        /// <summary>
        /// Converts the value of the specified string to its equivalent Unicode character. A return code indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string that contains a single character, or null.</param>
        /// <param name="result">When this method returns, contains a Unicode character equivalent to the sole character in <paramref name="s" />, if the conversion succeeded, or an undefined value if the conversion failed. The conversion fails if the <paramref name="s" /> parameter is null or the length of <paramref name="s" /> is not 1. This parameter is passed uninitialized.</param>
        /// <returns>true if the <paramref name="s" /> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string s, out UnicodeChar result)
        {
            if (s == null)
            {
                result = new UnicodeChar(0);
                return false;
            }
            int a = s[0];
            int num1 = a - 55296;
            if (num1 < 0 || num1 > 2047)
            {
                if (s.Length != 1)
                {
                    result = new UnicodeChar(0);
                    return false;
                }
                result = new UnicodeChar(a);
                return true;
            }
            if (num1 > 1023 ||
                s.Length != 2)
            {
                result = new UnicodeChar(0);
                return false;
            }

            int num2 = s[1] - 56320;
            if (num2 >= 0 && num2 <= 1023)
            {
                result = new UnicodeChar(num1 * 1024 + num2 + 65536);
                return true;
            }
            result = new UnicodeChar(0);
            return false;
        }

        /// <summary>
        /// Performs a binary search on the code points collection.
        /// </summary>
        /// <typeparam name="T">The property enumeration type.</typeparam>
        /// <param name="codePoint">The code point to find.</param>
        /// <param name="codePoints">The code points collection.</param>
        /// <param name="asciiIndex">Index of the last code point range which is less than or equal to the <see cref="AsciiMaxCodePoint" />.</param>
        /// <param name="notFound">The value to return when the code point was not found.</param>
        /// <returns>The code point property.</returns>
        private static T BinarySearchCodePoints<T>(int codePoint, [NotNull] int[] codePoints, int asciiIndex, T notFound)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            // The binary search searches between two indices, start by setting the min and max to the full range.
            int minIndex = 0;
            // Note we divide by 3, as the array has 3 pieces of information for each codepoint.
            int maxIndex = (codePoints.Length - 1) / 3;

            if (codePoint <= AsciiMaxCodePoint)
                // Our first optimization is to notice that in the majority of use cases we will be searching for ASCII characters,
                // so we can immediately strip the majority of the codePoints set.
                maxIndex = asciiIndex;
            else if (codePoint - AsciiMaxCodePoint < maxIndex - asciiIndex)
            {
                // As the ranges are ordered AND are at least length 1, then the maximum index cannot be greater than the code point itself - 
                // this is a lovely and cheap optimization for relatively low code-points.  We further improve by ignoring the first 'asciiIndex'
                // code points as we already know that codePoint doesn't lie here.
                minIndex = asciiIndex + 1;
                maxIndex = codePoint - AsciiMaxCodePoint + asciiIndex;
            }

            do
            {
                // Find midpoint
                int index = (minIndex + maxIndex) >> 1;

                // Calculate actual index in codePoints collection.
                int actualIndex = index * 3;
                int start = codePoints[actualIndex++];

                // Optimization to prevent lookup of end if we already match start.
                if (start == codePoint) return (T)(object)codePoints[actualIndex + 1];

                // If start > codePoint then our maximum index is the index before this one.
                if (start > codePoint) maxIndex = index - 1;

                // If end < codePoint then our minimum index is the index after this one.
                else if (codePoints[actualIndex++] < codePoint) minIndex = index + 1;

                // If start <= codePoint && end >= codePoint, we're overlapping and we're done
                else return (T)(object)codePoints[actualIndex];
            } while (minIndex <= maxIndex);

            // Didn't find the code point.
            return notFound;
        }
    }
}