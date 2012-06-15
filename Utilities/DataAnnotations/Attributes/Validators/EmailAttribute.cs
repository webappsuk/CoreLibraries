using System;
using System.ComponentModel.DataAnnotations;
using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    public class EmailAttribute : ValidationAttribute, ITravellerValidator
    {
        public EmailAttribute()
        { }

        public Base GetExt4Validator()
        {
            return new EMail(this);
        }

        public override bool IsValid(object value)
        {
            string address;
            if ((address = value as string) != null)
            {
                // some specific string checks
                if (address.Contains(".@")
                    || address.Contains("..")
                    || address.IndexOf(".", StringComparison.Ordinal) == 0
                    || address.Contains("\\@")
                    )
                    return false;

                int indexOfAt = address.IndexOf('@');
                int lastIndexOfDot = address.LastIndexOf('.');
                int lastIndexOfColon = address.LastIndexOf(':');

                // at our most basic, check that we have an @ symbol and a period that comes after it
                return indexOfAt > 0
                    && (lastIndexOfDot > indexOfAt
                        || lastIndexOfColon > indexOfAt);
            }

            return false;
        }
    }
}
