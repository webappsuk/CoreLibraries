using System;
using System.Collections.Generic;
using System.Globalization;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Interface to an object that provides <see cref="CultureInfo"/>.
    /// </summary>
    public interface ICultureInfoProvider
    {
        /// <summary>
        /// The date this provider was published.
        /// </summary>
        [PublicAPI]
        DateTime Published { get; }

        /// <summary>
        /// The cultures in the provider.
        /// </summary>
        [PublicAPI]
        [NotNull]
        [ItemNotNull]
        IEnumerable<ExtendedCultureInfo> All { get; }

        /// <summary>
        /// Gets the number of cultures specified in the provider.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [PublicAPI]
        int Count { get; }

        /// <summary>
        /// Retrieves an <see cref="ExtendedCultureInfo" /> with the name specified (see <see cref="CultureInfo.Name"/>).
        /// </summary>
        /// <param name="cultureName">The ISO Code.</param>
        /// <returns>
        ///   The <see cref="CultureInfo"/> that corresponds to the <paramref name="cultureName"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="cultureName"/> cannot be null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureName"/> is <see langword="null"/>.
        /// </exception>
        [PublicAPI]
        [CanBeNull]
        ExtendedCultureInfo Get([NotNull] string cultureName);

        /// <summary>
        /// Retrieves an <see cref="ExtendedCultureInfo" /> equivalent to the specified (see <see cref="CultureInfo"/>).
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>
        ///   The <see cref="CultureInfo"/> that corresponds to the <paramref name="cultureInfo"/> specified (if any);
        ///   otherwise the default value for the type is returned.
        /// </returns>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> for this method,
        ///   <paramref name="cultureInfo"/> cannot be null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        [PublicAPI]
        [CanBeNull]
        ExtendedCultureInfo Get([NotNull] CultureInfo cultureInfo);

        /// <summary>
        /// Finds the cultures that use a specific currency.
        /// </summary>
        /// <param name="currencyInfo">The currency information.</param>
        /// <returns>The cultures that us the specified currency.</returns>
        IEnumerable<ExtendedCultureInfo> FindByCurrency(CurrencyInfo currencyInfo);
    }
}