#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.IO;
#if !BUILD_TASKS
using System.Net;
using WebApplications.Utilities.Configuration;
#endif

namespace WebApplications.Utilities.Globalization
{
    /// <summary>
    /// Provides methods for reading and writing currency information to/from binary and XML files.
    /// </summary>
    [PublicAPI]
    public class CurrencyInfoProvider
        : ICurrencyInfoProvider
    {
#if !BUILD_TASKS
        /// <summary>
        /// A <see cref="ICurrencyInfoProvider"/> with no currencies!
        /// </summary>
        private class EmptyCurrencyInfoProvider : ICurrencyInfoProvider
        {
            private readonly DateTime _published;

            /// <summary>
            /// Initializes a new instance of the <see cref="EmptyCurrencyInfoProvider"/> class.
            /// </summary>
            /// <param name="published">The published.</param>
            public EmptyCurrencyInfoProvider(DateTime published)
            {
                _published = published;
            }

            /// <summary>
            /// The date this provider was published.
            /// </summary>
            public DateTime Published
            {
                get { return _published; }
            }

            /// <summary>
            /// The currencies in the provider.
            /// </summary>
            public IEnumerable<CurrencyInfo> All
            {
                get { return Enumerable.Empty<CurrencyInfo>(); }
            }

            /// <summary>
            /// Gets the number of currencies specified in the provider.
            /// </summary>
            /// <value>
            /// The count.
            /// </value>
            public int Count
            {
                get { return 0; }
            }

            /// <summary>
            /// Retrieves a <see cref="CurrencyInfo" /> with the ISO Code specified.
            /// </summary>
            /// <param name="currencyCode">The ISO Code.</param>
            /// <returns>
            /// The <see cref="CurrencyInfo" /> that corresponds to the <paramref name="currencyCode" /> specified (if any);
            /// otherwise the default value for the type is returned.
            /// </returns>
            public CurrencyInfo Get(string currencyCode)
            {
                return null;
            }

            /// <summary>
            /// Gets the <see cref="CurrencyInfo" /> from the specified region information.
            /// </summary>
            /// <param name="regionInfo">The region information.</param>
            /// <returns>
            /// The associated <see cref="CurrencyInfo" /> if found; otherwise <see langword="null" />.
            /// </returns>
            public CurrencyInfo Get(RegionInfo regionInfo)
            {
                return null;
            }

            /// <summary>
            /// Gets the <see cref="CurrencyInfo" /> from the specified culture information.
            /// </summary>
            /// <param name="cultureInfo">The culture information.</param>
            /// <returns>
            /// The associated <see cref="CurrencyInfo" /> if found; otherwise <see langword="null" />.
            /// </returns>
            public CurrencyInfo Get(CultureInfo cultureInfo)
            {
                return null;
            }

            /// <summary>
            /// Gets the <see cref="CurrencyInfo" /> from the specified extended culture information.
            /// </summary>
            /// <param name="cultureInfo">The culture information.</param>
            /// <returns>
            /// The associated <see cref="CurrencyInfo" /> if found; otherwise <see langword="null" />.
            /// </returns>
            public CurrencyInfo Get(ExtendedCultureInfo cultureInfo)
            {
                return null;
            }
        }

        /// <summary>
        /// The empty currency provider has no currencies.
        /// </summary>
        [NotNull]
        public static readonly ICurrencyInfoProvider Empty;

        /// <summary>
        /// The current provider.
        /// </summary>
        [NotNull]
        private static ICurrencyInfoProvider _current;

        /// <summary>
        /// Whether <see cref="_current"/> was loaded from the config.
        /// </summary>
        private static bool _isFromConfig;

        /// <summary>
        /// Gets or sets the current global <see cref="CurrencyInfoProvider">provider</see>.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        [NotNull]
        public static ICurrencyInfoProvider Current
        {
            get { return _current; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _current = value;
                _isFromConfig = false;
            }
        }

        /// <summary>
        /// Initializes the <see cref="CurrencyInfoProvider"/> class.
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">The ISO 4217 file specified in the configuration file was not found.</exception>
        /// <exception cref="System.IO.InvalidDataException">The data in the ISO 4217 file was invalid.</exception>
        /// <exception cref="System.IO.FileLoadException">An exception was thrown while loading the currency information from the ISO 4217 file.</exception>
        static CurrencyInfoProvider()
        {
            _current = Empty = new EmptyCurrencyInfoProvider(DateTime.MinValue);

            SetCurrentProvider();
        }

        /// <summary>
        /// Called when the utility configuration changes. If the <see cref="Configuration.UtilityConfiguration.ISO4217"/> property changes, the data_stream will be reloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Configuration.UtilityConfiguration.ConfigurationChangedEventArgs"/> instance containing the event data.</param>
        internal static void OnUtilityConfigurationChanged(
            [NotNull] object sender,
            [NotNull] UtilityConfiguration.ConfigurationChangedEventArgs e)
        {
            if (_isFromConfig &&
                !string.Equals(
                    e.NewConfiguration.ISO4217,
                    e.OldConfiguration.ISO4217,
                    StringComparison.InvariantCulture))
                SetCurrentProvider();
        }

        /// <summary>
        /// Sets the current provider.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void SetCurrentProvider(string path = null)
        {
            _current = LoadCurrencies(path);
            _isFromConfig = path == null;
        }

        /// <summary>
        /// Loads the currency info provider from the path given.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException">The ISO 4217 file specified was not found.</exception>
        /// <exception cref="System.IO.FileLoadException">An exception was thrown while loading the currency information from the ISO 4217 file.</exception>
        /// <exception cref="System.IO.InvalidDataException">The data in the ISO 4217 file was invalid.</exception>
        [NotNull]
        public static ICurrencyInfoProvider LoadCurrencies(string path = null)
        {
            path = path ?? UtilityConfiguration.Active.ISO4217;
            if (string.IsNullOrWhiteSpace(path))
                return Empty;

            Uri uri = new Uri(path.TrimStart('\\', '/'), UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(UtilityExtensions.AppDomainBaseUri, uri);

            ICurrencyInfoProvider provider;

            if (uri.IsFile)
            {
                path = uri.LocalPath;
                if (!File.Exists(path))
                    throw new FileNotFoundException(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        string.Format(Resources.CurrencyInfoProvider_CurrencyInfoProvider_FileNotFound, path));

                try
                {
                    provider = LoadFromFile(path);
                }
                catch (Exception e)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    throw new FileLoadException(
                        string.Format(Resources.CurrencyInfoProvider_CurrencyInfoProvider_LoadError, path),
                        e);
                }
            }
            else
            {
                path = uri.AbsoluteUri;
                try
                {
                    WebRequest request = WebRequest.Create(uri);
                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        Debug.Assert(stream != null);
                        provider = Load(stream);
                    }
                }
                catch (Exception e)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    throw new FileLoadException(
                        string.Format(Resources.CurrencyInfoProvider_CurrencyInfoProvider_LoadError, path),
                        e);
                }
            }

