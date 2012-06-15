using System;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Display
{
    public class DisplayAttribute : Attribute
    {
        public string Tag { get; set; }

        public DisplayAttribute(string tag)
        {
            Tag = tag;
        }
    }
}
