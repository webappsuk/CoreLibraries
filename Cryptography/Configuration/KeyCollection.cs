using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Cryptography.Configuration
{
    public class KeyCollection : ConfigurationElementCollection<string, KeyElement>
    {
        protected override string GetElementKey(KeyElement element)
        {
            return element.Value;
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}