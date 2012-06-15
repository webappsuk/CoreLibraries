using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public class RegularExpressionAttribute : System.ComponentModel.DataAnnotations.RegularExpressionAttribute, ITravellerValidator
    {
        public RegularExpressionAttribute(string pattern) : base(pattern) { }

        public Base GetExt4Validator()
        {
            return new Format(this);
        }
    }
}