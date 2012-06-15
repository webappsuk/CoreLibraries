using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations.ExtJsValidators
{
    public class EMail : Base
    {
        [JsonProperty("type")]
        public override string Type { get { return "email"; } }

        public EMail(ValidationAttribute attribute)
            : base(attribute)
        { }
    }
}