using System.Globalization;
using Newtonsoft.Json;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.DataAnnotations.ExtJsValidators
{
    public class Length : Base
    {
        [JsonProperty("type")]
        public new string Type { get { return "length"; } }
        [JsonProperty("min")]
        public string Min { get; set; }
        [JsonProperty("max")]
        public string Max { get; set; }

        public Length()
        { }

        public Length(MaxLengthAttribute attribute)
            : base(attribute)
        {
            Max = attribute.Length.ToString(CultureInfo.InvariantCulture);
        }

        public Length(MinLengthAttribute attribute)
            : base(attribute)
        {
            Min = attribute.Length.ToString(CultureInfo.InvariantCulture);
        }

        public Length(StringLengthAttribute attribute)
            : base(attribute)
        {
            Max = attribute.MaximumLength.ToString(CultureInfo.InvariantCulture);
            Min = attribute.MinimumLength.ToString(CultureInfo.InvariantCulture);
        }
    }
}