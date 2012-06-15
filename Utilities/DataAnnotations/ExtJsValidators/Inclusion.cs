using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations.ExtJsValidators
{
    public class Inclusion: Sieve
    {
        [JsonProperty("type")]
        public new string Type { get { return "inclusion"; } }

        public Inclusion(IncludeAttribute attribute)
            : base(attribute)
        { }
    }
}