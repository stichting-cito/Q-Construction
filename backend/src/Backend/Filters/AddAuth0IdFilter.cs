using System;
using System.Linq;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Citolab.QConstruction.Backend.Filters
{
    public class AddUserIdFilter : IActionFilter
    {
        private readonly ILoggedInUserProvider _loggedInUserProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMemoryCache _memoryCache;
        public AddUserIdFilter(ILoggedInUserProvider loggedInUserProvider, IHttpContextAccessor httpContextAccessor,
            IRepositoryFactory repositoryFactory, IMemoryCache memoryCache)
        {
            _loggedInUserProvider = loggedInUserProvider;
            _httpContextAccessor = httpContextAccessor;
            _repositoryFactory = repositoryFactory;
            _memoryCache = memoryCache;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var auth0Id = _httpContextAccessor.HttpContext.User.Auth0Id();
            if (!string.IsNullOrEmpty(auth0Id))
            {
                _loggedInUserProvider.UserObject = GetUser(auth0Id);
            }
        }

        private User GetUser(string auth0Id)
        {
            if (_memoryCache.TryGetValue(auth0Id, out var u) && (u is User user))
            {
                return user;
            }
            var dbUser = _repositoryFactory.GetRepository<User>().AsQueryable()
                .FirstOrDefault(us => us.IdToken == auth0Id);
            if (dbUser == null) return null;
            _memoryCache.Set(auth0Id, dbUser, DateTimeOffset.Now.AddDays(1));
            return dbUser;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }
    }
}
