using WebApplications.Utilities.DataAnnotations.Attributes.Display;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.Test.DataAnnotations
{
    public interface IExample : IExampleParent
    {
        [Required]
        [Display("CompanyId")]
        [IdProperty]
        int CompanyId { get; set; }

        // duplicate string length type attributes should be refactored
        [Required]
        [MaxLength(65)]
        [MinLength(15)]
        [StringLength(50, MinimumLength = 10)]
        [RegularExpression(@"\d{4}")]
        string Code { get; set; }

        [Compare("Compare2")]
        string Compare1 { get; set; }

        [Compare("Compare1")]
        string Compare2 { get; set; }

        [Exclude("Nope", "Not this either")]
        string Exclude { get; set; }

        [Include("onlyThis", "maybeThisToo")]
        string Include { get; set; }

        [Email]
        string Email { get; set; }
    }
}
