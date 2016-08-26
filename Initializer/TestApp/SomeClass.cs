namespace WebApplications.Utilities.Initializer.TestApp
{
    public class SomeClass
    {
        /// <summary>
        /// Added for testing issue #27. A method in this project using a type from
        /// another project as a constant, for the default value of the parameter.
        /// </summary>
        /// <param name="importance">The importance.</param>
        public void SomeMethod(OutputImportance importance = OutputImportance.Error)
        {
        }
    }
}