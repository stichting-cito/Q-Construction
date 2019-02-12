using Microsoft.AspNetCore.Authorization;

namespace Citolab.QConstruction.Logic.HelperClasses.Authorization
{
    /// <summary>
    ///     Role requirement attribute
    /// </summary>
    public class RoleRequirement : IAuthorizationRequirement
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="role"></param>
        public RoleRequirement(string role)
        {
            Role = role;
        }

        /// <summary>
        ///     Role of the role
        /// </summary>
        public string Role { get; set; }
    }
}