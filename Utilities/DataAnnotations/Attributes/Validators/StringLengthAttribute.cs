using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public class StringLengthAttribute : System.ComponentModel.DataAnnotations.StringLengthAttribute, ITravellerValidator
    {
        public StringLengthAttribute(int maximumLength) : base(maximumLength)
        { }

        public Base GetExt4Validator()
        {
            return new Length(this);
        }
    }
}