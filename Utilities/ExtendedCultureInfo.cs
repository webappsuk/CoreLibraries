using System;
using System.Globalization;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    public class ExtendedCultureInfo : CultureInfo
    {
        /// <summary>
        /// Gets the associated region information.
        /// </summary>
        /// <value>
        /// The region information.
        /// </value>
        [PublicAPI]
        [NotNull]
        public readonly RegionInfo RegionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCultureInfo"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="cultureInfo">The culture information.</param>
        internal ExtendedCultureInfo([NotNull] CultureInfo cultureInfo)
            : base(cultureInfo.LCID, cultureInfo.UseUserOverride)
        {
            RegionInfo = new RegionInfo(cultureInfo.LCID);
        }
    }
}