using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Cryptography.Configuration
{
    public class KeyElement : Utilities.Configuration.ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return GetProperty<string>("value"); }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("expiry", IsRequired = true)]
        public DateTime Expiry
        {
            get { return GetProperty<DateTime>("expiry"); }
            set { this["expiry"] = value; }
        }
    }
}
