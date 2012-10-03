#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.SqlServer.Types;
using WebApplications.Testing.Data;

namespace WebApplications.Testing
{
    /// <summary>
    /// Useful extension methods.
    /// </summary>
    public static class Tester
    {
        /// <summary>
        /// Creates one random generator per thread.
        /// </summary>
        [NotNull] private static readonly ThreadLocal<Random> _randomGenerators =
            new ThreadLocal<Random>(() => new Random());

        /// <summary>
        /// Valid SqlDbTypes for random generator.
        /// </summary>
        [NotNull] private static readonly SqlDbType[] _sqlDbTypes =
            new[]
                {
                    SqlDbType.BigInt,
                    SqlDbType.Binary,
                    SqlDbType.Bit,
                    SqlDbType.Char,
                    SqlDbType.DateTime,
                    SqlDbType.Decimal,
                    SqlDbType.Float,
                    SqlDbType.Image,
                    SqlDbType.Int,
                    SqlDbType.Money,
                    SqlDbType.NChar,
                    SqlDbType.NText,
                    SqlDbType.NVarChar,
                    SqlDbType.Real,
                    SqlDbType.UniqueIdentifier,
                    SqlDbType.SmallDateTime,
                    SqlDbType.SmallInt,
                    SqlDbType.SmallMoney,
                    SqlDbType.Text,
                    SqlDbType.Timestamp,
                    SqlDbType.TinyInt,
                    SqlDbType.VarBinary,
                    SqlDbType.VarChar,
                    SqlDbType.Xml,
                    SqlDbType.Udt,
                    SqlDbType.Date,
                    SqlDbType.Time,
                    SqlDbType.DateTime2,
                    SqlDbType.DateTimeOffset,
                };

        #region validSrids
        [NotNull] private static readonly int[] _validSrids = new[]
            {
                4120,
                4121,
                4122,
                4123,
                4124,
                4127,
                4128,
                4129,
                4130,
                4131,
                4132,
                4133,
                4134,
                4135,
                4136,
                4137,
                4138,
                4139,
                4141,
                4142,
                4143,
                4144,
                4145,
                4146,
                4147,
                4148,
                4149,
                4150,
                4151,
                4152,
                4153,
                4154,
                4155,
                4156,
                4157,
                4158,
                4159,
                4160,
                4161,
                4162,
                4163,
                4164,
                4165,
                4166,
                4167,
                4168,
                4169,
                4170,
                4171,
                4173,
                4174,
                4175,
                4176,
                4178,
                4179,
                4180,
                4181,
                4182,
                4183,
                4184,
                4188,
                4189,
                4190,
                4191,
                4192,
                4193,
                4194,
                4195,
                4196,
                4197,
                4198,
                4199,
                4200,
                4201,
                4202,
                4203,
                4204,
                4205,
                4206,
                4207,
                4208,
                4209,
                4210,
                4211,
                4212,
                4213,
                4214,
                4215,
                4216,
                4218,
                4219,
                4220,
                4221,
                4222,
                4223,
                4224,
                4225,
                4227,
                4229,
                4230,
                4231,
                4232,
                4236,
                4237,
                4238,
                4239,
                4240,
                4241,
                4242,
                4243,
                4244,
                4245,
                4246,
                4247,
                4248,
                4249,
                4250,
                4251,
                4252,
                4253,
                4254,
                4255,
                4256,
                4257,
                4258,
                4259,
                4261,
                4262,
                4263,
                4265,
                4266,
                4267,
                4268,
                4269,
                4270,
                4271,
                4272,
                4273,
                4274,
                4275,
                4276,
                4277,
                4278,
                4279,
                4280,
                4281,
                4282,
                4283,
                4284,
                4285,
                4286,
                4288,
                4289,
                4292,
                4293,
                4295,
                4297,
                4298,
                4299,
                4300,
                4301,
                4302,
                4303,
                4304,
                4306,
                4307,
                4308,
                4309,
                4310,
                4311,
                4312,
                4313,
                4314,
                4315,
                4316,
                4317,
                4318,
                4319,
                4322,
                4324,
                4326,
                4600,
                4601,
                4602,
                4603,
                4604,
                4605,
                4606,
                4607,
                4608,
                4609,
                4610,
                4611,
                4612,
                4613,
                4614,
                4615,
                4616,
                4617,
                4618,
                4619,
                4620,
                4621,
                4622,
                4623,
                4624,
                4625,
                4626,
                4627,
                4628,
                4629,
                4630,
                4632,
                4633,
                4636,
                4637,
                4638,
                4639,
                4640,
                4641,
                4642,
                4643,
                4644,
                4646,
                4657,
                4658,
                4659,
                4660,
                4661,
                4662,
                4663,
                4664,
                4665,
                4666,
                4667,
                4668,
                4669,
                4670,
                4671,
                4672,
                4673,
                4674,
                4675,
                4676,
                4677,
                4678,
                4679,
                4680,
                4682,
                4683,
                4684,
                4686,
                4687,
                4688,
                4689,
                4690,
                4691,
                4692,
                4693,
                4694,
                4695,
                4696,
                4697,
                4698,
                4699,
                4700,
                4701,
                4702,
                4703,
                4704,
                4705,
                4706,
                4707,
                4708,
                4709,
                4710,
                4711,
                4712,
                4713,
                4714,
                4715,
                4716,
                4717,
                4718,
                4719,
                4720,
                4721,
                4722,
                4723,
                4724,
                4725,
                4726,
                4727,
                4728,
                4729,
                4730,
                4732,
                4733,
                4734,
                4735,
                4736,
                4737,
                4738,
                4739,
                4740,
                4741,
                4742,
                4743,
                4744,
                4745,
                4746,
                4747,
                4748,
                4749,
                4750,
                4751,
                4752,
                4753,
                4754,
                4755,
                4756,
                4757,
                4758,
                4801,
                4802,
                4803,
                4804,
                4805,
                4806,
                4807,
                4808,
                4809,
                4810,
                4811,
                4813,
                4814,
                4815,
                4816,
                4817,
                4818,
                4820,
                4821,
                4895,
                4898,
                4900,
                4901,
                4902,
                4903,
                4904,
                4907,
                4909,
                4921,
                4923,
                4925,
                4927,
                4929,
                4931,
                4933,
                4935,
                4937,
                4939,
                4941,
                4943,
                4945,
                4947,
                4949,
                4951,
                4953,
                4955,
                4957,
                4959,
                4961,
                4963,
                4965,
                4967,
                4971,
                4973,
                4975,
                4977,
                4979,
                4981,
                4983,
                4985,
                4987,
                4989,
                4991,
                4993,
                4995,
                4997,
                4999,
                104001
            };
        #endregion validSrids

