#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: LatLng.cs
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

using System;

namespace WebApplications.Utilities.Location
{
    /// <summary>
    ///   Stores the Latitude/Longitude of a point.
    /// </summary>
    public struct LatLng
    {
        internal const double EarthsRadiusInKilometers = 6371;

        /// <summary>
        ///   The Latitude of the point.
        /// </summary>
        public readonly double Latitude;

        /// <summary>
        ///   The Longitude of the point.
        /// </summary>
        public readonly double Longitude;

        /// <summary>
        ///   Initialises a new instance of the <see cref="LatLng"/> class.
        /// </summary>
        /// <param name="latitude">The Latitude of the point.</param>
        /// <param name="longitude">The Longitude of the point.</param>
        public LatLng(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        ///   Calculates the distance (in km) between the current point and the <see cref="LatLng">point</see> specified.
        ///   By default this uses the Law of Cosines formula (http://en.wikipedia.org/wiki/Law_of_cosines).
        /// </summary>
        /// <param name="otherPoint">The point we want to calculate the distance to.</param>
        /// <param name="calculation">
        ///   <para>The distance calculation to use.</para>
        ///   <para>By default this uses the Law of Cosines formula.</para>
        /// </param>
        /// <returns>
        ///   The distance (in km) between the current point and the <paramref name="otherPoint"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The specified <paramref name="calculation"/> is outside the allowable <see cref="DistanceCalculation">values</see>.
        /// </exception>
        public double DistanceTo(LatLng otherPoint, DistanceCalculation calculation = DistanceCalculation.LawOfCosines)
        {
            return Distance(this, otherPoint, calculation);
        }

        /// <summary>
        ///   Calculates the midpoint between the current point and the <see cref="LatLng">point</see> specified.
        /// </summary>
        /// <param name="otherPoint">The other point to use in the calculation.</param>
        /// <returns>
        ///   The midpoint between the current point and the <paramref name="otherPoint"/> specified.
        /// </returns>
        public LatLng MidpointTo(LatLng otherPoint)
        {
            return MidPoint(this, otherPoint);
        }

        /// <summary>
        ///   Calculates the distance (in km) between two specified <see cref="LatLng">latlong points</see>.
        ///   By default this uses the Law of Cosines formula (http://en.wikipedia.org/wiki/Law_of_cosines).
        /// </summary>
        /// <param name="pointOne">The first point to use in the calculation.</param>
        /// <param name="pointTwo">The second point to use in the calculation.</param>
        /// <param name="calculation">
        ///   <para>The calculation to use.</para>
        ///   <para>By default this uses the Law of Cosines formula.</para>
        /// </param>
        /// <returns>
        ///   The distance (in km) between the two points specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The specified <paramref name="calculation"/> is outside the allowable <see cref="DistanceCalculation">values</see>.
        /// </exception>
        public static double Distance(LatLng pointOne, LatLng pointTwo,
                                      DistanceCalculation calculation = DistanceCalculation.LawOfCosines)
        {
            switch (calculation)
            {
                case DistanceCalculation.Haversine:
                    double pointOneLatitude = pointOne.Latitude.ToRadians();
                    double pointOneLongitude = pointOne.Longitude.ToRadians();
                    double pointTwoLatitude = pointTwo.Latitude.ToRadians();
                    double pointTwoLongitude = pointTwo.Longitude.ToRadians();

                    double longitude = pointTwoLongitude - pointOneLongitude;
                    double latitude = pointTwoLatitude - pointOneLatitude;

                    double intermediateResult = Math.Pow(Math.Sin(latitude/2.0), 2.0) +
                                                Math.Cos(pointOneLatitude)*Math.Cos(pointTwoLatitude)*
                                                Math.Pow(Math.Sin(longitude/2.0), 2.0);

                    // Intermediate result c (great circle distance in Radians).

                    double c = 2.0*Math.Atan2(Math.Sqrt(intermediateResult), Math.Sqrt(1.0 - intermediateResult));

                    return EarthsRadiusInKilometers*c;
                case DistanceCalculation.LawOfCosines:
                    return (Math.Acos(
                        Math.Sin(pointOne.Latitude)*Math.Sin(pointTwo.Latitude) +
                        Math.Cos(pointOne.Latitude)*Math.Cos(pointTwo.Latitude)*
                        Math.Cos(pointTwo.Longitude - pointOne.Longitude)
                                )*EarthsRadiusInKilometers).ToRadians();
                default:
                    throw new ArgumentOutOfRangeException("calculation");
            }
        }

        /// <summary>
        ///   Calculates the midpoint between two specified <see cref="LatLng">latlong points</see>.
        /// </summary>
        /// <param name="pointOne">The first point to use in the calculation.</param>
        /// <param name="pointTwo">The second point to use in the calculation.</param>
        /// <returns>
        ///   The midpoint between the two points specified.
        /// </returns>
        public static LatLng MidPoint(LatLng pointOne, LatLng pointTwo)
        {
            double dLon = (pointTwo.Longitude - pointTwo.Longitude).ToRadians();
            double bx = Math.Cos(pointTwo.Latitude.ToRadians())*Math.Cos(dLon);
            double by = Math.Cos(pointTwo.Latitude.ToRadians())*Math.Sin(dLon);

            double latitude = (Math.Atan2(
                Math.Sin(pointOne.Latitude.ToRadians()) + Math.Sin(pointTwo.Latitude.ToRadians()),
                Math.Sqrt(
                    (Math.Cos(pointOne.Latitude.ToRadians()) + bx)*
                    (Math.Cos(pointOne.Latitude.ToRadians()) + bx) + by*by))).ToDegrees();

            double longitude = pointOne.Longitude +
                               Math.Atan2(by, Math.Cos(pointOne.Latitude.ToRadians()) + bx).ToDegrees();

            return new LatLng(latitude, longitude);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Lat: {0}, Long: {1}", Latitude, Longitude);
        }
    }
}