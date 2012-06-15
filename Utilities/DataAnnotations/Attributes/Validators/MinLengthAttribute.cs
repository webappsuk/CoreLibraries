using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public class MinLengthAttribute : System.ComponentModel.DataAnnotations.MinLengthAttribute, ITravellerValidator
    {
        public MinLengthAttribute(int length) : base(length) { }

        public Base GetExt4Validator()
        {
            return new Length(this);
        }
    }
}