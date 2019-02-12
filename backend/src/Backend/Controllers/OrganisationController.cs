using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Citolab.QConstruction.Logic.DomainLogic;
using Citolab.QConstruction.Logic.DomainLogic.Commands.ScreeningsList;
using Citolab.QConstruction.Logic.DomainLogic.Commands.User;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Wishlist;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;

namespace Citolab.QConstruction.Backend.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    public class OrganisationController : Controller
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IMediator _mediator;

        public OrganisationController(IRepositoryFactory repositoryFactory,
            IMediator mediator, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _repositoryFactory = repositoryFactory;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [SwaggerOperation("Get all organisations")]
        public ActionResult<IList<string>> Get()
        {
            var userRepos = _repositoryFactory.GetRepository<User>();
            var orgs = userRepos.AsQueryable()
                .Where(u => !string.IsNullOrEmpty(u.Organisation))
                .Select(u => u.Organisation)
                .ToList()
                .Distinct()
                .ToList();
            return Ok(orgs);
        }

        [HttpDelete("{name}")]
        [SwaggerOperation("Delete an organisation")]
        public ActionResult<string> Delete(string name)
        {
            var wishlistRepos = _repositoryFactory.GetRepository<Wishlist>();
            var usersRepos = _repositoryFactory.GetRepository<User>();
            var wishlistItemRepos = _repositoryFactory.GetRepository<WishlistItem>();
            var screeningRepos = _repositoryFactory.GetRepository<Screening>();
            var itemSummaryRepos = _repositoryFactory.GetRepository<ItemSummary>();
            var statsPerWishlistRepos = _repositoryFactory.GetRepository<StatsPerWishlist>();
            var itemRepos = _repositoryFactory.GetRepository<Item>();

            var users = usersRepos
                .AsQueryable()
                .Where(u => u.Organisation == name)
                .ToList();
            // Delete Auth0 users
            GeneralHelper.DeleteAuth0User(_memoryCache,
                users.Select(u => u.Email).ToList());
            var wishlist = wishlistRepos
                .AsQueryable().FirstOrDefault(w => w.OrganisationName == name);
            if (wishlist == null) throw new DomainException("wishlist not found", true);
            // Delete users
            users.ForEach(u => usersRepos.DeleteAsync(u.Id).Wait());

            // Delete items, item summaries and screenings
            var items = itemRepos
                .AsQueryable()
                .Where(i => i.WishListId == wishlist.Id)
                .ToList();

            items.ForEach(i =>
            {
                screeningRepos.AsQueryable()
                    .Where(s => s.ItemId == i.Id)
                    .ToList()
                    .ForEach(s => screeningRepos.DeleteAsync(s.Id).Wait());
            });

            items.ForEach(i => itemRepos.DeleteAsync(i.Id).Wait());
            itemSummaryRepos.AsQueryable().Where(i => i.WishListId == wishlist.Id).ToList()
                .ForEach(i => itemSummaryRepos.DeleteAsync(i.Id).Wait());
            // Delete wishlist stats
            var stats = statsPerWishlistRepos.GetAsync(wishlist.Id).Result;
            if (stats != null)
            {
                statsPerWishlistRepos.DeleteAsync(wishlist.Id).Wait();
            }
            
            // Delete wishlist items
            wishlistItemRepos
                .AsQueryable()
                .Where(wi => wi.WishlistId == wishlist.Id)
                .ToList()
                .ForEach(wi => wishlistItemRepos.DeleteAsync(wi.Id).Wait());
            // Delete screening list
            var screeningsListRepos = _repositoryFactory.GetRepository<ScreeningList>();
            var list = screeningsListRepos.GetAsync(wishlist.ScreeningsListId).Result;
            if (list != null)
            {
                screeningsListRepos.DeleteAsync(list.Id).Wait();
            }
            
            // Delete wishlist
            wishlistRepos.DeleteAsync(wishlist.Id).Wait();
            return Ok("Deleted");
        }

        [HttpPost]
        [SwaggerOperation("Add a new organisation")]
        public ActionResult<string> AddDemoOrganisationNl([FromBody] Organisation value) =>
            Ok(CreateDemoEnvironmentForOrganisation(value.Language.ToUpperInvariant(), value.Name));

        private string CreateDemoEnvironmentForOrganisation(string language, string name)
        {
            // For organisations that request a demo, the following is created
            // - Default screening list
            // - Wishlist filled based
            // - Add 1 manager and 2 testexperts and 2 constructors
            name = new Regex("[^a-zA-Z0-9]").Replace(name, "");
            var screeningsList =
                _mediator.Send(
                    new FillScreeningByCsvCommand(
                        $"Citolab.QConstruction.Logic.Data.SCREENINGS_LIST_{language}.csv",
                        $"Default {language}")).Result;

            var wishlist = AddWishlist($"DEMO {name}", $"Citolab.QConstruction.Logic.Data.DEMO_Q_LO_{language}.csv",
                screeningsList.Id);
            wishlist.DisabledItemTypes = new List<ItemType>
            {
                ItemType.GraphicGapMatch,
                ItemType.Hotspot
            };
            wishlist.OrganisationName = name;
            _repositoryFactory.GetRepository<Wishlist>().UpdateAsync(wishlist).Wait();
            var wishlistKv = new KeyValue {Id = wishlist.Id, Value = wishlist.Title};
            var users = new List<User>
            {
                new User
                {
                    UserType = UserType.RestrictedManager,
                    AllowedWishlists = new List<KeyValue> {wishlistKv}.ToArray(),
                    Email = $"M_{name}@citolab.nl",
                    Organisation = name,
                    SelectedWishlist = wishlistKv,
                    Name = "Manager",
                    Password = PasswordHelper.Generate(8)
                },
                new User
                {
                    UserType = UserType.Toetsdeskundige,
                    AllowedWishlists = new List<KeyValue> {wishlistKv}.ToArray(),
                    Email = $"TE_1_{name}@citolab.nl",
                    Organisation = name,
                    SelectedWishlist = wishlistKv,
                    Name = language == "nl" ? "Toetsdeskundige 1" : "Test expert 1",
                    Password = PasswordHelper.Generate(8)
                },
                new User
                {
                    UserType = UserType.Toetsdeskundige,
                    AllowedWishlists = new List<KeyValue> {wishlistKv}.ToArray(),
                    Email = $"TE_2_{name}@citolab.nl",
                    Organisation = name,
                    SelectedWishlist = wishlistKv,
                    Name = language == "nl" ? "Toetsdeskundige 2" : "Test expert 2",
                    Password = PasswordHelper.Generate(8)
                },
                new User
                {
                    UserType = UserType.Constructeur,
                    AllowedWishlists = new List<KeyValue> {wishlistKv}.ToArray(),
                    Email = $"C_1_{name}@citolab.nl",
                    Organisation = name,
                    SelectedWishlist = wishlistKv,
                    Name = language == "nl" ? "Constructeur 1" : "Item author 1",
                    Password = PasswordHelper.Generate(8)
                },
                new User
                {
                    UserType = UserType.Constructeur,
                    AllowedWishlists = new List<KeyValue> {wishlistKv}.ToArray(),
                    Email = $"C_2_{name}@citolab.nl",
                    Organisation = name,
                    SelectedWishlist = wishlistKv,
                    Name = language == "nl" ? "Constructeur 2" : "Item author 2",
                    Password = PasswordHelper.Generate(8)
                }
            };

            var result = $"Created demo set for {name}. {Environment.NewLine}{Environment.NewLine}";
            result = string.Concat(result,
                string.Join(Environment.NewLine,
                    users.Select(u => $"{GetTypeName(u.UserType)} email: {u.Email} password: {u.Password}")));
            users.ForEach(u => _mediator.Send(new AddUserCommand(u)).Wait());

            return result;
        }

        private static string GetTypeName(UserType? userType)
        {
            if (!userType.HasValue) return "unknown";
            switch (userType.Value)
            {
                case UserType.Admin:
                    return "administrator";
                case UserType.Manager:
                case UserType.RestrictedManager:
                    return "manager";
                case UserType.Toetsdeskundige:
                    return "test expert";
                case UserType.Constructeur:
                    return "item author";
                default: return "unknown";
            }
        }


        private Wishlist AddWishlist(string title, string resourceName, Guid screeningListId)
        {
            var wishlist =
                _mediator.Send(
                    new AddWishListCommand(new Wishlist
                    {
                        Title = title,
                        ScreeningsListId = screeningListId,
                        Created = DateTime.Now
                    })).Result;
            return _mediator.Send(new FillWishlistByCsvCommand(resourceName, wishlist.Id, 120)).Result;
        }
    }
}