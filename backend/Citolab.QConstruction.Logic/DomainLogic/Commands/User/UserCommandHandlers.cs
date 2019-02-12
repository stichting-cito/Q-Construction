using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Auth0.ManagementApi.Models;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.User
{
    /// <summary>
    ///     Add user command
    /// </summary>
    public class UserCommandHandlers
        : IRequestHandler<AddUserCommand, Model.User>,
        IRequestHandler<UserByAuth0IdQuery, Model.User>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILoggedInUserProvider _loggedInUser;

        /// <summary>
        ///     Add user command handler
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="memoryCache"></param>
        /// <param name="loggedInUser"></param>
        public UserCommandHandlers(IRepositoryFactory repositoryFactory, IMemoryCache memoryCache, ILoggedInUserProvider loggedInUser)
        {
            _repositoryFactory = repositoryFactory;
            _memoryCache = memoryCache;
            _loggedInUser = loggedInUser;
        }

        public Task<Model.User> Handle(AddUserCommand message, CancellationToken cancellationToken)
        {

            var userRepository = _repositoryFactory.GetRepository<Model.User>();
            if (!_loggedInUser.GetUserId().HasValue)  throw  new UnauthorizedAccessException();
            var loggedInUser = userRepository.GetAsync(_loggedInUser.GetUserId().Value).Result;
            if (!(loggedInUser.UserType == UserType.Admin || loggedInUser.UserType == UserType.Manager)) throw new UnauthorizedAccessException();
            //dirty fix: 
            if (string.IsNullOrEmpty(message.Value.Username)) message.Value.Username = message.Value.Name;
            var indb = userRepository.AsQueryable().FirstOrDefault(u => u.Email == message.Value.Email);
            //user with same email cant be added again. 
            if (indb != null) return Task.Run(() => indb);
            var emailList = new List<string> { message.Value.Email };
            var existingUsers = GeneralHelper.GetUsersByEmail(_memoryCache, emailList);
            var existingUser = existingUsers != null && existingUsers.Any()
                ? existingUsers.First()
                : CreateAuth0User(message.Value);
            message.Value.IdToken = existingUser.UserId;
            message.Value.Password = string.Empty; //clear password, its set to the Auth0 user
            return userRepository.AddAsync(message.Value);
        }

        public Task<Model.User> Handle(UserByAuth0IdQuery message, CancellationToken cancellationToken) =>
            Task.Run(() =>
            {
                var userRepository = _repositoryFactory.GetRepository<Model.User>();
                return userRepository.AsQueryable().FirstOrDefault(u => u.IdToken == message.ExternalId);
            });
        private Auth0.ManagementApi.Models.User CreateAuth0User(Model.User user) =>
            GeneralHelper.GetUserApiClient(_memoryCache).CreateAsync(new UserCreateRequest
            {
                Email = user.Email,
                EmailVerified = true,
                Connection = "Username-Password-Authentication",
                FullName = user.Name,
                Password = user.Password,
                AppMetadata = JsonConvert.DeserializeObject($@"{{""roles"": [""{user.UserType}""]}}")
            }).Result;
    }
}