#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Stores a semantic version number for a program.
    /// </summary>
    [Serializable]
    public sealed class SemanticVersion : IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>,
        ISerializable
    {
        /// <summary>
        /// The characters that are valid in the pre-release and build part strings
        /// </summary>
        [NotNull]
        private static readonly HashSet<char> _validChars =
            new HashSet<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-.");

        /// <summary>
        /// The dot delimiter for splitting pre-release parts
        /// </summary>
        private static readonly char[] _dotDelimiter = {'.'};

        /// <summary>
        /// Version zero
        /// </summary>
        [NotNull]
        public static readonly SemanticVersion Zero = new SemanticVersion(0, 0, 0, null, null, false);

        /// <summary>
        /// Wildcard version
        /// </summary>
        [NotNull]
        public static readonly SemanticVersion Any = new SemanticVersion(
            Optional<int>.Unassigned,
            Optional<int>.Unassigned,
            Optional<int>.Unassigned,
            Optional<string>.Unassigned,
            Optional<string>.Unassigned,
            true);

        [NotNull]
        private readonly Lazy<string> _string;

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion" /> class.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch version number.</param>
        /// <param name="preRelease">The optional pre-release version.</param>
        /// <param name="build">The optional build version.</param>
        /// <param name="isPartial">if set to <see langword="true" /> [is partial].</param>
        private SemanticVersion(
            Optional<int> major,
            Optional<int> minor,
            Optional<int> patch,
            Optional<string> preRelease,
            Optional<string> build,
            bool isPartial)
        {
            Contract.Requires(major.Value >= 0);
            Contract.Requires(minor.Value >= 0);
            Contract.Requires(patch.Value >= 0);

            Major = major;
            Minor = minor;
            Patch = patch;

            if (preRelease.IsAssigned &&
                preRelease.Value == null) preRelease = string.Empty;
            if (build.IsAssigned &&
                build.Value == null) build = string.Empty;

            PreRelease = preRelease;
            Build = build;

            IsPartial = isPartial;

            /* 
             * Create method to build the string representation on demand.
             */
            _string = new Lazy<string>(
                () =>
                {
                    Contract.Assert(!preRelease.IsAssigned || preRelease.Value != null);
                    Contract.Assert(!build.IsAssigned || build.Value != null);

                    if (!isPartial)
                        return string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}.{1}.{2}{3}{4}",
                            major.Value.ToString("D"),
                            minor.Value.ToString("D"),
                            patch.Value.ToString("D"),
                            preRelease.Value.Length > 0 ? "-" + preRelease.Value : string.Empty,
                            build.Value.Length > 0 ? "+" + build.Value : string.Empty);

                    StringBuilder s = new StringBuilder();
                    if (major.IsAssigned)
                        s.Append(major.Value.ToString("D"));
                    else
                    {
                        if (!minor.IsAssigned &&
                            !patch.IsAssigned &&
                            !preRelease.IsAssigned &&
                            !build.IsAssigned)
                            return "*";
                        s.Append('*');
                    }

                    if (minor.IsAssigned)
                        s.Append('.').Append(minor.Value.ToString("D"));
                    else
                    {
                        s.Append(".*");
                        if (!patch.IsAssigned &&
                            !preRelease.IsAssigned &&
                            !build.IsAssigned)
                            return s.ToString();
                    }

                    if (patch.IsAssigned)
                        s.Append('.').Append(patch.Value.ToString("D"));
                    else
                    {
                        s.Append(".*");
                        if (!preRelease.IsAssigned &&
                            !build.IsAssigned)
                            return s.ToString();
                    }

                    if (preRelease.IsAssigned)
                    {
                        s.Append('-');
                        if (preRelease.Value.Length > 0)
                            s.Append(preRelease.Value);
                    }
                    else
                    {
                        s.Append("-*");
                        if (!build.IsAssigned)
                            return s.ToString();
                    }

                    s.Append('+');
                    if (build.IsAssigned)
                    {
                        if (build.Value.Length > 0)
                            s.Append(build.Value);
                    }
                    else
                        s.Append('*');

                    return s.ToString();
                },
                LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion" /> class.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch version number.</param>
        /// <param name="preRelease">The pre-release version.</param>
        /// <param name="build">The build version.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// major;The major version number can not be negative.
        /// or
        /// minor;The minor version number can not be negative.
        /// or
        /// patch;The patch version number can not be negative.
        /// or
        /// preRelease;The pre-release string contains invalid characters.
        /// </exception>
        public SemanticVersion(
            Optional<int> major = default(Optional<int>),
            Optional<int> minor = default(Optional<int>),
            Optional<int> patch = default(Optional<int>),
            Optional<string> preRelease = default(Optional<string>),
            Optional<string> build = default(Optional<string>))
            : this(
                major,
                minor,
                patch,
                preRelease,
                build,
                !build.IsAssigned ||
                !preRelease.IsAssigned ||
                !patch.IsAssigned ||
                !minor.IsAssigned ||
                !major.IsAssigned)
        {
            if (major.Value < 0)
                throw new ArgumentOutOfRangeException("major", major, "The major version number can not be negative.");
            if (minor.Value < 0)
                throw new ArgumentOutOfRangeException("minor", minor, "The minor version number can not be negative.");
            if (patch.Value < 0)
                throw new ArgumentOutOfRangeException("patch", patch, "The patch version number can not be negative.");
            Contract.EndContractBlock();
            if ((preRelease.Value != null) &&
                (!IsValidPart(preRelease.Value)))
                throw new ArgumentOutOfRangeException(
                    "preRelease",
                    preRelease,
                    "The pre-release string contains invalid characters.");
            if ((build != null) &&
                (!IsValidPart(build.Value)))
                throw new ArgumentOutOfRangeException("build", build, "The build string contains invalid characters");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion" /> class from a <see cref="System.Version" />.
        /// A version of 1.2.3.4 will become <c>1.2.3[-preRelease]+build.4</c>, where <c>[-preRelease]</c> will be the
        /// <paramref cref="preRelease"/> if it was specified and not null.
        /// </summary>
        /// <param name="version">The system version.</param>
        /// <param name="preRelease">The optional preRelease version.</param>
        public SemanticVersion([NotNull] Version version, [CanBeNull] string preRelease = "")
            : this(
                version.Major,
                version.Minor,
                version.Build > -1 ? version.Build : 0,
                preRelease,
                version.Revision > -1
                    ? string.Format("build.{0}", version.Revision)
                    : null)
        {
            Contract.Requires(version != null);
        }

        /// <summary>
        /// Deserializes a <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private SemanticVersion([NotNull] SerializationInfo info, StreamingContext context)
            : this(
                info.GetInt32("Major") < 0 ? Optional<int>.Unassigned : info.GetInt32("Major"),
                info.GetInt32("Minor") < 0 ? Optional<int>.Unassigned : info.GetInt32("Minor"),
                info.GetInt32("Patch") < 0 ? Optional<int>.Unassigned : info.GetInt32("Patch"),
                info.GetString("PreRelease") ?? Optional<string>.Unassigned,
                info.GetString("Build") ?? Optional<string>.Unassigned,
                info.GetBoolean("IsPartial"))
        {
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Major", Major.IsAssigned ? Major.Value : -1);
            info.AddValue("Minor", Minor.IsAssigned ? Minor.Value : -1);
            info.AddValue("Patch", Patch.IsAssigned ? Patch.Value : -1);
            info.AddValue("PreRelease", PreRelease.IsAssigned ? PreRelease.Value : null);
            info.AddValue("Build", Build.IsAssigned ? Build.Value : null);
            info.AddValue("IsPartial", IsPartial);
        }

        /// <summary>
        /// Determines whether a pre-release string is valid.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns><see langword="true" /> if the pre-release string is valid; otherwise, <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidPart([CanBeNull] string part)
        {
            return part == null || part.All(c => _validChars.Contains(c));
        }

        /// <summary>
        /// Gets the major version number.
        /// </summary>
        /// <value>
        /// The value of this property is a non-negative integer for the major
        /// version number.
        /// </value>
        public readonly Optional<int> Major;

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        /// <value>
        /// The value of this property is a non-negative integer for the minor
        /// version number.
        /// </value>
        public readonly Optional<int> Minor;

        /// <summary>
        /// Gets the patch version number.
        /// </summary>
        /// <value>
        /// The value of this property is a non-negative integer for the patch
        /// version number.
        /// </value>
        public readonly Optional<int> Patch;

        /// <summary>
        /// Gets the pre-release version component.
        /// </summary>
        /// <value>
        /// The value of this property is a string containing the pre-release
        /// identifier.
        /// </value>
        public readonly Optional<string> PreRelease;

        /// <summary>
        /// Gets the build number.
        /// </summary>
        /// <value>
        /// The value of this property is a string containing the build
        /// identifier for the version number.
        /// </value>
        public readonly Optional<string> Build;

        /// <summary>
        /// Gets a value indicating whether some parts of the version are unassigned.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if some parts of the version are unassigned; otherwise, <see langword="false" />.
        /// </value>
        public readonly bool IsPartial;

        /// <summary>
        /// Gets a full semantic version from this version. If this version is not partial,
        /// this version will be returned. Otherwise, any unassigned parts will have their default
        /// value (0 and null).
        /// </summary>
        /// <returns><c>this</c> if this version <see cref="IsPartial">is partial</see>. Otherwise,
        /// a new version with no unspecified parts.</returns>
        [NotNull]
        public SemanticVersion GetFull()
        {
            return IsPartial
                ? new SemanticVersion(
                    Major.Value,
                    Minor.Value,
                    Patch.Value,
                    PreRelease.Value ?? string.Empty,
                    Build.Value ?? string.Empty,
                    false)
                : this;
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> objects for equality.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second semantic version object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if the objects are equal, or <b>false</b> if the
        /// objects are not equal.
        /// </returns>
        public static bool operator ==([CanBeNull] SemanticVersion version, [CanBeNull] SemanticVersion other)
        {
            return ReferenceEquals(null, version)
                ? ReferenceEquals(null, other)
                : version.Equals(other);
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> objects for equality.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if the objects are not equal, or <b>false</b> if the
        /// objects are equal.
        /// </returns>
        public static bool operator !=([CanBeNull] SemanticVersion version, [CanBeNull] SemanticVersion other)
        {
            return ReferenceEquals(null, version)
                ? !ReferenceEquals(null, other)
                : !version.Equals(other);
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> objects to determine if
        /// the first object logically precedes the second object.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if <paramref name="version"/> precedes 
        /// <paramref name="other"/>, otherwise <b>false</b>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "MFC3: The version argument is being validated using code contracts.")]
        public static bool operator <([CanBeNull] SemanticVersion version, [CanBeNull] SemanticVersion other)
        {
            if (version == null)
                return other == null;
            if (other == null)
                return false;

            return 0 > version.CompareTo(other);
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> object to determine if
        /// the first object logically follows the second object.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if <paramref name="version"/> follows
        /// <paramref name="other"/>, otherwise <b>false</b>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "MFC3: The version argument is being validated using code contracts.")]
        public static bool operator >([CanBeNull] SemanticVersion version, [CanBeNull] SemanticVersion other)
        {
            if (version == null)
                return other != null;
            if (other == null)
                return true;

            return 0 < version.CompareTo(other);
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> objects to determine if
        /// the first object logically precedes or is equal to the second object.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if <paramref name="version"/> if equal to or precedes 
        /// <paramref name="other"/>, otherwise <b>false</b>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "MFC3: The version argument is being validated using code contracts.")]
        public static bool operator <=([CanBeNull] SemanticVersion version, [CanBeNull] SemanticVersion other)
        {
            if (version == null)
                return other == null;
            if (other == null)
                return false;

            return 0 > version.CompareTo(other);
        }

        /// <summary>
        /// Compares two <see cref="SemanticVersion"/> object to determine if
        /// the first object logically follows or is equal to the second object.
        /// </summary>
        /// <param name="version">
        /// The first <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <param name="other">
        /// The second <see cref="SemanticVersion"/> object to compare.
        /// </param>
        /// <returns>
        /// <b>True</b> if <paramref name="version"/> if equal to or follows
        /// <paramref name="other"/>, otherwise <b>false</b>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "MFC3: The version argument is being validated using code contracts.")]
        public static bool operator >=([CanBeNull] SemanticVersion version, [CanBeNull] SemanticVersion other)
        {
            if (version == null)
                return other != null;
            if (other == null)
                return true;

            return 0 < version.CompareTo(other);
        }

        /// <summary>
        /// Casts a <see cref="SemanticVersion"/> to a <see cref="System.Version"/>.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        [NotNull]
        public static explicit operator Version([NotNull] SemanticVersion version)
        {
            Contract.Requires(version != null);

            if (string.IsNullOrEmpty(version.Build.Value))
                return new Version(version.Major.Value, version.Minor.Value, version.Patch.Value);

            string buildString = new string(version.Build.Value.Where(char.IsDigit).ToArray());

            return buildString.Length > 0
                ? new Version(version.Major.Value, version.Minor.Value, version.Patch.Value, int.Parse(buildString))
                : new Version(version.Major.Value, version.Minor.Value, version.Patch.Value);
        }

        /// <summary>
        /// Casts a <see cref="System.Version"/> to a <see cref="SemanticVersion"/>.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        [NotNull]
        public static implicit operator SemanticVersion([NotNull] Version version)
        {
            Contract.Requires(version != null);
            return new SemanticVersion(version);
        }

        /// <summary>
        /// Compares two objects.
        /// </summary>
        /// <param name="obj">
        /// The object to compare to this object.
        /// </param>
        /// <returns>
        /// Returns a value that indicates the relative order of the objects
        /// that are being compared.
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description>
        /// This instance precedes <paramref name="obj"/> in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description>
        /// This instance occurs in the same position in the sort order as
        /// <paramref name="obj"/>.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description>
        /// This instance follows <paramref name="obj"/> in the sort order.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is not a <see cref="SemanticVersion"/>
        /// object.
        /// </exception>
        public int CompareTo([CanBeNull] object obj)
        {
            SemanticVersion otherVersion = obj as SemanticVersion;
            return null != otherVersion ? CompareTo(otherVersion) : int.MaxValue;
        }

        /// <summary>
        /// Compares the current object with another 
        /// <see cref="SemanticVersion"/> object.
        /// </summary>
        /// <param name="other">
        /// The other <see cref="SemanticVersion"/> object to compare to this
        /// instance.
        /// </param>
        /// <returns>
        /// Returns a value that indicates the relative order of the objects
        /// that are being compared.
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description>
        /// This instance precedes <paramref name="other"/> in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description>
        /// This instance occurs in the same position in the sort order as
        /// <paramref name="other"/>.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description>
        /// This instance follows <paramref name="other"/> in the sort order.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo([NotNull] SemanticVersion other)
        {
            Contract.Assert(other != null);

            if (ReferenceEquals(this, other))
                return 0;

            int result = Major.CompareTo(other.Major);
            if (result != 0)
                return result;

            result = Minor.CompareTo(other.Minor);
            if (result != 0)
                return result;

            result = Patch.CompareTo(other.Patch);
            return result != 0
                ? result
                : ComparePrereleaseVersions(PreRelease, other.PreRelease);
        }

        /// <summary>
        /// Gets if the <paramref name="other"/> version matches this version. Any unassigned parts
        /// on either side are considered equal.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Matches([NotNull] SemanticVersion other)
        {
            Contract.Requires(other != null);

            if (ReferenceEquals(this, other))
                return true;

            if (Major.IsAssigned &&
                other.Major.IsAssigned &&
                (Major.Value != other.Major.Value))
                return false;

            if (Minor.IsAssigned &&
                other.Minor.IsAssigned &&
                (Minor.Value != other.Minor.Value))
                return false;

            if (Patch.IsAssigned &&
                other.Patch.IsAssigned &&
                (Patch.Value != other.Patch.Value))
                return false;

            if ((PreRelease.IsAssigned && other.PreRelease.IsAssigned && (PreRelease.Value != other.PreRelease.Value)))
                return false;

            return !Build.IsAssigned || !other.Build.IsAssigned || (Build.Value == other.Build.Value);
        }

        /// <summary>
        /// Compares this instance to another object for equality.
        /// </summary>
        /// <param name="obj">
        /// The object to compare to this instance.
        /// </param>
        /// <returns>
        /// <b>True</b> if the objects are equal, or <b>false</b> if the
        /// objects are not equal.
        /// </returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            SemanticVersion other = obj as SemanticVersion;
            return null != other && Equals(other);
        }

        /// <summary>
        /// Compares this instance to another <see cref="SemanticVersion"/>
        /// object for equality.
        /// </summary>
        /// <param name="other">
        /// The <see cref="SemanticVersion"/> object to compare to this
        /// instance.
        /// </param>
        /// <returns>
        /// <b>True</b> if the objects are equal, or false if the objects are
        /// not equal.
        /// </returns>
        public bool Equals([CanBeNull] SemanticVersion other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (ReferenceEquals(other, null))
                return false;

            return Major == other.Major
                   && Minor == other.Minor
                   && Patch == other.Patch
                   && PreRelease == other.PreRelease;
        }

        /// <summary>
        /// Calculates the hash code for the object.
        /// </summary>
        /// <returns>
        /// The hash code for the object.
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = 17;
            hashCode = (hashCode * 37) + Major.GetHashCode();
            hashCode = (hashCode * 37) + Minor.GetHashCode();
            hashCode = (hashCode * 37) + Patch.GetHashCode();
            hashCode = (hashCode * 37) + PreRelease.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Returns the string representation of the semantic version number.
        /// </summary>
        /// <returns>
        /// The semantic version number.
        /// </returns>
        public override string ToString()
        {
            return _string.Value;
        }

        /// <summary>
        /// Compares two pre-release version values to determine precedence.
        /// </summary>
        /// <param name="identifier1">
        /// The first identifier to compare.
        /// </param>
        /// <param name="identifier2">
        /// The second identifier to compare.
        /// </param>
        /// <returns>
        /// Returns a value that indicates the relative order of the objects
        /// that are being compared.
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>Less than zero</term>
        /// <description>
        /// <paramref name="identifier1"/> precedes 
        /// <paramref name="identifier2"/> in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description>
        /// The identifiers occur in the same position in the sort order.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Greater than zero</term>
        /// <description>
        /// <paramref name="identifier1"/> follows 
        /// <paramref name="identifier2"/> in the sort order.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        private static int ComparePrereleaseVersions(Optional<string> identifier1, Optional<string> identifier2)
        {
            if (!identifier1.IsAssigned)
                return identifier2.IsAssigned ? -1 : 0;
            if (!identifier2.IsAssigned)
                return 1;

            Contract.Assert(identifier1.Value != null);
            Contract.Assert(identifier2.Value != null);

            if (identifier1.Value.Length < 1)
                return identifier2.Value.Length < 1 ? 1 : 0;
            if (identifier2.Value.Length < 1)
                return -1;

            int result = 0;

            string[] parts1 = identifier1.Value.Split(_dotDelimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] parts2 = identifier2.Value.Split(_dotDelimiter, StringSplitOptions.RemoveEmptyEntries);
            int max = Math.Max(parts1.Length, parts2.Length);

            for (int i = 0; i < max; i++)
            {
                if (i == parts1.Length &&
                    i != parts2.Length)
                {
                    result = -1;
                    break;
                }

                if (i != parts1.Length &&
                    i == parts2.Length)
                {
                    result = 1;
                    break;
                }

                string part1 = parts1[i];
                string part2 = parts2[i];

                Contract.Assert(part1 != null);
                Contract.Assert(part2 != null);

                int value1, value2;
                if (int.TryParse(part1, NumberStyles.None, CultureInfo.InvariantCulture, out value1) &&
                    int.TryParse(part2, NumberStyles.None, CultureInfo.InvariantCulture, out value2))
                    result = value1.CompareTo(value2);
                else
                    result = string.Compare(part1, part2, StringComparison.Ordinal);

                if (0 != result)
                    break;
            }

            return result;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SemanticVersion" /> class.
        /// </summary>
        /// <param name="version">The semantic version number to be parsed.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">version;The specified semantic version string was not valid.</exception>
        [CanBeNull]
        public static SemanticVersion Parse([CanBeNull] string version)
        {
            if (version == null) return null;
            SemanticVersion semanticVersion;
            if (!TryParse(version, out semanticVersion))
                throw new ArgumentOutOfRangeException(
                    "version",
                    version,
                    "The specified semantic version string was not valid.");
            return semanticVersion;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SemanticVersion" /> class.
        /// </summary>
        /// <param name="version">The semantic version number to be parsed.</param>
        /// <param name="semanticVersion">The semantic version.</param>
        /// <returns><see langword="true" /> if the version was parsed successfully, <see langword="false" /> otherwise.</returns>
        public static bool TryParse([CanBeNull] string version, [CanBeNull] out SemanticVersion semanticVersion)
        {
            semanticVersion = null;
            if (string.IsNullOrEmpty(version)) return false;

            Optional<int> major = Optional<int>.Unassigned;
            Optional<int> minor = Optional<int>.Unassigned;
            Optional<int> patch = Optional<int>.Unassigned;
            Optional<string> preRelease = Optional<string>.Unassigned;
            Optional<string> build = Optional<string>.Unassigned;
            bool isPartial = false;

            StringBuilder builder = new StringBuilder(10);
            int p = 0;
            int np = -1;

            int index = 0;
            while (index < version.Length)
            {
                char c = version[index++];
                if (c == '*')
                {
                    if (builder.Length > 0)
                        return false;
                    isPartial = true;
                    builder.Append(c);
                }
                else
                    switch (p)
                    {
                            // Major
                        case 0:
                            // Minor
                        case 1:
                            // Patch
                        case 2:
                            // The first three parts expect integers.
                            switch (c)
                            {
                                case '.':
                                    if (p > 1)
                                        return false;
                                    np = p + 1;
                                    break;
                                case '-':
                                    if (p != 2)
                                        return false;
                                    np = 3;
                                    break;
                                case '+':
                                    if (p != 2)
                                        return false;
                                    np = 4;
                                    break;
                                default:
                                    // We are only allowed +ve integers, so the builder can have a maximum of 10 characters, and must contain digits.
                                    if ((builder.Length > 9) ||
                                        !Char.IsDigit(c))
                                        return false;
                                    builder.Append(c);
                                    break;
                            }
                            break;
                        case 3:
                            // The pre-release part.
                            switch (c)
                            {
                                case '+':
                                    np = 4;
                                    break;
                                default:
                                    if (!_validChars.Contains(c))
                                        return false;
                                    builder.Append(c);
                                    break;
                            }
                            break;
                        case 4:
                            // The build part.
                            if (!_validChars.Contains(c))
                                return false;

                            builder.Append(c);
                            break;
                        default:
                            Contract.Assert(false);
                            break;
                    }

                bool end = index >= version.Length;
                if (!end &&
                    (np < 0)) continue;

                // We can't finish with a '.' part seperator.
                if (end &&
                    (np < 3) &&
                    (np > 0))
                    return false;

                string part;
                if (builder.Length < 1)
                {
                    if (p < 3) return false;
                    part = null;
                }
                else
                {
                    part = builder.ToString();
                    builder.Clear();
                }

                if (isPartial &&
                    (part != null) &&
                    (part[0] == '*'))
                {
                    // We only allow '*' on it's own.
                    if (part.Length > 1)
                        return false;
                }
                else
                {
                    int ip;
                    switch (p)
                    {
                        case 0:
                            if (!int.TryParse(part, out ip))
                                return false;
                            major = new Optional<int>(ip);
                            break;
                        case 1:
                            if (!int.TryParse(part, out ip))
                                return false;
                            minor = new Optional<int>(ip);
                            break;
                        case 2:
                            if (!int.TryParse(part, out ip))
                                return false;
                            patch = new Optional<int>(ip);
                            break;
                        case 3:
                            preRelease = new Optional<string>(part);
                            break;
                        case 4:
                            build = new Optional<string>(part);
                            break;
                        default:
                            Contract.Assert(false);
                            break;
                    }
                }

                if (np < 0) continue;
                p = np;
                np = -1;

                if (!end) continue;

                // Deal with the case we end with a '-' or '+' to indicate null.
                switch (p)
                {
                    case 3:
                        preRelease = Optional<string>.DefaultAssigned;
                        break;
                    case 4:
                        build = Optional<string>.DefaultAssigned;
                        break;
                    default:
                        Contract.Assert(false);
                        break;
                }
            }

            if (isPartial)
            {
                if (!major.IsAssigned &&
                    !minor.IsAssigned &&
                    !patch.IsAssigned &&
                    !preRelease.IsAssigned &&
                    !build.IsAssigned)
                {
                    semanticVersion = Any;
                    return true;
                }
            }
            else
            {
                // Must specify at least major,minor and patch for full semantic versions.
                if (p < 2)
                    return false;

                // For non-partial semantic versions, a missing pre-release or build is considered null not unassigned.
                if (!preRelease.IsAssigned)
                    preRelease = Optional<string>.DefaultAssigned;
                if (!build.IsAssigned)
                    build = Optional<string>.DefaultAssigned;
            }

            semanticVersion = new SemanticVersion(
                major,
                minor,
                patch,
                preRelease,
                build,
                isPartial);
            return true;
        }
    }
}