#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: LatLngTests.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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
            return Random.NextDouble() * (_MaxLat - _MinLat) + _MinLat;
        }

        private static double RandomLng()
        {
            return Random.NextDouble() * (_MaxLng - _MinLng) + _MinLng;
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

            Assert.AreEqual(testLatLngA, testLatLngB, "Two LatLng instances describing the same location should be equivalant");
        }

        [TestMethod]
        public void LatLng_DistanceTo_GivesSameResultAsDistance()
        {
            LatLng testLatLngA = new LatLng(RandomLat(), RandomLng());
            LatLng testLatLngB = new LatLng(RandomLat(), RandomLng());

            Assert.AreEqual(LatLng.Distance(testLatLngA, testLatLngB), testLatLngA.DistanceTo(testLatLngB), "DistanceTo method should give the same result as static Distance method");
        }

        [TestMethod]
        public void LatLng_MidpointTo_GivesSameResultAsMidpoint()
        {
            LatLng testLatLngA = new LatLng(RandomLat(), RandomLng());
            LatLng testLatLngB = new LatLng(RandomLat(), RandomLng());

            Assert.AreEqual(LatLng.MidPoint(testLatLngA, testLatLngB), testLatLngA.MidpointTo(testLatLngB), "MidpointTo method should give the same result as static Midpoint method");
        }
    }
}
