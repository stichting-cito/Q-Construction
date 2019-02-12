using System;
using Citolab.QConstruction.Model;

namespace Citolab.QConstruction.Logic.HelperClasses
{
    public class Auth0LoggedInUserProvider : ILoggedInUserProvider
    {
        public Guid? GetUserId()
        {
            return (UserObject as User)?.Id;
        }

        public object UserObject { get; set; }
    }

        public interface ILoggedInUserProvider
    {
        object UserObject { get; set; }
        Guid? GetUserId();
    }
}
