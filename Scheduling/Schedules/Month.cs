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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedules
{
    /// <summary>
    /// Enumeration of the potential months in a year.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum Month
    {
        /// <summary>
        /// The month of January.
        /// </summary>
        January = 1 << 1,

        /// <summary>
        /// The month of February.
        /// </summary>
        February = 1 << 2,

        /// <summary>
        /// The month of March.
        /// </summary>
        March = 1 << 3,

        /// <summary>
        /// The month of April.
        /// </summary>
        April = 1 << 4,

        /// <summary>
        /// The month of May.
        /// </summary>
        May = 1 << 5,

        /// <summary>
        /// The month of June.
        /// </summary>
        June = 1 << 6,

        /// <summary>
        /// The month of July.
        /// </summary>
        July = 1 << 7,

        /// <summary>
        /// The month of August.
        /// </summary>
        August = 1 << 8,

        /// <summary>
        /// The month of September.
        /// </summary>
        September = 1 << 9,

        /// <summary>
        /// The month of October.
        /// </summary>
        October = 1 << 10,

        /// <summary>
        /// The month of November.
        /// </summary>
        November = 1 << 11,

        /// <summary>
        /// The month of December.
        /// </summary>
        December = 1 << 12,

        /// <summary>
        /// No months in the year.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Every six months starting from January.
        /// </summary>
        TwiceAYear = January | July,

        /// <summary>
        /// Every three months starting from January.
        /// </summary>
        FourTimesAYear = TwiceAYear | May | November,

        /// <summary>
        /// Every four months starting from January.
        /// </summary>
        ThreeTimesAYear = January | May | September,

        /// <summary>
        /// Every two months starting from January.
        /// </summary>
        EveryTwoMonths = FourTimesAYear | March | September,

        /// <summary>
        /// Every two months starting from February.
        /// </summary>
        EveryOtherMonth = ~EveryTwoMonths,

        /// <summary>
        /// Every month in the year.
        /// </summary>
        Every = EveryTwoMonths | EveryOtherMonth
    }
}