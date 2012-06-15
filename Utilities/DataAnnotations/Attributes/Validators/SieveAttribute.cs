using System.Collections.Generic;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public abstract class SieveAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        public IEnumerable<object> ComparisonValues { get; set; }

        protected SieveAttribute(params object[] values)
        {
            ComparisonValues = values;
        }
    }
}
