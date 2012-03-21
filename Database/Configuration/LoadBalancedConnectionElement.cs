using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A LoadBalanced connection.
    /// </summary>
    public class LoadBalancedConnectionElement : Utilities.Configuration.ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the identifier for a load balanced connection.
        /// </summary>
        /// <value>The connection element.</value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("id", IsRequired = false, IsKey = true)]
        [NotNull]
        public string Id
        {
            get { return GetProperty<string>("id"); }
            set { SetProperty("id", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether schemas must be identical.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if schemas must be identical; otherwise <see langword="false"/>.
        /// </value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("ensureSchemasIdentical", DefaultValue = false, IsRequired = false)]
        public bool EnsureSchemasIdentical
        {
            get { return GetProperty<bool>("ensureSchemasIdentical"); }
            set { SetProperty("ensureSchemasIdentical", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether the load balanced collection is enabled.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the load balanced collection is enabled; otherwise <see langword="false"/>.
        /// </value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        ///   Gets or sets the connections.
        /// </summary>
        /// <value>The connections.</value>
        /// <exception cref="ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ConnectionCollection), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        [NotNull]
        public ConnectionCollection Connections
        {
            get { return GetProperty<ConnectionCollection>(""); }
            set { SetProperty("", value); }
        }

        /// <summary>
        ///   Used to initialize a default set of values for the <see cref="ConnectionCollection"/> object.
        /// </summary>
        /// <remarks>
        ///   Called to set the internal state to appropriate default values.
        /// </remarks>
        protected override void InitializeDefault()
        {
            // ReSharper disable ConstantNullCoalescingCondition
            Connections = Connections ?? new ConnectionCollection();
            // ReSharper restore ConstantNullCoalescingCondition

            base.InitializeDefault();
        }

        /// <summary>
        ///   Performs an implicit conversion from a <see cref="ConnectionCollection"/> 
        ///   to a <see cref="WebApplications.Utilities.Database.LoadBalancedConnection"/>.
        /// </summary>
        /// <param name="collection">The collection element.</param>
        /// <returns>The result of the conversion.</returns>
        [CanBeNull]
        public static implicit operator LoadBalancedConnection([CanBeNull]LoadBalancedConnectionElement collection)
        {
            return collection == null || !collection.Enabled
                       ? null
                       : new LoadBalancedConnection(
                             collection.Connections
                                 .Where(lbc => lbc != null && lbc.Enabled)
                                 .Select(lbc => new KeyValuePair<string, double>(lbc.ConnectionString, lbc.Weight)),
                             collection.EnsureSchemasIdentical);
        }
    }
}