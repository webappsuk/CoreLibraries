#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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
using System.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Class for assisting in building SQL query strings.
    /// </summary>
    [PublicAPI]
    public class SqlStringBuilder
    {
        [NotNull]
        private readonly StringBuilder _builder;

        /// <summary>
        /// Gets the underlying string builder.
        /// </summary>
        /// <value>The builder.</value>
        [NotNull]
        public StringBuilder Builder => _builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStringBuilder"/> class.
        /// </summary>
        public SqlStringBuilder() => _builder = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStringBuilder"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public SqlStringBuilder([NotNull] StringBuilder builder) 
            => _builder = builder ?? throw new ArgumentNullException(nameof(builder));

        /// <summary>
        /// Appends the identifier given, wrapping it in quotes (<c>[]</c>).
        /// </summary>
        /// <param name="identifier">The identifier to quote and append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        [NotNull]
        public SqlStringBuilder AppendIdentifier([NotNull] string identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            _builder.Append('[').Append(identifier.Replace("]", "]]")).Append(']');
            return this;
        }

        /// <summary>
        /// Appends the string given, wrapping it in quotes (<c>''</c>), escaping any single quote characters which appear in the string.
        /// </summary>
        /// <param name="value">The value to quote and append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        [NotNull]
        public SqlStringBuilder AppendVarChar([NotNull] string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            _builder.Append('\'').Append(value.Replace("'", "''")).Append('\'');
            return this;
        }

        /// <summary>
        /// Appends the unicode string given, wrapping it in quotes (<c>N''</c>), escaping any single quote characters which appear in the string.
        /// </summary>
        /// <param name="value">The value to quote and append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        [NotNull]
        public SqlStringBuilder AppendNVarChar(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            _builder.Append("N'").Append(value.Replace("'", "''")).Append('\'');
            return this;
        }

        #region StringBuilder methods
        /// <summary>Appends the string representation of a specified 8-bit unsigned integer to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(byte value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified 32-bit signed integer to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(int value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified 16-bit signed integer to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(short value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified Unicode character to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The Unicode character to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(char value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified double-precision floating-point number to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(double value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified 8-bit signed integer to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(sbyte value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified Boolean value to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The Boolean value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(bool value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified decimal number to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(decimal value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified 16-bit unsigned integer to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(ushort value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified 64-bit signed integer to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(long value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified 32-bit unsigned integer to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(uint value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified object to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The object to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(object value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends a copy of a specified substring to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The string that contains the substring to append. </param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value" />. </param>
        /// <param name="count">The number of characters in <paramref name="value" /> to append. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is null, and <paramref name="startIndex" /> and <paramref name="count" /> are not zero. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> less than zero.-or- <paramref name="startIndex" /> less than zero.-or- <paramref name="startIndex" /> + <paramref name="count" /> is greater than the length of <paramref name="value" />.-or- Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(string value, int startIndex, int count)
        {
            _builder.Append(value, startIndex, count);
            return this;
        }

        /// <summary>Appends a copy of the specified string to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The string to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(string value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified subarray of Unicode characters to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">A character array. </param>
        /// <param name="startIndex">The starting position in <paramref name="value" />. </param>
        /// <param name="charCount">The number of characters to append. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="value" /> is null, and <paramref name="startIndex" /> and <paramref name="charCount" /> are not zero. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="charCount" /> is less than zero.-or- <paramref name="startIndex" /> is less than zero.-or- <paramref name="startIndex" /> + <paramref name="charCount" /> is greater than the length of <paramref name="value" />.-or- Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(char[] value, int startIndex, int charCount)
        {
            _builder.Append(value, startIndex, charCount);
            return this;
        }

        /// <summary>Appends a specified number of copies of the string representation of a Unicode character to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The character to append. </param>
        /// <param name="repeatCount">The number of times to append <paramref name="value" />. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="repeatCount" /> is less than zero.-or- Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        /// <exception cref="T:System.OutOfMemoryException">Out of memory.</exception>
        [NotNull]
        public SqlStringBuilder Append(char value, int repeatCount)
        {
            _builder.Append(value, repeatCount);
            return this;
        }

        /// <summary>Appends the string representation of the Unicode characters in a specified array to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The array of characters to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(char[] value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified 64-bit unsigned integer to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(ulong value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string representation of a specified single-precision floating-point number to this instance.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The value to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder Append(float value)
        {
            _builder.Append(value);
            return this;
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
        /// <param name="format">A composite format string (see Remarks). </param>
        /// <param name="args">An array of objects to format. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="format" /> or <paramref name="args" /> is null. </exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args" /> array.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder AppendFormat(string format, params object[] args)
        {
            _builder.AppendFormat(format, args);
            return this;
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of three arguments.</summary>
        /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
        /// <param name="format">A composite format string (see Remarks). </param>
        /// <param name="arg0">The first object to format. </param>
        /// <param name="arg1">The second object to format. </param>
        /// <param name="arg2">The third object to format. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="format" /> is null. </exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="format" /> is invalid.-or-The index of a format item is less than 0 (zero), or greater than or equal to 3.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
        {
            _builder.AppendFormat(format, arg0, arg1, arg2);
            return this;
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a single argument.</summary>
        /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of <paramref name="arg0" />.</returns>
        /// <param name="format">A composite format string (see Remarks). </param>
        /// <param name="arg0">An object to format. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="format" /> is null. </exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to 1.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder AppendFormat(string format, object arg0)
        {
            _builder.AppendFormat(format, arg0);
            return this;
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        /// <returns>A reference to this instance after the append operation has completed. After the append operation, this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format" /> where any format specification is replaced by the string representation of the corresponding object argument. </returns>
        /// <param name="provider">An object that supplies culture-specific formatting information. </param>
        /// <param name="format">A composite format string (see Remarks). </param>
        /// <param name="args">An array of objects to format.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="format" /> is null. </exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="format" /> is invalid. -or-The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args" /> array.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
        {
            _builder.AppendFormat(provider, format, args);
            return this;
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of either of two arguments.</summary>
        /// <returns>A reference to this instance with <paramref name="format" /> appended. Each format item in <paramref name="format" /> is replaced by the string representation of the corresponding object argument.</returns>
        /// <param name="format">A composite format string (see Remarks). </param>
        /// <param name="arg0">The first object to format. </param>
        /// <param name="arg1">The second object to format. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="format" /> is null. </exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="format" /> is invalid.-or-The index of a format item is less than 0 (zero), or greater than or equal to 2. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder AppendFormat(string format, object arg0, object arg1)
        {
            _builder.AppendFormat(format, arg0, arg1);
            return this;
        }

        /// <summary>Appends a copy of the specified string followed by the default line terminator to the end of the current <see cref="T:System.Text.SqlStringBuilder" /> object.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <param name="value">The string to append. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder AppendLine(string value)
        {
            _builder.AppendLine(value);
            return this;
        }

        /// <summary>Appends the default line terminator to the end of the current <see cref="T:System.Text.SqlStringBuilder" /> object.</summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        [NotNull]
        public SqlStringBuilder AppendLine()
        {
            _builder.AppendLine();
            return this;
        }

        /// <summary>Gets or sets the maximum number of characters that can be contained in the memory allocated by the current instance.</summary>
        /// <returns>The maximum number of characters that can be contained in the memory allocated by the current instance.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than the current length of this instance.-or- The value specified for a set operation is greater than the maximum capacity. </exception>
        public int Capacity
        {
            get => _builder.Capacity;
            set => _builder.Capacity = value;
        }

        /// <summary>Gets or sets the character at the specified character position in this instance.</summary>
        /// <returns>The Unicode character at position <paramref name="index" />.</returns>
        /// <param name="index">The position of the character. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the bounds of this instance while setting a character. </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// <paramref name="index" /> is outside the bounds of this instance while getting a character. </exception>
        public char this[int index]
        {
            get => _builder[index];
            set => _builder[index] = value;
        }

        /// <summary>Copies the characters from a specified segment of this instance to a specified segment of a destination <see cref="T:System.Char" /> array.</summary>
        /// <param name="sourceIndex">The starting position in this instance where characters will be copied from. The index is zero-based.</param>
        /// <param name="destination">The array where characters will be copied.</param>
        /// <param name="destinationIndex">The starting position in <paramref name="destination" /> where characters will be copied. The index is zero-based.</param>
        /// <param name="count">The number of characters to be copied.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destination" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="sourceIndex" />, <paramref name="destinationIndex" />, or <paramref name="count" />, is less than zero.-or-<paramref name="sourceIndex" /> is greater than the length of this instance.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="sourceIndex" /> + <paramref name="count" /> is greater than the length of this instance.-or-<paramref name="destinationIndex" /> + <paramref name="count" /> is greater than the length of <paramref name="destination" />.</exception>
        public void CopyTo(int sourceIndex, [NotNull] char[] destination, int destinationIndex, int count)
        {
            _builder.CopyTo(sourceIndex, destination, destinationIndex, count);
        }

        /// <summary>Ensures that the capacity of this instance of <see cref="T:System.Text.SqlStringBuilder" /> is at least the specified value.</summary>
        /// <returns>The new capacity of this instance.</returns>
        /// <param name="capacity">The minimum capacity to ensure. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.-or- Enlarging the value of this instance would exceed <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        public int EnsureCapacity(int capacity) => _builder.EnsureCapacity(capacity);

        /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
        /// <returns>true if this instance and <paramref name="sb" /> have equal string, <see cref="SqlStringBuilder.Capacity" />, and <see cref="SqlStringBuilder.MaxCapacity" /> values; otherwise, false.</returns>
        /// <param name="sb">An object to compare with this instance, or null. </param>
        public bool Equals(SqlStringBuilder sb) => _builder.Equals(sb._builder);

        /// <summary>Gets or sets the length of the current <see cref="T:System.Text.SqlStringBuilder" /> object.</summary>
        /// <returns>The length of this instance.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than zero or greater than <see cref="SqlStringBuilder.MaxCapacity" />. </exception>
        public int Length
        {
            get => _builder.Length;
            set => _builder.Length = value;
        }

        /// <summary>Gets the maximum capacity of this instance.</summary>
        /// <returns>The maximum number of characters this instance can hold.</returns>
        public int MaxCapacity => _builder.MaxCapacity;

        /// <summary>Converts the value of a substring of this instance to a <see cref="T:System.String" />.</summary>
        /// <returns>A string whose value is the same as the specified substring of this instance.</returns>
        /// <param name="startIndex">The starting position of the substring in this instance. </param>
        /// <param name="length">The length of the substring. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> or <paramref name="length" /> is less than zero.-or- The sum of <paramref name="startIndex" /> and <paramref name="length" /> is greater than the length of the current instance. </exception>
        [NotNull]
        public string ToString(int startIndex, int length) => _builder.ToString(startIndex, length);

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) => Equals(obj as SqlStringBuilder);

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => _builder.GetHashCode();

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => _builder.ToString();
        #endregion

        /// <summary>
        /// Performs an implicit conversion from <see cref="StringBuilder"/> to <see cref="SqlStringBuilder"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SqlStringBuilder(StringBuilder builder) => new SqlStringBuilder(builder);
    }
}