            if (provider == null)
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new InvalidDataException(
                    string.Format(Resources.CurrencyInfoProvider_CurrencyInfoProvider_DataInvalid, path));

            return provider;
        }
#endif

        /// <summary>
        /// The first four bytes expected in binary currency info files.
        /// Equivalent to the string '$CCY'.
        /// </summary>
        public const int BinaryHeader = 0x59434324;

        private readonly DateTime _published;

        /// <summary>
        /// Stores currency info (by code).
        /// </summary>
        [NotNull]
        private readonly IReadOnlyDictionary<string, CurrencyInfo> _currencyInfos;

        /// <summary>
        /// The date this file was published.
        /// </summary>
        public DateTime Published
        {
            get { return _published; }
        }

        /// <summary>
        /// The currencies in the file.
        /// </summary>
        [ItemNotNull]
        public IEnumerable<CurrencyInfo> All
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return _currencyInfos.Values; }
        }

        /// <summary>
        /// Gets the number of currencies specified in the provider.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get { return _currencyInfos.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyInfoProvider" /> class.
        /// </summary>
        /// <param name="published">The date this file was published.</param>
        /// <param name="currencies">The currencies in the file.</param>
        public CurrencyInfoProvider(DateTime published, [ItemNotNull] [NotNull] IEnumerable<CurrencyInfo> currencies)
        {
            if (currencies == null) throw new ArgumentNullException("currencies");
            _published = published;
            // ReSharper disable once PossibleNullReferenceException
            _currencyInfos = currencies.Distinct().ToDictionary(c => c.Code, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Loads the provider from a file. If the path ends in the extension .xml the provider will be loaded from XML; 
        /// otherwise the contents of the file will be used to determine the format.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns></returns>
        [CanBeNull]
        public static CurrencyInfoProvider LoadFromFile([NotNull] string path)
        {
            if (string.Equals(Path.GetExtension(path), ".xml", StringComparison.InvariantCultureIgnoreCase))
                return LoadFromXml(File.ReadAllText(path));

            using (FileStream file = File.OpenRead(path))
                return Load(file);
        }

        /// <summary>
        /// Loads the currencies from the given stream, determining the format of the data based on the contents of the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="leaveOpen">if set to <see langword="true" /> the <paramref name="stream"/> will be left open.</param>
        /// <returns></returns>
        [CanBeNull]
        public static CurrencyInfoProvider Load([NotNull] Stream stream, bool leaveOpen = false)
        {
            PeekableStream peekable = stream as PeekableStream ?? new PeekableStream(stream);

            if (peekable.PeekByte() == '$')
                return LoadFromBinary(peekable, leaveOpen);

            using (StreamReader reader = new StreamReader(peekable, Encoding.UTF8, false, 1024, leaveOpen))
                return LoadFromXml(reader);
        }

        /// <summary>
        /// Loads the currencies from XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        [CanBeNull]
        public static CurrencyInfoProvider LoadFromXml([NotNull] string xml)
        {
            if (xml == null) throw new ArgumentNullException("xml");

            using (StringReader reader = new StringReader(xml))
                return LoadFromXml(reader);
        }

        /// <summary>
        /// Loads the currencies from XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        [CanBeNull]
        public static CurrencyInfoProvider LoadFromXml([NotNull] TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            string xml = reader.ReadToEnd();
            XDocument doc = XDocument.Parse(xml); //.Load(reader);

            DateTime published;

            // ReSharper disable once PossibleNullReferenceException
            XAttribute pubAttr = doc.Root.Attribute("Pblshd");
            if (pubAttr == null ||
                !DateTime.TryParse(
                    pubAttr.Value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out published))
                published = DateTime.UtcNow;

            XElement ccyTbl = doc.Root.Element("CcyTbl");
            if (ccyTbl == null)
                return null;

            Dictionary<string, CurrencyInfo> currencies = new Dictionary<string, CurrencyInfo>();

            foreach (XElement entry in ccyTbl.Elements("CcyNtry"))
            {
                XElement name = entry.Element("CcyNm");
                XElement code = entry.Element("Ccy");
                XElement number = entry.Element("CcyNbr");
                XElement exponent = entry.Element("CcyMnrUnts");
                XAttribute isLatest = entry.Attribute("IsLatest");

                int num;

                if (code == null ||
                    name == null ||
                    number == null ||
                    !int.TryParse(number.Value, out num))
                    continue;

                // Ignore funds
                XAttribute isFundAttr = name.Attribute("IsFund");
                if (isFundAttr != null &&
                    string.Equals(isFundAttr.Value, "true", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                int exp;
                int? nexp = exponent != null &&
                            int.TryParse(exponent.Value, out exp)
                    ? (int?)exp
                    : null;

                bool latest = isLatest == null ||
                              string.Equals(isLatest.Value, "true", StringComparison.InvariantCultureIgnoreCase);

                CurrencyInfo existing;
                if (currencies.TryGetValue(code.Value, out existing))
                {
                    Debug.Assert(existing != null);

                    if (latest && !existing.IsLatest)
                    {
                        currencies[code.Value] = new CurrencyInfo(code.Value, num, nexp, name.Value, latest);
                        continue;
                    }

                    if (existing.ISONumber == num &&
                        existing.Exponent == nexp &&
                        existing.FullName == name.Value)
                        continue;

                    throw new InvalidDataException("Multiple currencies with the same code but different properties.");
                }

                currencies.Add(
                    code.Value,
                    new CurrencyInfo(code.Value, num, nexp, name.Value, latest));
            }

            return new CurrencyInfoProvider(
                published,
                currencies.Values);
        }

        /// <summary>
        /// Loads the currencies from binary.
        /// </summary>
        /// <param name="stream">The binary stream to load from.</param>
        /// <param name="leaveOpen">if set to <see langword="true" /> the <paramref name="stream"/> will be left open.</param>
        /// <returns></returns>
        [NotNull]
        public static CurrencyInfoProvider LoadFromBinary([NotNull] Stream stream, bool leaveOpen = false)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen))
            {
                // Read the header info.
                if (reader.ReadInt32() != BinaryHeader)
                    throw new InvalidDataException("The currency info file was an invalid format.");

                DateTime published = DateTime.SpecifyKind(DateTime.FromBinary(reader.ReadInt64()), DateTimeKind.Utc);
                int count = reader.ReadInt32();

                List<CurrencyInfo> currencies = new List<CurrencyInfo>(count);
                while (stream.Position < stream.Length)
                {
                    // The count acts as a form of check on file validity
                    if (--count < 0) throw new InvalidDataException("The currency info file was an invalid format.");
                    int number = reader.ReadInt32();
                    string code = reader.ReadString();
                    string name = reader.ReadString();
                    bool hasExp = reader.ReadBoolean();
                    int? exponent = hasExp ? reader.ReadInt32() : (int?)null;
                    bool isLatest = reader.ReadBoolean();

                    currencies.Add(new CurrencyInfo(code, number, exponent, name, isLatest));
                }
                if (count > 0)
                    throw new InvalidDataException("The currency info file was an invalid format.");

                return new CurrencyInfoProvider(published, currencies);
            }
        }

        /// <summary>
        ///   Retrieves a <see cref="CurrencyInfo"/> with the ISO Code specified.
        /// </summary>
        /// <param name="currencyCode">The ISO Code.</param>
        /// <returns>
        ///   The <see cref="CurrencyInfo"/> that corresponds to the <paramref name="currencyCode"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="currencyCode"/> is <see langword="null"/>.
        /// </exception>
        public CurrencyInfo Get(string currencyCode)
        {
            if (currencyCode == null) throw new ArgumentNullException("currencyCode");
            CurrencyInfo currencyInfo;
            _currencyInfos.TryGetValue(currencyCode, out currencyInfo);
            return currencyInfo;
        }

        /// <summary>
        /// Gets the <see cref="CurrencyInfo" /> from the specified region information.
        /// </summary>
        /// <param name="regionInfo">The region information.</param>
        /// <returns>
        /// The associated <see cref="CurrencyInfo" /> if found; otherwise <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="regionInfo"/> is <see langword="null"/>.
        /// </exception>
        public CurrencyInfo Get(RegionInfo regionInfo)
        {
            if (regionInfo == null) throw new ArgumentNullException("regionInfo");
            CurrencyInfo currencyInfo;
            _currencyInfos.TryGetValue(regionInfo.ISOCurrencySymbol, out currencyInfo);
            return currencyInfo;
        }

        /// <summary>
        /// Gets the <see cref="CurrencyInfo" /> from the specified culture information.
        /// </summary>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>
        /// The associated <see cref="CurrencyInfo" /> if found; otherwise <see langword="null" />.
        /// </returns>
        public CurrencyInfo Get(CultureInfo cultureInfo)
        {
            ExtendedCultureInfo eci = cultureInfo as ExtendedCultureInfo;
            CurrencyInfo currencyInfo;
            if (!ReferenceEquals(eci, null))
                return eci.RegionInfo != null &&
                       _currencyInfos.TryGetValue(eci.RegionInfo.ISOCurrencySymbol, out currencyInfo)
                    ? currencyInfo
                    : null;

            eci = CultureInfoProvider.Current.Get(cultureInfo);
            if (ReferenceEquals(eci, null)) return null;

            return eci.RegionInfo != null &&
                   _currencyInfos.TryGetValue(eci.RegionInfo.ISOCurrencySymbol, out currencyInfo)
                ? currencyInfo
                : null;
        }

        /// <summary>
        /// Gets the specified culture information.
        /// </summary>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns></returns>
        public CurrencyInfo Get(ExtendedCultureInfo cultureInfo)
        {
            CurrencyInfo currencyInfo;
            return cultureInfo.RegionInfo != null &&
                   _currencyInfos.TryGetValue(cultureInfo.RegionInfo.ISOCurrencySymbol, out currencyInfo)
                ? currencyInfo
                : null;
        }
    }
}