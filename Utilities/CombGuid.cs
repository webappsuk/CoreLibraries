#region © Copyright Web Applications (UK) Ltd, 2013.  All rights reserved.
// Copyright (c) 2013, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   A <see cref="Guid"/> that encodes its creation date and can be used with SQL Server to create efficient inserts and indexes.
    /// </summary>
    /// <remarks>
    ///   The encoded date is accurate to ~3.4ms (or ~8ms from SQL) depending on the source.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    [ComVisible(true)]
    [UsedImplicitly]
    public struct CombGuid : IFormattable, IComparable, IComparable<CombGuid>, IEquatable<CombGuid>, IComparable<Guid>,
                             IEquatable<Guid>
    {
        /// <summary>
        ///   The empty <see cref="CombGuid"/>.
        /// </summary>
        [UsedImplicitly] public static readonly CombGuid Empty;

        /// <summary>
        ///   The <see cref="System.Guid"/> component of the <see cref="CombGuid"/>.
        /// </summary>
        [UsedImplicitly] public readonly Guid Guid;

        /// <summary>
        ///   The base UTC date time used during new <see cref="System.DateTime"/> calculations.
        /// </summary>
        private static readonly DateTime _baseDate = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///   The UTC <see cref="System.DateTime"/> encoded into the <see cref="CombGuid"/>.
        /// </summary>
        /// <remarks>This is accurate to ~3.4ms (or ~8ms from SQL) depending on the source.</remarks>
        public readonly DateTime Created;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CombGuid"/> struct.
        /// </summary>
        /// <param name="b">A 16-element byte array containing values with which to initialize the Guid.</param>
        /// <seealso cref="System.Guid"/>
        public CombGuid([NotNull] byte[] b)
        {
            Guid = new Guid(b);
            Created = GetDateTime(Guid);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CombGuid"/> struct.
        /// </summary>
        /// <param name="a">The first 4 bytes of the Guid.</param>
        /// <param name="b">The next 2 bytes of the Guid.</param>
        /// <param name="c">The next 2 bytes of the Guid.</param>
        /// <param name="d">The next byte of the Guid.</param>
        /// <param name="e">The next byte of the Guid.</param>
        /// <param name="f">The next byte of the Guid.</param>
        /// <param name="g">The next byte of the Guid.</param>
        /// <param name="h">The next byte of the Guid.</param>
        /// <param name="i">The next byte of the Guid.</param>
        /// <param name="j">The next byte of the Guid.</param>
        /// <param name="k">The final byte of the Guid.</param>
        /// <remarks>
        ///   Specifying the bytes in this manner can be used to circumvent byte order restrictions on certain computers (endianness).
        /// </remarks>
        /// <seealso cref="System.Guid"/>
        public CombGuid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            Guid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
            Created = GetDateTime(Guid);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CombGuid"/> struct.
        /// </summary>
        /// <param name="a">The first 4 bytes of the Guid.</param>
        /// <param name="b">The next 2 bytes of the Guid.</param>
        /// <param name="c">The next 2 bytes of the Guid.</param>
        /// <param name="d">The final 8 bytes of the Guid.</param>
        /// <seealso cref="System.Guid"/>
        public CombGuid(int a, short b, short c, [NotNull] byte[] d)
        {
            Guid = new Guid(a, b, c, d);
            Created = GetDateTime(Guid);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CombGuid"/> struct.
        /// </summary>
        /// <param name="a">The first 4 bytes of the Guid.</param>
        /// <param name="b">The next 2 bytes of the Guid.</param>
        /// <param name="c">The next 2 bytes of the Guid.</param>
        /// <param name="d">The next byte of the Guid.</param>
        /// <param name="e">The next byte of the Guid.</param>
        /// <param name="f">The next byte of the Guid.</param>
        /// <param name="g">The next byte of the Guid.</param>
        /// <param name="h">The next byte of the Guid.</param>
        /// <param name="i">The next byte of the Guid.</param>
        /// <param name="j">The next byte of the Guid.</param>
        /// <param name="k">The final byte of the Guid.</param>
        /// <remarks>
        ///   Specifying the bytes in this manner can be used to circumvent byte order restrictions on certain computers (endianness).
        /// </remarks>
        /// <seealso cref="System.Guid"/>
        public CombGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            Guid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
            Created = GetDateTime(Guid);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CombGuid"/> using the <see cref="string"/> specified. 
        /// </summary>
        /// <param name="g">
        ///   <para>The string containing a Guid. The format can be one of the following (d represents a hex digit):</para>
        ///   <para>32 continuous digits: dddddddddddddddddddddddddddddddd.</para>
        ///   <para>-or-</para>
        ///   <para>Groups of 8, 4, 4, 4 and 12 digits: dddddddd-dddd-dddd-dddd-dddddddddddd</para>
        ///   <para>(The entire GUID can be enclosed in either braces or parenthesis.)</para>
        ///   <para>-or-</para>
        ///   <para>Groups of 8, 4, and 4 digits, then a subset of 8 groups of 2 digits. Each group is prefixed by "0x" or "0X"
        ///   and is separated by commas. The entire GUID, as well as the subset, is enclosed in matching braces:
        ///   {0xdddddddd, 0xdddd, 0xdddd,{0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd}}</para>
        ///   <para>Embedded spaces and leading zeros are ignored as well as leading zeros in a group.</para>
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="g"/> is a <see langword="null"/>.</exception>
        /// <exception cref="FormatException">The format of <paramref name="g"/> is invalid.</exception>
        /// <exception cref="OverflowException">The format of <paramref name="g"/> is invalid.</exception>
        public CombGuid([NotNull] String g)
        {
            Guid = new Guid(g);
            Created = GetDateTime(Guid);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CombGuid"/> struct.
        /// </summary>
        /// <param name="g">The Guid.</param>
        /// <seealso cref="System.Guid"/>
        public CombGuid(Guid g)
        {
            Guid = g;
            Created = GetDateTime(Guid);
        }

        /// <summary>
        ///   Parses the specified <see cref="string"/> into a <see cref="CombGuid"/> equivalent.
        /// </summary>
        /// <param name="input">The Guid to parse.</param>
        /// <returns>
        ///   A <see cref="CombGuid"/> containing the parsed <paramref name="input"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="input"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <paramref name="input"/> is not in a recognised format.
        /// </exception>
        [UsedImplicitly]
        public static CombGuid Parse([NotNull] String input)
        {
            return new CombGuid(Guid.Parse(input));
        }

        /// <summary>
        ///   Tries to parse the specified <see cref="string"/> into a <see cref="CombGuid"/> equivalent.
        /// </summary>
        /// <param name="input">The Guid to parse.</param>
        /// <param name="result">
        ///   If the <paramref name="input"/> is parsed successfully, a <see cref="CombGuid"/> containing the parsed value
        ///   is output; otherwise an empty <see cref="CombGuid"/>.
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the parse was successful; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool TryParse(String input, out CombGuid result)
        {
            Guid g;
            if (Guid.TryParse(input, out g))
            {
                result = new CombGuid(g);
                return true;
            }
            result = Empty;
            return false;
        }

        /// <summary>
        ///   Parses the <see cref="string"/> input into the equivalent <see cref="CombGuid"/> (with the format specified).
        /// </summary>
        /// <param name="input">The Guid to parse.</param>
        /// <param name="format">The format specifier.</param>
        /// <returns>
        ///   A <see cref="CombGuid"/> containing the parsed <paramref name="input"/>, which is in the specified <paramref name="format"/>.
        /// </returns>
        /// <remarks>
        ///   The <paramref name="format"/> can be:
        ///   <list type="bullet">
        ///     <item><description>"N" - 32 digits.</description></item>
        ///     <item><description>"D" - 32 digits separated by hyphens.</description></item>
        ///     <item><description>"B" - 32 digits separated by hyphens (enclosed in braces).</description></item>
        ///     <item><description>"P" - 32 digits separated by hyphens (enclosed in parenthesis).</description></item>
        ///     <item><description>"X" - Four hex values (enclosed in braces) where the fourth value is a subset of eight hex values (also enclosed in braces).
        ///     ({0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}</description></item>
        ///   </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is a <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        ///   The value of <paramref name="format"/> is not one of the following:
        ///   <list type="bullet">
        ///     <item><description>"N"</description></item>
        ///     <item><description>"D"</description></item>
        ///     <item><description>"B"</description></item>
        ///     <item><description>"P"</description></item>
        ///     <item><description>"X"</description></item>
        ///   </list>
        /// </exception>
        [UsedImplicitly]
        public static CombGuid ParseExact([NotNull] String input, [NotNull] String format)
        {
            return new CombGuid(Guid.ParseExact(input, format));
        }

        /// <summary>
        ///   Parses the <see cref="string"/> input into the equivalent <see cref="CombGuid"/> (with the format specified).
        /// </summary>
        /// <param name="input">The Guid to parse.</param>
        /// <param name="format">The format string.</param>
        /// <param name="result">
        ///   If the <paramref name="input"/> is parsed successfully, a <see cref="CombGuid"/> containing the parsed
        ///   value is output; otherwise an empty <see cref="CombGuid"/>.
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the <paramref name="input"/> was parsed successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   <para>The <paramref name="format"/> can be:</para>
        ///   <list type="bullet">
        ///     <item><description>"N" - 32 digits.</description></item>
        ///     <item><description>"D" - 32 digits separated by hyphens.</description></item>
        ///     <item><description>"B" - 32 digits separated by hyphens (enclosed in braces).</description></item>
        ///     <item><description>"P" - 32 digits separated by hyphens (enclosed in parenthesis).</description></item>
        ///     <item><description>"X" - Four hex values (enclosed in braces) where the fourth value is a subset of eight hex values (also enclosed in braces).
        ///     ({0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}</description></item>
        ///   </list>
        /// </remarks>
        [UsedImplicitly]
        public static bool TryParseExact(String input, String format, out CombGuid result)
        {
            Guid g;
            if (Guid.TryParseExact(input, format, out g))
            {
                result = new CombGuid(g);
                return true;
            }
            result = Empty;
            return false;
        }

        /// <summary>
        ///   Returns an unsigned byte <see cref="Array">array</see> containing the <see cref="CombGuid"/>.
        /// </summary>
        /// <returns>
        ///   An unsigned 16-element byte <see cref="Array">array</see> containing the <see cref="CombGuid"/>.
        /// </returns>
        [UsedImplicitly]
        public byte[] ToByteArray()
        {
            return Guid.ToByteArray();
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance in "registry" format. 
        /// </summary>
        /// <returns>
        ///   <para>A <see cref="string"/> representation of this instance.</para>
        ///   <para><b>Output Format:</b> dddddddd-dddd-dddd-dddd-dddddddddddd (d represents a hex digit).</para>
        /// </returns>
        public override String ToString()
        {
            return Guid.ToString();
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        /// <remarks>This is suitable for use in hashing algorithms and data structures like a hash table.</remarks>
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        /// <summary>
        ///   Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="o">The object to compare with this instance.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        public override bool Equals(Object o)
        {
            if (o == null)
                return false;
            if (o is CombGuid)
                return Guid.Equals(((CombGuid) o).Guid);
            if (o is Guid)
                return Guid.Equals((Guid) o);
            return false;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="CombGuid"/> is equal to this instance.
        /// </summary>
        /// <param name="g">
        ///   The <see cref="CombGuid"/> to compare with this instance.
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <see cref="CombGuid"/> (<paramref name="g"/>) is equal to this instance;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Equals(CombGuid g)
        {
            return Guid.Equals(g.Guid);
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Guid"/> to compare with this instance.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the <paramref name="other"/> <see cref="System.Guid"/> is equal to this instance;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Equals(Guid other)
        {
            return Guid.Equals(other);
        }

        /// <summary>
        ///   Compares this instance to the specified <see cref="object"/>.
        /// </summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>
        ///   A signed number indicating the the relative value between <paramref name="value"/> and this instance.
        ///   <list type="bullet">
        ///     <item><description>A negative integer: The instance is less than <paramref name="value"/>.</description></item>
        ///     <item><description>Zero: The instance equal to <paramref name="value"/>.</description></item>
        ///     <item><description>A positive integer: The instance is greater than <paramref name="value"/> or <paramref name="value"/>
        ///     is <see langword="null">null</see>.</description></item>
        ///   </list>
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="value"/> was not a <see cref="System.Guid"/>/<see cref="CombGuid"/>.
        /// </exception>
        public int CompareTo(Object value)
        {
            if (value == null)
                return 1;
            if (value is CombGuid)
                return Guid.CompareTo(((CombGuid) value).Guid);
            if (value is Guid)
                return Guid.CompareTo((Guid) value);

            throw new ArgumentException("Can only compare a CombGuid to a CombGuid.");
        }

        /// <summary>
        ///   Compares this instance to the specified <see cref="CombGuid"/>.
        /// </summary>
        /// <param name="value">The <see cref="CombGuid"/> to compare to this instance.</param>
        /// <returns>
        ///   <para>A signed number indicating the relative value between <paramref name="value"/> and this instance.</para>
        ///   <list type="bullet">
        ///     <item><description>A negative integer: The instance is less than <paramref name="value"/>.</description></item>
        ///     <item><description>Zero: The instance equal to <paramref name="value"/>.</description></item>
        ///     <item><description>A positive integer: The instance is greater than <paramref name="value"/>.</description></item>
        ///   </list>
        /// </returns>
        public int CompareTo(CombGuid value)
        {
            return Guid.CompareTo(value.Guid);
        }

        /// <summary>
        ///   Compares this instance to the specified <see cref="System.Guid"/>.
        /// </summary>
        /// <returns>
        ///   <para>A signed number indicating the relative value between <paramref name="other"/> and this instance.</para>
        ///   <list type="bullet">
        ///     <item><description>A negative integer: The instance is less than <paramref name="other"/>.</description></item>
        ///     <item><description>Zero: The instance equal to <paramref name="other"/>.</description></item>
        ///     <item><description>A positive integer: The instance is greater than <paramref name="other"/>.</description></item>
        ///   </list>
        /// </returns>
        public int CompareTo(Guid other)
        {
            return Guid.CompareTo(other);
        }

        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// <param name="a">The first <see cref="CombGuid"/> to compare.</param>
        /// <param name="b">The second <see cref="CombGuid"/> to compare.</param>
        public static bool operator ==(CombGuid a, CombGuid b)
        {
            return a.Guid == b.Guid;
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// <param name="a">The first <see cref="CombGuid"/> to compare.</param>
        /// <param name="b">The second <see cref="CombGuid"/> to compare.</param>
        public static bool operator !=(CombGuid a, CombGuid b)
        {
            return a.Guid != b.Guid;
        }

        /// <summary>
        ///   Implements the operator &gt;.
        /// </summary>
        /// <param name="a">The first <see cref="CombGuid"/> to compare.</param>
        /// <param name="b">The second <see cref="CombGuid"/> to compare.</param>
        public static bool operator >(CombGuid a, CombGuid b)
        {
            return a.Guid.CompareTo(b.Guid) > 0;
        }

        /// <summary>
        ///   Implements the operator &lt;.
        /// </summary>
        /// <param name="a">The first <see cref="CombGuid"/> to compare.</param>
        /// <param name="b">The second <see cref="CombGuid"/> to compare.</param>
        public static bool operator <(CombGuid a, CombGuid b)
        {
            return a.Guid.CompareTo(b.Guid) < 0;
        }

        /// <summary>
        ///   Implements the operator &gt;=.
        /// </summary>
        /// <param name="a">The first <see cref="CombGuid"/> to compare.</param>
        /// <param name="b">The second <see cref="CombGuid"/> to compare.</param>
        public static bool operator >=(CombGuid a, CombGuid b)
        {
            return a.Guid.CompareTo(b.Guid) >= 0;
        }

        /// <summary>
        ///   Implements the operator &lt;=.
        /// </summary>
        /// <param name="a">The first <see cref="CombGuid"/> to compare.</param>
        /// <param name="b">The second <see cref="CombGuid"/> to compare.</param>
        public static bool operator <=(CombGuid a, CombGuid b)
        {
            return a.Guid.CompareTo(b.Guid) <= 0;
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="CombGuid"/> to <see cref="System.Guid"/>.
        /// </summary>
        /// <param name="combGuid">The <see cref="CombGuid"/> to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Guid(CombGuid combGuid)
        {
            return combGuid.Guid;
        }

        /// <summary>
        ///   Performs an implicit conversion from <see cref="System.Guid"/> to <see cref="CombGuid"/>.
        /// </summary>
        /// <param name="guid">The <see cref="System.Guid"/> to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator CombGuid(Guid guid)
        {
            return new CombGuid(guid);
        }

        /// <summary>
        ///   Creates a new <see cref="CombGuid"/> with the current <see cref="System.DateTime.Now"/> embedded
        ///   as its creation date.
        /// </summary>
        /// <returns>A new <see cref="CombGuid"/> instance.</returns>
        [SecuritySafeCritical]
        [UsedImplicitly]
        public static CombGuid NewCombGuid()
        {
            return NewCombGuid(DateTime.UtcNow);
        }

        /// <summary>
        ///   Creates a new <see cref="CombGuid"/> based on the <see cref="System.DateTimeOffset">date time</see> specified.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A new <see cref="CombGuid"/> instance.</returns>
        [SecuritySafeCritical]
        [UsedImplicitly]
        public static CombGuid NewCombGuid(DateTimeOffset dateTime)
        {
            return NewCombGuid(dateTime.UtcDateTime);
        }

        /// <summary>
        ///   Creates a new <see cref="CombGuid"/> based on the <see cref="System.DateTime">date time</see> specified.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A new <see cref="CombGuid"/> instance.</returns>
        [SecuritySafeCritical]
        [UsedImplicitly]
        public static CombGuid NewCombGuid(DateTime dateTime)
        {
            // Always convert to universal time.
            dateTime = dateTime.ToUniversalTime();
            if (dateTime < _baseDate)
                throw new ArgumentOutOfRangeException(
                    "dateTime",
                    Resources.CombGuid_InvalidDateTime_TooEarly,
                    _baseDate.ToString());

            // First of all get a new GUID into a byte[].
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            // Convert to a byte array  
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333  
            byte[] daysArray = BitConverter.GetBytes((dateTime.Date - _baseDate).Days);
            byte[] msecsArray = BitConverter.GetBytes((long) ((dateTime - dateTime.Date).TotalMilliseconds/3.333333));

            // We place the first four bytes of the msecs array into the last four bytes of the GUID (in reverse).
            guidArray[15] = msecsArray[0];
            guidArray[14] = msecsArray[1];
            guidArray[13] = msecsArray[2];
            guidArray[12] = msecsArray[3];

            // Then we take the first two bytes of the days.
            guidArray[11] = daysArray[0];
            guidArray[10] = daysArray[1];

            return new CombGuid(guidArray);
        }

        /// <summary>
        ///   Gets the <see cref="System.Guid"/>'s creation date (if it's a <see cref="CombGuid"/>).
        /// </summary>
        /// <param name="guid">The Guid.</param>
        /// <returns>
        ///   The <see cref="System.Guid"/>'s creation <see cref="System.DateTime"/> (always UTC).
        /// </returns>
        /// <remarks>
        ///   The creation date is stored in the last 6 bytes of the <see cref="System.Guid"/>.
        ///   A group of 4 bytes for the msecs and a group of 2 bytes for the days.
        /// </remarks>
        [UsedImplicitly]
        public static DateTime GetDateTime(Guid guid)
        {
            // First of all get the GUID into a byte[].
            byte[] guidArray = guid.ToByteArray();

            byte[] daysArray = new byte[4];
            byte[] msecsArray = new byte[8];

            // We place the first four bytes of the msecs array into the last four bytes of the GUID (in reverse).
            msecsArray[0] = guidArray[15];
            msecsArray[1] = guidArray[14];
            msecsArray[2] = guidArray[13];
            msecsArray[3] = guidArray[12];

            // Then we take the first two bytes of the days.
            daysArray[0] = guidArray[11];
            daysArray[1] = guidArray[10];

            return _baseDate.AddDays(BitConverter.ToInt32(daysArray, 0))
                            .AddMilliseconds(BitConverter.ToInt64(msecsArray, 0)*3.333333);
        }

        /// <summary>
        ///   Gets the creation <see cref="System.DateTime">date time</see> for the specified <see cref="CombGuid"/>.
        /// </summary>
        /// <param name="guid">The <see cref="CombGuid"/>.</param>
        /// <returns>The value of the <see cref="CombGuid"/>'s <see cref="Created"/> member.</returns>
        [UsedImplicitly]
        public static DateTime GetDateTime(CombGuid guid)
        {
            return guid.Created;
        }

        /// <summary>
        ///   Gets the <see cref="CombGuid"/>'s creation <see cref="System.DateTime">date time</see>.
        /// </summary>
        /// <returns>The value of the <see cref="Created"/> member.</returns>
        [UsedImplicitly]
        public DateTime GetDateTime()
        {
            return Created;
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format specifier.</param>
        /// <returns>A <see cref="string"/> representation of this instance.</returns>
        /// <remarks>
        ///   <para>The <paramref name="format"/> can be:</para>
        ///   <list type="bullet">
        ///     <item><description>"N" - 32 digits.</description></item>
        ///     <item><description>"D" - 32 digits separated by hyphens.</description></item>
        ///     <item><description>"B" - 32 digits separated by hyphens (enclosed in braces).</description></item>
        ///     <item><description>"P" - 32 digits separated by hyphens (enclosed in parenthesis).</description></item>
        ///     <item><description>"X" - Four hex values (enclosed in braces) where the fourth value is a subset of
        ///     eight hex values (also enclosed in braces).</description></item>
        ///   </list>
        ///   <para>If the format is a <see langword="null"/> or an empty string then "D" is used.</para>
        /// </remarks>
        /// <exception cref="FormatException">
        ///   <para>The value of <paramref name="format"/> is not one of the following:</para>
        ///   <list type="bullet">
        ///     <item><description><see langword="null">null</see></description></item>
        ///     <item><description>An empty string ("")</description></item>
        ///     <item><description>"N"</description></item>
        ///     <item><description>"D"</description></item>
        ///     <item><description>"B"</description></item>
        ///     <item><description>"P"</description></item>
        ///     <item><description>"X"</description></item>
        ///   </list>
        /// </exception>
        /// <seealso cref="System.Guid.ToString()">Guid.ToString</seealso>
        /// <seealso cref="System.IFormatProvider"/>
        [UsedImplicitly]
        public String ToString(String format)
        {
            return Guid.ToString(format, null);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format specifier.</param>
        /// <param name="provider">Controls culture specific formatting.</param>
        /// <returns>A <see cref="string"/> representation of this instance.</returns>
        /// <remarks>
        ///   <para>The <paramref name="format"/> can be:</para>
        ///   <list type="bullet">
        ///     <item><description>"N" - 32 digits.</description></item>
        ///     <item><description>"D" - 32 digits separated by hyphens.</description></item>
        ///     <item><description>"B" - 32 digits separated by hyphens (enclosed in braces).</description></item>
        ///     <item><description>"P" - 32 digits separated by hyphens (enclosed in parenthesis).</description></item>
        ///     <item><description>"X" - Four hex values (enclosed in braces) where the fourth value is a subset of eight hex values 
        ///     (also enclosed in braces). {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}</description></item>
        ///   </list>
        ///   <para>If the format is a <see langword="null"/> or an empty string then "D" is used.</para>
        /// </remarks>
        /// <exception cref="FormatException">
        ///   <para>The value of <paramref name="format"/> is not one of the following:</para>
        ///   <list type="bullet">
        ///     <item><description><see langword="null">null</see></description></item>
        ///     <item><description>An empty string ("")</description></item>
        ///     <item><description>"N"</description></item>
        ///     <item><description>"D"</description></item>
        ///     <item><description>"B"</description></item>
        ///     <item><description>"P"</description></item>
        ///     <item><description>"X"</description></item>
        ///   </list>
        /// </exception>
        /// <seealso cref="System.Guid.ToString()">Guid.ToString</seealso>
        /// <seealso cref="System.IFormatProvider"/>
        public String ToString(String format, IFormatProvider provider)
        {
            return Guid.ToString(format, provider);
        }
    }
}