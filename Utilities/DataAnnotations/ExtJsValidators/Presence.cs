using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations.ExtJsValidators
{
    public class Presence : Base
    {
        [JsonProperty("type")]
        public new string Type { get { return "presence"; } }

        public Presence(RequiredAttribute attribute)
            : base(attribute)
        { }
    }
}