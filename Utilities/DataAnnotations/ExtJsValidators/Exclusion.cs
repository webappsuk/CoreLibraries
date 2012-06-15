using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations.ExtJsValidators
{
    public class Exclusion : Sieve
    {
        [JsonProperty("type")]
        public override string Type { get { return "exclusion"; } }

        public Exclusion(ExcludeAttribute attribute)
            : base(attribute)
        { }
    }
}