using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public class Base
    {
        [JsonProperty("type")]
        public virtual string Type { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("errorTag")]
        public string ErrorTag { get; set; }

        public Base(ValidationAttribute attribute)
        {
            ErrorTag = attribute.ErrorMessage;
        }

        public Base() { }
    }
}