using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.Test.DataAnnotations
{
    public interface IExampleParent : IExampleParentParent
    {
        [Required]
        string ParentProperty { get; set; }
    }
}
