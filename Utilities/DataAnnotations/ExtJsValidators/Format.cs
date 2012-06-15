using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations.ExtJsValidators
{
    public class Format : Base
    {
        [JsonProperty("type")]
        public new string Type { get { return "format"; } }

        [JsonProperty("matcher")]
        public Regex Expression { get; set; }

        public Format(RegularExpressionAttribute attribute) : base(attribute)
        {
            Expression = new Regex(attribute.Pattern);
        }
    }
}