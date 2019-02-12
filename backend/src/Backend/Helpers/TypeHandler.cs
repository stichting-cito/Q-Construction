using Citolab.QConstruction.Model;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citolab.QConstruction.Backend.Helpers
{
    public class TypeHandler : AuthorizationHandler<TypeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TypeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "userType"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var claimValue = context.User.FindFirst(c => c.Type == "UserType").Value;
            UserType claimAsType = (UserType)Enum.Parse(typeof(UserType), claimValue);
            if (requirement.UserTypes.Any(x => x == claimAsType))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class TypeRequirement : IAuthorizationRequirement
    {
        public TypeRequirement(params UserType[] userTypes)
        {
            UserTypes = userTypes;
        }

        public UserType[] UserTypes { get; }
    }
}
