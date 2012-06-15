using System;
using System.Runtime.Serialization;
using WebApplications.Utilities.DataAnnotations.Attributes.Display;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.Test.DataAnnotations
{
    public class Example : IExample
    {
        public enum EnumType
        {
            FirstValue,
            SecondValue,
            ThirdValue
        }

        [Required]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string Code { get; set; }

        public string Compare1 { get; set; }

        public string Compare2 { get; set; }

        public string Exclude { get; set; }

        public string Include { get; set; }

        public string Email { get; set; }

        [Required]
        public int TypeId { get; set; }

        public DateTime TestDate { get; set; }

        [IgnoreDataMember]
        public BadType BadType { get; set; }

        [Display("DisplayTag")]
        public string Display { get; set; }

        public string ParentProperty { get; set; }

        public EnumType EnumValue { get; set; }
    }
}