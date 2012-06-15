using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations.ExtJsValidators
{
    public class Compare : Base
    {
        [JsonProperty("type")]
        public new virtual string Type { get { return "compare"; } }

        public string CompareTo { get; private set; }

        public Compare(CompareAttribute attribute)
            : base(attribute)
        {
            CompareTo = attribute.OtherProperty;
        }
    }
}
