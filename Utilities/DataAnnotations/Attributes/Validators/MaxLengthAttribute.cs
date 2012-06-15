using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public class MaxLengthAttribute : System.ComponentModel.DataAnnotations.MaxLengthAttribute, ITravellerValidator
    {
        public MaxLengthAttribute(int length) 
            : base(length) 
        { }

        public Base GetExt4Validator()
        {
            return new Length(this);
        }
    }
}