using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using Auth0.Core.Collections;
using Auth0.Core.Http;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.Repository;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
namespace Citolab.QConstruction.Logic.HelperClasses
{
    /// <summary>
    ///     General helper functions
    /// </summary>
    public static class GeneralHelper
    {

        private static string GetToken(IMemoryCache memoryCache)
        {
            if (memoryCache != null && memoryCache.TryGetValue("cached_token", out string cachedToken))
            {
                return cachedToken;
            }
            using (var client = new HttpClient())
            {
                var response = client.PostAsync($"https://{Constants.Auth0Domain}/oauth/token",
                    CreateStringContentToPost(new AuthenticationObject
                    {
                        Audience = $"https://{Constants.Auth0Domain}/api/v2/",
                        Client_secret = Constants.ClientSecret,
                        Client_id = Constants.ManagementClientId
                    })).Result;
                var resposeContent = JsonConvert.DeserializeObject<JObject>(response.Content.ReadAsStringAsync().Result);
                var token = resposeContent.Value<string>("access_token");
                memoryCache.Set("cached_token", token, DateTimeOffset.Now.AddHours(4));
                return token;
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class AuthenticationObject
        {
            public string Grant_type => "client_credentials";
            public string Client_id { get; set; }
            public string Client_secret { get; set; }
            public string Audience { get; set; }
        }

        public static StringContent CreateStringContentToPost(object body) =>
            new StringContent(ObjectToString(body), Encoding.UTF8, "application/json");


        public static string ObjectToString(object objectToSerialise) =>
            JsonConvert.SerializeObject(objectToSerialise, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

        public static IUsersClient GetUserApiClient(IMemoryCache memoryCache) =>
            new UsersClient(new ApiConnection(GetToken(memoryCache),
                $"https://{Constants.Auth0Domain}/api/v2", DiagnosticsHeader.Default));


        public static IList<User> GetAll(IMemoryCache memoryCache)
        {
            // This will work up to 1000 users.
            PagedList<User> users;
            var client = GetUserApiClient(memoryCache);
            const int perPage = 100; //100 = max
            users = new PagedList<Auth0.ManagementApi.Models.User>();
            var index = 0;
            while (true)
            {
                var request = new GetUsersRequest { SearchEngine = "v3" };
                var paginationInfo = new PaginationInfo(index, perPage, false);
                var retrievedUsers = client.GetAllAsync(request, paginationInfo).Result;
                if (retrievedUsers.Count == 0) break;
                users.AddRange(retrievedUsers);
                index++;
                Thread.Sleep(600); // new Auth0 rate limits since 12-09-2017:
            }
            memoryCache.Set("auth0Users", users, DateTimeOffset.UtcNow.AddHours(1));
            return users;
        }

        public static IList<User> GetUsersByEmail(IMemoryCache memoryCache, IList<string> emailAdresses)
        {
            emailAdresses = emailAdresses.Select(e => e.ToLower()).ToList();
            memoryCache.TryGetValue("auth0Users", out PagedList<User> users);
            if (users != null && emailAdresses.Count(e => users.Any(u => u.Email == e)) == emailAdresses.Count())
                return users.Where(u => emailAdresses.Contains(u.Email.ToLower())).ToList();
            var client = GetUserApiClient(memoryCache);

            const int perPage = 100; //100 = max
            if (users == null)
            {
                users = new PagedList<User>();
            }

            var index = 0;
            while (true)
            {
                var addresses = emailAdresses.Skip(index * perPage).Take(perPage).ToArray();
                if (addresses.Length == 0) break;
                var query = $"email:(\"{string.Join("\" OR \"", addresses)}\")";
                var request = new GetUsersRequest { Query = query, SearchEngine = "v3" };
                var paginationInfo = new PaginationInfo(0, perPage, false);
                var retrievedUsers = client.GetAllAsync(request, paginationInfo).Result;
                users.AddRange(retrievedUsers);
                index++;
                Thread.Sleep(600); // new Auth0 rate limits since 12-09-2017:
            }
            memoryCache.Set("auth0Users", users, DateTimeOffset.UtcNow.AddHours(1));
            return users.Where(u => emailAdresses.Contains(u.Email.ToLower())).ToList();
        }

        public static void DeleteAuth0User(IMemoryCache memoryCache, IList<string> emailAdresses)
        {
            var client = GetUserApiClient(memoryCache);
            GetUsersByEmail(memoryCache, emailAdresses)
                .ToList()
                .ForEach(u => client.DeleteAsync(u.UserId).Wait());
        }

        public static void SyncAuth0Users(IRepositoryFactory repositoryFactory, IMemoryCache memCache)
        {
            var userRepository = repositoryFactory.GetRepository<Model.User>();
            var auth0Ids = userRepository.AsQueryable().Select(u => u.IdToken).ToList();
            var name = string.Empty;
            var role = string.Empty;
            var picture = string.Empty;
            var existingUsers = GetAll(memCache);
            foreach (var auth0User in existingUsers.Where(u => !auth0Ids.Contains(u.UserId)))
            {
                try
                {
                    var appMetadata = auth0User.AppMetadata?.ToString();
                    var userMetadata = auth0User.UserMetadata?.ToString();
                    JObject appdataObject = string.IsNullOrEmpty(appMetadata) ? null : JObject.Parse(appMetadata);
                    JObject userdataObject = string.IsNullOrEmpty(userMetadata) ? null : JObject.Parse(userMetadata);
                    role = appdataObject?.SelectToken("roles").FirstOrDefault()?.ToString();
                    name = auth0User.FullName;
                    picture = string.IsNullOrEmpty(auth0User.Picture) || auth0User.Picture.Contains("gravatar")
                        ? userdataObject?.SelectToken("picture")?.ToString()
                        : auth0User.Picture;
                }
                catch (Exception)
                {
                    //Do nothing
                }

                Model.UserType? userType = null;
                if (Enum.TryParse(role, out Model.UserType ut))
                {
                    userType = ut;
                }

                userRepository.AddAsync(new Model.User
                {
                    IdToken = auth0User.UserId,
                    Name = name,
                    Email = auth0User.Email,
                    Picture = picture,
                    UserType = userType
                }).Wait();
            }
        }

        public static Stream LoadCsvWithData(string resourceName)
        {
            var assembly = typeof(GeneralHelper).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception($"Could not load reference json resource ({resourceName}).");
            }
            return stream;
        }
    }
}