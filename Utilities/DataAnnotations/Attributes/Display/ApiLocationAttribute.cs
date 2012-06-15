using System;
using Newtonsoft.Json;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Display
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ApiLocationAttribute : Attribute
    {
        [JsonProperty("apiUrl")]
        public string ApiUrl { get; set; }

        public ApiLocationAttribute(string location)
        {
            ApiUrl = location;
        }
    }
}