        /// <summary>
        /// A random number generator.
        /// </summary>
        [NotNull]
        public static Random RandomGenerator
        {
            get { return _randomGenerators.Value; }
        }

        /// <summary>
        /// Generates a random boolean.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Boolean"/>.</returns>
        public static bool RandomBoolean(this Random random)
        {
            return (random ?? RandomGenerator).Next(2) == 1;
        }

        /// <summary>
        /// Generates a random byte.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Byte"/>.</returns>
        public static byte RandomByte(this Random random)
        {
            return (byte) (random ?? RandomGenerator).Next(0x100);
        }

        /// <summary>
        /// Generates a random char.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Char"/>.</returns>
        public static char RandomChar(this Random random)
        {
            return (char) (random ?? RandomGenerator).Next(0x10000);
        }

        /// <summary>
        /// Generates a random Int16.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Int16"/>.</returns>
        public static short RandomInt16(this Random random)
        {
            byte[] bytes = new byte[2];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// Generates a random Int32.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Int32"/>.</returns>
        public static int RandomInt32(this Random random)
        {
            byte[] bytes = new byte[4];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Generates a random Int64.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Int64"/>.</returns>
        public static long RandomInt64(this Random random)
        {
            byte[] bytes = new byte[8];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// Generates a random float.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Single"/>.</returns>
        public static float RandomFloat(this Random random)
        {
            byte[] bytes = new byte[4];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Generates a random double.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Double"/>.</returns>
        public static double RandomDouble(this Random random)
        {
            byte[] bytes = new byte[8];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

        /// <summary>
        /// Generates a random decimal.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.Decimal"/>.</returns>
        public static decimal RandomDecimal(this Random random)
        {
            random = random ?? RandomGenerator;

            // Calculate last byte
            // We need a scale from 0-28 and a sign, so calculate a random number between -28 & 28.
            // This makes +'ves sligthly more common (as 0 is positive) but is faster.
            int scale = -28 + random.Next(57);
            int sign = 0;
            if (scale < 0)
            {
                sign = unchecked((int) 0x80000000);
                scale = -scale;
            }

            // Now we can create msb.
            int msb = sign + (scale << 16);

            return
                new decimal(new[]
                                {
                                    random.RandomInt32(),
                                    random.RandomInt32(),
                                    random.RandomInt32(),
                                    msb
                                });
        }

        /// <summary>
        /// Generates a random date time, with a specific <see cref="DateTimeKind" />.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <param name="kind">The <see cref="DateTimeKind" />.</param>
        /// <returns>A random <see cref="System.DateTime"/>.</returns>
        public static DateTime RandomDateTime(this Random random, DateTimeKind kind)
        {
            // Last two bits are used internally by date time.
            long ticks = RandomInt64(random) & 0x3FFFFFFFFFFFFFFF;

            // If ticks is more than max value, just and it to ensure less than max value.
            if (ticks > 0x2bca2875f4373fff)
                ticks &= 0x2bca2875f4373fff;

            return new DateTime(ticks, kind);
        }

        /// <summary>
        /// Generates a random date time.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.DateTime"/>.</returns>
        /// <remarks>Also generates a random <see cref="DateTimeKind" />.</remarks>
        public static DateTime RandomDateTime(this Random random)
        {
            random = random ?? RandomGenerator;
            // Last two bits are used internally by date time.
            long ticks = RandomInt64(random) & 0x3FFFFFFFFFFFFFFF;

            // If ticks is more than max value, just and it to ensure less than max value.
            if (ticks > 0x2bca2875f4373fff)
                ticks &= 0x2bca2875f4373fff;

            DateTimeKind kind;
            switch (random.Next(3))
            {
                case 0:
                    kind = DateTimeKind.Utc;
                    break;
                case 1:
                    kind = DateTimeKind.Local;
                    break;
                default:
                    kind = DateTimeKind.Unspecified;
                    break;
            }

            return new DateTime(ticks, kind);
        }

        /// <summary>
        /// Generates a random date time.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="System.DateTimeOffset"/>.</returns>
        /// <remarks>Also generates a random <see cref="DateTimeKind" />.</remarks>
        public static DateTimeOffset RandomDateTimeOffset(this Random random)
        {
            random = random ?? RandomGenerator;
            // Last two bits are used internally by date time.
            long ticks = random.RandomInt64() & 0x3FFFFFFFFFFFFFFF;

            // If ticks is more than max value, just and it to ensure less than max value.
            if (ticks > 0x2bca2875f4373fff)
                ticks &= 0x2bca2875f4373fff;

            // Calculate random offset +/- 14 hours in minutes (offsets must be in minutes)
            long offsetTicks = (TimeSpan.TicksPerHour*-14) + (TimeSpan.TicksPerMinute*random.Next(1680));

            // Ensure offsetTicks don't take us outside of the DateTime range.
            // We could decrease/increase ticks and round to nearest minute, but it is easier just to set to 0.
            long ticksOffset = offsetTicks - ticks;
            if ((ticksOffset < 0) ||
                (ticksOffset > 0x2bca2875f4373fff)) offsetTicks = 0;

            return new DateTimeOffset(ticks, TimeSpan.FromTicks(offsetTicks));
        }

        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <param name="unicode">if set to <see langword="true" /> string is UTF16; otherwise it uses ASCII.</param>
        /// <param name="nullProbability">The probability of a null being returned (0.0 for no nulls).</param>
        /// <returns>A random <see cref="System.String"/>.</returns>
        public static string RandomString(this Random random, int maxLength = -1, bool unicode = true,
                                          double nullProbability = 0.0)
        {
            random = random ?? RandomGenerator;
            // Check for random nulls
            if ((nullProbability > 0.0) &&
                (random.NextDouble() < nullProbability))
                return null;

            // Get string length, if there's no maximum then use 8001 (as 8000 is max specific size in SQL Server).
            int length = random.Next(maxLength < 0 ? 8000 : maxLength);
            if (length < 1)
                return String.Empty;

            if (!unicode)
            {
                byte[] bytes = new byte[length];
                random.NextBytes(bytes);
                return new ASCIIEncoding().GetString(bytes);
            }

            StringBuilder stringBuilder = new StringBuilder(length);
            for (int charIndex = 0; charIndex < length; ++charIndex)
                stringBuilder.Append(random.RandomUnicodeCharacter());
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Generates a random Unicode character.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <param name="supplementaryPlaneProbability">The probability of the character coming from a random supplementary plane (0.0 for a Basic Multilingual Plane character).</param>
        /// <returns>A random unicode character.</returns>
        public static char[] RandomUnicodeCharacter(this Random random, double supplementaryPlaneProbability = 0.1)
        {
            random = random ?? RandomGenerator;
            if (supplementaryPlaneProbability > 0.0 && random.NextDouble() < supplementaryPlaneProbability)
                return new char[]
                           {
                               (char)random.Next(0xD800, 0xDBFF),
                               (char)random.Next(0xDC00, 0xDFFF)
                           };
            int character = random.Next(0xF7E1);
            switch (character)
            {
                case 0:
                    character = 0x0009;
                    break;
                case 1:
                    character = 0x000A;
                    break;
                case 2:
                    character = 0x000D;
                    break;
                default:
                    // Other valid characters are 0x0020-0xD7FF and 0xE000-0xFFFD:
                    character += character < 0xD7E3 ? 0x001D : 0x081D;
                    break;
            }
            return new char[] { (char)character };
        }


        /// <summary>
        /// Generates a random <see cref="SqlDbType"/> for a column.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random <see cref="SqlDbType"/>.</returns>
        public static SqlDbType RandomSqlDbTypeForColumn(this Random random)
        {
            return _sqlDbTypes[(random ?? RandomGenerator).Next(_sqlDbTypes.Length)];
        }

        /// <summary>
        /// Generates a random SRID.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <returns>A random SRID.</returns>
        public static int RandomSRID(this Random random)
        {
            return _validSrids[(random ?? RandomGenerator).Next(_validSrids.Length)];
        }

        /// <summary>
        /// Generates the random SQL value.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <param name="sqlDbType">Type of the SQL db.</param>
        /// <param name="length">The length (if fixed length).</param>
        /// <param name="nullProbability">The probability of a column's value being set to SQL null (0.0 for no nulls) [Defaults to 0.0 = 0%].</param>
        /// <param name="fill">if set to <see langword="true" /> expects the column to be full (only appropriate for fixed length columns).</param>
        /// <returns>A random SQL value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks>Does not support SqlDbType.Structured.</remarks>
        public static object RandomSqlValue(this Random random, SqlDbType sqlDbType, int length = -1,
                                            double nullProbability = 0.1, bool fill = false)
        {
            random = random ?? RandomGenerator;

            // Check for random nulls
            if ((nullProbability > 0.0) &&
                (random.NextDouble() < nullProbability))
                return sqlDbType.NullValue();

            // Randomise length if appropriate.
            if (length < 0)
                length = random.Next(4096);
            else if (!fill)
                length = random.Next(length);

            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    return random.RandomInt64();
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.VarBinary:
                    byte[] bytes = new byte[length];
                    random.NextBytes(bytes);
                    return bytes;
                case SqlDbType.Timestamp:
                    byte[] tBytes = new byte[8];
                    random.NextBytes(tBytes);
                    return tBytes;
                case SqlDbType.Bit:
                    return random.RandomBoolean();
                case SqlDbType.DateTime:
                    return random.RandomDateTime(DateTimeKind.Unspecified);
                case SqlDbType.Decimal:
                    return random.RandomDecimal();
                case SqlDbType.Real:
                    return random.RandomFloat();
                case SqlDbType.Float:
                    return random.RandomDouble();
                case SqlDbType.Int:
                    return random.RandomInt32();
                case SqlDbType.Money:
                    return random.RandomDecimal();
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    return random.RandomString(length/2);
                case SqlDbType.UniqueIdentifier:
                    return Guid.NewGuid();
                case SqlDbType.SmallDateTime:
                    // Resolution is to the minute, so we calculate minutes and multiply by ticks per minute.
                    return MinSmallDateTime + TimeSpan.FromMinutes(random.Next(47036160));
                case SqlDbType.SmallInt:
                    return random.RandomInt16();
                case SqlDbType.SmallMoney:
                    return (decimal) random.RandomInt32()/10000;
                case SqlDbType.TinyInt:
                    return random.RandomByte();
                case SqlDbType.Char:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                    return random.RandomString(length, false);
                case SqlDbType.Variant:
                    // Generate an object of random type - but don't allow nulls this time (as we've already had a shot at being null).
                    return random.RandomSqlValue(random.RandomSqlDbTypeForColumn(), length, 0, fill);
                case SqlDbType.Xml:
                    // TODO code technically generate a random document here.
                    return "<TestDocument><Node attribute=\"attributeValue\">Node value</Node></TestDocument>";
                case SqlDbType.Udt:
                    switch (random.Next(3))
                    {
                        case 0:
                            return SqlGeography.Point(-90 + (random.Next(180000)/1000),
                                                      -15069.0 + (random.Next(3013800)/100),
                                                      random.RandomSRID());
                        case 1:
                            return SqlGeometry.Point(-90 + (random.Next(180000)/1000),
                                                     -15069.0 + (random.Next(3013800)/100),
                                                     random.RandomSRID());
                        default:
                            // TODO this does not generate every possible variation, but it's robust.
                            int count = random.Next(1, 100);
                            StringBuilder s = new StringBuilder("/");
                            for (int a = 0; a < count; a++)
                            {
                                s.Append(Math.Abs(RandomInt32(random)));
                                s.Append("/");
                            }
                            return SqlHierarchyId.Parse(s.ToString());
                    }
                case SqlDbType.Date:
                    return random.RandomDateTime().Date;
                case SqlDbType.Time:
                    return random.RandomDateTime().TimeOfDay;
                case SqlDbType.DateTime2:
                    return random.RandomDateTime().Date;
                case SqlDbType.DateTimeOffset:
                    return random.RandomDateTimeOffset();
                default:
                    // NB SqlDbType.Structured is not valid for a column.
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Generates a random record set definition
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <param name="columns">The columns (if less than one, is random).</param>
        /// <returns>A random <see cref="RecordSetDefinition"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [NotNull]
        public static RecordSetDefinition RandomRecordSetDefinition(this Random random, int columns = 0)
        {
            random = random ?? RandomGenerator;
            if (columns < 1)
                columns = random.Next(100) + 1;

            ColumnDefinition[] columnDefinitions = new ColumnDefinition[columns];
            for (int c = 0; c < columns; c++)
            {
                SqlDbType type = random.RandomSqlDbTypeForColumn();
                bool isNullable = random.RandomBoolean();
                object defaultValue = random.RandomSqlValue(type, nullProbability: isNullable ? 0.5 : 0);
                columnDefinitions[c] = new ColumnDefinition("Column " + (c + 1),
                                                            type,
                                                            isNullable: isNullable,
                                                            defaultValue: defaultValue);
            }

            return new RecordSetDefinition(columnDefinitions);
        }

        /// <summary>
        /// Returns a formatted <see cref="string"/> with ' completed in {ms}ms.' appended.
        /// </summary>
        /// <param name="stopwatch">The stopwatch.</param>
        /// <param name="format">The format string.</param>
        /// <param name="parameters">The objects to format in the string.</param>
        /// <returns>
        /// A <see cref="string"/> containing the <paramref name="parameters"/> in the specified <paramref name="format"/>
        /// with ' completed in {ms}ms.' appended. The time duration is taken from the <paramref name="stopwatch"/>.
        /// </returns>
        /// <exception cref="System.FormatException">
        /// <para>The <paramref name="format"/> is invalid</para>
        /// <para>-or-</para>
        /// <para>The index of the format item is less than zero, or greater than or equal to the length or the <paramref name="parameters"/>.</para>
        /// </exception>
        [StringFormatMethod("format")]
        [NotNull]
        public static string ToString([NotNull] this Stopwatch stopwatch, [CanBeNull] string format = null,
                                      [NotNull] params object[] parameters)
        {
            if (String.IsNullOrEmpty(format))
            {
                format = "Stopwatch";
            }
            else if (parameters.Length > 0)
            {
                try
                {
                    format = String.Format(format, parameters);
                }
                catch (FormatException)
                {
                }
            }

            return String.Format("{0} completed in {1}ms.", format, (stopwatch.ElapsedTicks*1000M)/Stopwatch.Frequency);
        }

        /// <summary>
        /// Returns a random element from an enumeration, that matches the predicate; otherwise returns the default value.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <param name="predicate">The optional predicate.</param>
        /// <returns>A random element or default.</returns>
        [CanBeNull]
        public static T RandomOrDefault<T>([NotNull] this IEnumerable<T> enumeration, Func<T, bool> predicate = null)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration", "The enumeration cannot be null.");

            // We may as well build a list, as we have to count elements anyway.
            List<T> filtered = predicate == null ? enumeration.ToList() : enumeration.Where(predicate).ToList();

            int count = filtered.Count;
            return count < 1 ? default(T) : filtered[RandomGenerator.Next(count)];
        }

        /// <summary>
        /// Returns a random element from an enumeration, that matches the predicate; otherwise throws an exception if the predicate is not matched.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <param name="predicate">The optional predicate.</param>
        /// <returns>A random element.</returns>
        [CanBeNull]
        public static T Random<T>([NotNull] this IEnumerable<T> enumeration, Func<T, bool> predicate = null)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration", "The enumeration cannot be null.");

            // We may as well build a list, as we have to count elements anyway.
            List<T> filtered = predicate == null ? enumeration.ToList() : enumeration.Where(predicate).ToList();

            int count = filtered.Count;
            if (count < 1)
                throw new InvalidOperationException("The enumeration did not return any results.");
            return filtered[RandomGenerator.Next(count)];
        }

        public static bool IsNull(this object value)
        {
            if (value == null || DBNull.Value == value)
                return true;
            INullable nullable = value as INullable;
            return nullable != null && nullable.IsNull;
        }

        public static SqlDbType ToSqlDbType(this DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                    return SqlDbType.VarChar;
                case DbType.Binary:
                    return SqlDbType.VarBinary;
                case DbType.Byte:
                    return SqlDbType.TinyInt;
                case DbType.Boolean:
                    return SqlDbType.Bit;
                case DbType.Currency:
                    return SqlDbType.Money;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.Time:
                    return SqlDbType.DateTime;
                case DbType.Decimal:
                    return SqlDbType.Decimal;
                case DbType.Double:
                    return SqlDbType.Float;
                case DbType.Guid:
                    return SqlDbType.UniqueIdentifier;
                case DbType.Int16:
                    return SqlDbType.SmallInt;
                case DbType.Int32:
                    return SqlDbType.Int;
                case DbType.Int64:
                    return SqlDbType.BigInt;
                case DbType.Object:
                    return SqlDbType.Variant;
                case DbType.Single:
                    return SqlDbType.Real;
                case DbType.String:
                    return SqlDbType.NVarChar;
                case DbType.AnsiStringFixedLength:
                    return SqlDbType.Char;
                case DbType.StringFixedLength:
                    return SqlDbType.NChar;
                case DbType.Xml:
                    return SqlDbType.Xml;
                case DbType.DateTime2:
                    return SqlDbType.DateTime2;
                case DbType.DateTimeOffset:
                    return SqlDbType.DateTimeOffset;
                default:
                    // Note SByte, UInt16, UInt32, UInt64 and VarNumeric are all by SQL Server unsupported.
                    throw new ArgumentOutOfRangeException("dbType");
            }
        }

        /// <summary>
        /// Get's the SQL null value for the type.
        /// </summary>
        /// <param name="sqlDbType">Type of the SQL db.</param>
        /// <returns>The null value for the specified SQL type.</returns>
        [NotNull]
        public static object NullValue(this SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    return SqlInt64.Null;
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return SqlBinary.Null;
                case SqlDbType.Bit:
                    return SqlBoolean.Null;
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                    return SqlString.Null;
                case SqlDbType.DateTime:
                    return SqlDateTime.Null;
                case SqlDbType.Decimal:
                    return SqlDecimal.Null;
                case SqlDbType.Float:
                    return SqlDouble.Null;
                case SqlDbType.Int:
                    return SqlInt32.Null;
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return SqlMoney.Null;
                case SqlDbType.Real:
                    return SqlSingle.Null;
                case SqlDbType.UniqueIdentifier:
                    return SqlGuid.Null;
                case SqlDbType.SmallDateTime:
                    return SqlDateTime.Null;
                case SqlDbType.SmallInt:
                    return SqlInt16.Null;
                case SqlDbType.TinyInt:
                    return SqlByte.Null;
                case SqlDbType.Xml:
                    return SqlXml.Null;
                default:
                    return DBNull.Value;
            }
        }

        /// <summary>
        /// The minumum value for SmallDateTime.
        /// </summary>
        public static readonly DateTime MinSmallDateTime = new DateTime(1990, 1, 1);

        /// <summary>
        /// The maximum value for SmallDateTime.
        /// </summary>
        public static readonly DateTime MaxSmallDateTime = new DateTime(2079, 6, 7) - new TimeSpan(1);
    }
}