using System.Threading.Tasks;
using Citolab.QConstruction.Backend.HelperClasses;
using Microsoft.AspNetCore.Authorization;

namespace Citolab.QConstruction.Logic.HelperClasses.Authorization
{
    /// <summary>
    ///     Role han
    /// </summary>
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        /// <summary></summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            if (context.User.Role() == requirement.Role)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}