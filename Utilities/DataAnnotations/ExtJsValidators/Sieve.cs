using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations.ExtJsValidators
{
    public abstract class Sieve : Base
    {
        [JsonProperty("list")]
        public List<object> List { get; set; }

        protected Sieve(SieveAttribute attribute)
            : base(attribute)
        {
            // get list of valid items
            List = attribute.ComparisonValues.ToList();
        }
    }
}