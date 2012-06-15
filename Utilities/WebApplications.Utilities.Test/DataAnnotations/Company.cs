using System.ComponentModel.DataAnnotations;
using WebApplications.Utilities.DataAnnotations.Attributes.Display;

namespace WebApplications.Utilities.Test.DataAnnotations
{
    [ApiLocation("/api/Company/")]
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [Utilities.DataAnnotations.Attributes.Display.Display("Models.Company.TypeId")]
        public int TypeId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "CodeErrorLength")]
        [Utilities.DataAnnotations.Attributes.Display.Display("Models.Company.Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "CodeErrorLength")]
        [Utilities.DataAnnotations.Attributes.Display.Display("Models.Company.Code")]
        public string Code { get; set; }
    }
}
