using System;
using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute, ITravellerValidator
    {
        public Base GetExt4Validator()
        {
            return new Presence(this);
        }
    }
}