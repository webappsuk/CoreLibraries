using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public class CompareAttribute : System.ComponentModel.DataAnnotations.CompareAttribute, ITravellerValidator
    {
        public CompareAttribute(string otherProperty) 
            : base(otherProperty)
        { }

        public Base GetExt4Validator()
        {
            return new Compare(this);
        }
    }
}