using System.Linq;
using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public class ExcludeAttribute : SieveAttribute, ITravellerValidator
    {
        public ExcludeAttribute(params object[] values)
            : base(values)
        { }

        public Base GetExt4Validator()
        {
            return new Exclusion(this);
        }

        public override bool IsValid(object value)
        {
            return !ComparisonValues.Contains(value);
        }
    }
}
