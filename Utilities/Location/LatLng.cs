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
        public static double Distance(
            LatLng pointOne,
            LatLng pointTwo,
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

                    double intermediateResult = Math.Pow(Math.Sin(latitude / 2.0), 2.0) +
                                                Math.Cos(pointOneLatitude) * Math.Cos(pointTwoLatitude) *
                                                Math.Pow(Math.Sin(longitude / 2.0), 2.0);

                    // Intermediate result c (great circle distance in Radians).

                    double c = 2.0 * Math.Atan2(Math.Sqrt(intermediateResult), Math.Sqrt(1.0 - intermediateResult));

                    return EarthsRadiusInKilometers * c;
                case DistanceCalculation.LawOfCosines:
                    return (Math.Acos(
                        Math.Sin(pointOne.Latitude) * Math.Sin(pointTwo.Latitude) +
                        Math.Cos(pointOne.Latitude) * Math.Cos(pointTwo.Latitude) *
                        Math.Cos(pointTwo.Longitude - pointOne.Longitude)
                        ) * EarthsRadiusInKilometers).ToRadians();
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
            double bx = Math.Cos(pointTwo.Latitude.ToRadians()) * Math.Cos(dLon);
            double by = Math.Cos(pointTwo.Latitude.ToRadians()) * Math.Sin(dLon);

            double latitude = (Math.Atan2(
                Math.Sin(pointOne.Latitude.ToRadians()) + Math.Sin(pointTwo.Latitude.ToRadians()),
                Math.Sqrt(
                    (Math.Cos(pointOne.Latitude.ToRadians()) + bx) *
                    (Math.Cos(pointOne.Latitude.ToRadians()) + bx) + by * by))).ToDegrees();

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