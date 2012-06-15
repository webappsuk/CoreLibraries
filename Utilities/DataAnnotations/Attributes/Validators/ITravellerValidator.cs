using System;

namespace WebApplications.Utilities.DataAnnotations.Attributes.Validators
{
    /// <summary>
    /// Traveller validator
    /// </summary>
    /// <remarks></remarks>
    [Obsolete("Traveller specific interface which needs to be removed.")]
    public interface ITravellerValidator
    {
        Base GetExt4Validator();
    }
}
