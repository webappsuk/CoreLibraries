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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Location;

namespace WebApplications.Utilities.Test.Location
{
    [TestClass]
    public class LatLngTests : UtilitiesTestBase
    {
        private const double _MaxLat = 90;
        private const double _MinLat = -90;
        private const double _MaxLng = 180;
        private const double _MinLng = -180;

        private static double RandomLat()
        {
            return Random.NextDouble()*(_MaxLat - _MinLat) + _MinLat;
        }

        private static double RandomLng()
        {
            return Random.NextDouble()*(_MaxLng - _MinLng) + _MinLng;
        }

        [TestMethod]
        public void LatLng_ValidConstructor_PropertiesMatchThoseSupplied()
        {
            double lat = RandomLat();
            double lng = RandomLng();

            LatLng testLatLng = new LatLng(lat, lng);

            Assert.AreEqual(lat, testLatLng.Latitude, "Latitude property should return the value initially supplied");
            Assert.AreEqual(lng, testLatLng.Longitude, "Longtitude property should return the value initially supplied");
        }

        [TestMethod]
        public void LatLng_EquivalantLatLng_AreEqual()
        {
            double lat = RandomLat();
            double lng = RandomLng();

            LatLng testLatLngA = new LatLng(lat, lng);
            LatLng testLatLngB = new LatLng(lat, lng);

            Assert.AreEqual(testLatLngA, testLatLngB,
                            "Two LatLng instances describing the same location should be equivalant");
        }

        [TestMethod]
        public void LatLng_DistanceTo_GivesSameResultAsDistance()
        {
            LatLng testLatLngA = new LatLng(RandomLat(), RandomLng());
            LatLng testLatLngB = new LatLng(RandomLat(), RandomLng());

            Assert.AreEqual(LatLng.Distance(testLatLngA, testLatLngB), testLatLngA.DistanceTo(testLatLngB),
                            "DistanceTo method should give the same result as static Distance method");
        }

        [TestMethod]
        public void LatLng_MidpointTo_GivesSameResultAsMidpoint()
        {
            LatLng testLatLngA = new LatLng(RandomLat(), RandomLng());
            LatLng testLatLngB = new LatLng(RandomLat(), RandomLng());

            Assert.AreEqual(LatLng.MidPoint(testLatLngA, testLatLngB), testLatLngA.MidpointTo(testLatLngB),
                            "MidpointTo method should give the same result as static Midpoint method");
        }
    }
}