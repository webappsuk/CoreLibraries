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
using System.Diagnostics;
using WebApplications.Utilities.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test.SemanticVersions
{
    [TestClass]
    public class SemanticVersionTests : UtilitiesTestBase
    {
        [NotNull]
        private readonly Tuple<string, SemanticVersion, string>[] _validTests =
        {
            // Partials
            new Tuple<string, SemanticVersion, string>("*", SemanticVersion.Any, "*"),
            new Tuple<string, SemanticVersion, string>("*.*", SemanticVersion.Any, "*"),
            new Tuple<string, SemanticVersion, string>("*.*.*", SemanticVersion.Any, "*"),
            new Tuple<string, SemanticVersion, string>("*.*.*-*", SemanticVersion.Any, "*"),
            new Tuple<string, SemanticVersion, string>("*.*.*+*", SemanticVersion.Any, "*"),
            new Tuple<string, SemanticVersion, string>("*.*.*-*+*", SemanticVersion.Any, "*"),
            new Tuple<string, SemanticVersion, string>("1.*", new SemanticVersion(1), "1.*"),
            new Tuple<string, SemanticVersion, string>("1.*.*", new SemanticVersion(1), "1.*"),
            new Tuple<string, SemanticVersion, string>("1.*.*-*", new SemanticVersion(1), "1.*"),
            new Tuple<string, SemanticVersion, string>("1.*.*+*", new SemanticVersion(1), "1.*"),
            new Tuple<string, SemanticVersion, string>("1.*.*-*+*", new SemanticVersion(1), "1.*"),
            new Tuple<string, SemanticVersion, string>("1.2.*", new SemanticVersion(1, 2), "1.2.*"),
            new Tuple<string, SemanticVersion, string>("1.2.*-*", new SemanticVersion(1, 2), "1.2.*"),
            new Tuple<string, SemanticVersion, string>("1.2.*+*", new SemanticVersion(1, 2), "1.2.*"),
            new Tuple<string, SemanticVersion, string>("1.2.*-*+*", new SemanticVersion(1, 2), "1.2.*"),
            new Tuple<string, SemanticVersion, string>("1.2.3-*", new SemanticVersion(1, 2, 3), "1.2.3-*"),
            // Note that an absent pre-release or build is consider partial in an partial semantic version, but considered null in a full semantic version.
            new Tuple<string, SemanticVersion, string>("1.2.3+*", new SemanticVersion(1, 2, 3), "1.2.3-*"),
            new Tuple<string, SemanticVersion, string>("1.2.3-*+*", new SemanticVersion(1, 2, 3), "1.2.3-*"),
            new Tuple<string, SemanticVersion, string>("1.2.3-a+*", new SemanticVersion(1, 2, 3, "a"), "1.2.3-a+*"),

            // Complex partials
            new Tuple<string, SemanticVersion, string>("*.1", new SemanticVersion(Optional<int>.Unassigned, 1), "*.1.*"),
            new Tuple<string, SemanticVersion, string>(
                "*.1.*",
                new SemanticVersion(Optional<int>.Unassigned, 1),
                "*.1.*"),
            new Tuple<string, SemanticVersion, string>(
                "*.1.*-*",
                new SemanticVersion(Optional<int>.Unassigned, 1),
                "*.1.*"),
            new Tuple<string, SemanticVersion, string>(
                "*.1.*-*+*",
                new SemanticVersion(Optional<int>.Unassigned, 1),
                "*.1.*"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.1",
                new SemanticVersion(Optional<int>.Unassigned, Optional<int>.Unassigned, 1),
                "*.*.1-*"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.1-*",
                new SemanticVersion(Optional<int>.Unassigned, Optional<int>.Unassigned, 1),
                "*.*.1-*"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.1+*",
                new SemanticVersion(Optional<int>.Unassigned, Optional<int>.Unassigned, 1),
                "*.*.1-*"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.1-*+*",
                new SemanticVersion(Optional<int>.Unassigned, Optional<int>.Unassigned, 1),
                "*.*.1-*"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.*-",
                new SemanticVersion(Optional<int>.Unassigned, Optional<int>.Unassigned, Optional<int>.Unassigned, null),
                "*.*.*-+*"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.*-+*",
                new SemanticVersion(Optional<int>.Unassigned, Optional<int>.Unassigned, Optional<int>.Unassigned, null),
                "*.*.*-+*"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.*-*+",
                new SemanticVersion(
                    Optional<int>.Unassigned,
                    Optional<int>.Unassigned,
                    Optional<int>.Unassigned,
                    Optional<string>.Unassigned,
                    null),
                "*.*.*-*+"),
            
            // '-' is valid in pre-release and build strings!
            new Tuple<string, SemanticVersion, string>(
                "*.*.*--",
                new SemanticVersion(Optional<int>.Unassigned, Optional<int>.Unassigned, Optional<int>.Unassigned, "-"),
                "*.*.*--+*"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.*-*+-",
                new SemanticVersion(
                    Optional<int>.Unassigned,
                    Optional<int>.Unassigned,
                    Optional<int>.Unassigned,
                    Optional<string>.Unassigned,
                    "-"),
                "*.*.*-*+-"),
            new Tuple<string, SemanticVersion, string>(
                "*.*.*--+-",
                new SemanticVersion(
                    Optional<int>.Unassigned,
                    Optional<int>.Unassigned,
                    Optional<int>.Unassigned,
                    "-",
                    "-"),
                "*.*.*--+-"),


            // Full
            new Tuple<string, SemanticVersion, string>("1.2.3", new SemanticVersion(1, 2, 3, null, null), "1.2.3"),
            new Tuple<string, SemanticVersion, string>("1.2.3-a", new SemanticVersion(1, 2, 3, "a", null), "1.2.3-a"),
            new Tuple<string, SemanticVersion, string>("1.2.3-a+b", new SemanticVersion(1, 2, 3, "a", "b"), "1.2.3-a+b"),
            new Tuple<string, SemanticVersion, string>("1.2.3+b", new SemanticVersion(1, 2, 3, null, "b"), "1.2.3+b"),
            new Tuple<string, SemanticVersion, string>("1.2.3-", new SemanticVersion(1, 2, 3, null, null), "1.2.3"),
            new Tuple<string, SemanticVersion, string>("1.2.3+", new SemanticVersion(1, 2, 3, null, null), "1.2.3"),
            new Tuple<string, SemanticVersion, string>("1.2.3-+", new SemanticVersion(1, 2, 3, null, null), "1.2.3"),
            new Tuple<string, SemanticVersion, string>("1.2.3+-", new SemanticVersion(1, 2, 3, null, "-"), "1.2.3+-"),
        };

        [NotNull]
        private readonly string[] _invalidTests =
        {
            null,
            "",
            "1",
            "1.",
            "1.2",
            "1.2.",
            "*.*.*+*-*",
            "*.*.*.*",
            "*.*.*-*--"
        };

        [NotNull]
        private readonly SemanticVersion[] _orderTests =
        {
            new SemanticVersion(1, 0, 0),
            new SemanticVersion(1, 0, 0, "alpha", null),
            new SemanticVersion(1, 0, 0, "alpha.1", null),
            new SemanticVersion(1, 0, 0, "alpha.beta", null),
            new SemanticVersion(1, 0, 0, "beta", null),
            new SemanticVersion(1, 0, 0, null, null),
            new SemanticVersion(1, 2, 0),
            new SemanticVersion(1, 2, 0, "alpha", null),
            new SemanticVersion(1, 2, 0, "beta", null),
            new SemanticVersion(1, 2, 0, "beta.2", null),
            new SemanticVersion(1, 2, 0, "beta.11", null),
            new SemanticVersion(1, 2, 0, "rc.1", null),
            new SemanticVersion(1, 2, 0, null, null),
            new SemanticVersion(2),
            new SemanticVersion(2, 0),
            new SemanticVersion(2, 0, 0),
            new SemanticVersion(2, 0, 0, null),
        };

        [NotNull]
        private readonly SemanticVersion[] _ignoreBuildTests =
        {
            new SemanticVersion(1, 0, 0, null, null),
            new SemanticVersion(1, 0, 0, null, "build.1"),
            new SemanticVersion(1, 0, 0, null, "build.2"),
        };

        [NotNull]
        private readonly Tuple<bool, SemanticVersion, SemanticVersion>[] _matchTests =
        {
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1)),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1, 2)),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1, 2, 3)),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1, 2, 3, "a")),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1, 2, 3, "a", "b")),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(minor: 2)),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(patch: 3)),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(preRelease: "a")),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                true,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(build: "b")),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                false,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1, 1)),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                false,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1, patch: 1)),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                false,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1, preRelease: "b")),
            new Tuple<bool, SemanticVersion, SemanticVersion>(
                false,
                new SemanticVersion(1, 2, 3, "a", "b"),
                new SemanticVersion(1, build: "a")),
        };

        [TestMethod]
        public void SemanticVersion_TryParse_Succeeds()
        {
            bool success = true;
            foreach (Tuple<string, SemanticVersion, string> tuple in _validTests)
            {
                Assert.IsNotNull(tuple);
                Assert.IsNotNull(tuple.Item1);
                Assert.IsNotNull(tuple.Item2);
                Assert.IsNotNull(tuple.Item3);

                SemanticVersion semanticVersion;
                if (!SemanticVersion.TryParse(tuple.Item1, out semanticVersion))
                {
                    Trace.WriteLine(string.Format("Could not parse '{0}' as semantic version.", tuple.Item1));
                    success = false;
                    continue;
                }
                if (semanticVersion == null)
                {
                    Trace.WriteLine(string.Format("TryParse returned a null SemanticVersion for '{0}'.", tuple.Item1));
                    success = false;
                    continue;
                }
                if (!semanticVersion.Equals(tuple.Item2))
                {
                    Trace.WriteLine(
                        string.Format(
                            "The '{0}' semantic version does not match the returned SemanticVersion '{1}'.",
                            tuple.Item2,
                            semanticVersion));
                    success = false;
                    continue;
                }
                string resultString = semanticVersion.ToString();
                if (!resultString.Equals(tuple.Item3))
                {
                    Trace.WriteLine(
                        string.Format(
                            "The '{0}' semantic version string does not match the returned SemanticVersion.ToString() '{1}'.",
                            tuple.Item3,
                            resultString));
                    success = false;
                    continue;
                }
                Trace.WriteLine(string.Format("The '{0}' semantic version was parsed successfully.", tuple.Item1));
            }
            Assert.IsTrue(success, "Some tests failed.");
        }

        [TestMethod]
        public void SemanticVersion_TryParse_Fails()
        {
            bool success = true;
            foreach (string invalid in _invalidTests)
            {
                SemanticVersion semanticVersion;
                if (SemanticVersion.TryParse(invalid, out semanticVersion))
                {
                    Trace.WriteLine(
                        string.Format(
                            "Successfully parsed '{0}' as semantic version '{1}', when supposedly invalid.",
                            invalid,
                            semanticVersion));
                    success = false;
                    continue;
                }
                Trace.WriteLine(
                    string.Format("The '{0}' semantic version was successfully parsed as invalid.", invalid));
            }
            Assert.IsTrue(success, "Some tests failed.");
        }

        [TestMethod]
        public void SemanticVersion_Ordering()
        {
            SemanticVersion last = _orderTests[0];
            bool success = true;

            for (int i = 1; i < _orderTests.Length; i++)
            {
                SemanticVersion current = _orderTests[i];

                if (last < current)
                    Trace.WriteLine(string.Format("      '{0}' < '{1}'", last, current));
                else
                {
                    Trace.WriteLine(string.Format("Fail: '{0}' >= '{1}'", last, current));
                    success = false;
                }

                last = current;
            }

            Assert.IsTrue(success, "Some tests failed.");
        }

        [TestMethod]
        public void SemanticVersion_ComparisonIgnoresBuild()
        {
            SemanticVersion last = _ignoreBuildTests[0];
            bool success = true;

            for (int i = 1; i < _ignoreBuildTests.Length; i++)
            {
                SemanticVersion current = _ignoreBuildTests[i];

                if (last.Equals(current))
                    Trace.WriteLine(string.Format("      '{0}' == '{1}'", last, current));
                else
                {
                    Trace.WriteLine(string.Format("Fail: '{0}' != '{1}'", last, current));
                    success = false;
                }

                last = current;
            }

            Assert.IsTrue(success, "Some tests failed.");
        }

        [TestMethod]
        public void SemanticVersion_Matches()
        {
            bool success = true;

            foreach (Tuple<bool, SemanticVersion, SemanticVersion> pair in _matchTests)
            {
                bool matchExpected = pair.Item1;
                SemanticVersion a = pair.Item2;
                SemanticVersion b = pair.Item3;

                bool matches = a.Matches(b);
                Trace.Write(
                    matches
                        ? string.Format("'{0}' matches '{1}'", a, b)
                        : string.Format("'{0}' does not match '{1}'", a, b));

                if (matchExpected == matches)
                    Trace.WriteLine(" as expected");
                else
                {
                    Trace.WriteLine(" which is not expected");
                    success = false;
                }

                bool converse = b.Matches(a);
                if (converse != matches)
                {
                    Trace.WriteLine("The converse failed");
                    success = false;
                }
            }

            Assert.IsTrue(success, "Some tests failed.");
        }

        [TestMethod]
        public void SemanticVersion_GetFull()
        {
            SemanticVersion partial = new SemanticVersion(1, 2, 3);
            Assert.IsNotNull(partial);
            Assert.IsTrue(partial.IsPartial);
            SemanticVersion full = partial.GetFull();
            Assert.IsNotNull(full);
            Assert.IsFalse(full.IsPartial);
            Assert.IsTrue(partial.Matches(full));
        }
    }
}