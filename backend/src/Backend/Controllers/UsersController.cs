using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Auth0.Core.Collections;
using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.DomainLogic.Commands.User;
using Citolab.QConstruction.Logic.DomainLogic.Queries;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.Repository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Controller for users
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILoggedInUserProvider _loggedInUserProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserItemStatusCount> _userStatusCountRepository;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="mediator"></param>
        /// <param name="loggedInUserProvider"></param>
        /// <param name="memoryCache"></param>
        public UsersController(IRepositoryFactory repositoryFactory, IMediator mediator, ILoggedInUserProvider loggedInUserProvider,
            IMemoryCache memoryCache)
        {
            _userRepository = repositoryFactory.GetRepository<User>();
            _userStatusCountRepository = repositoryFactory.GetRepository<UserItemStatusCount>();
            _repositoryFactory = repositoryFactory;
            _mediator = mediator;
            _loggedInUserProvider = loggedInUserProvider;
            _memoryCache = memoryCache;
        }

        /// <summary>
        ///     Get all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("Get Users")]
        public ActionResult<IEnumerable<User>> Get()
        {
            var users = _userRepository.AsQueryable().ToList();
            var loggedInUser = _loggedInUserProvider.GetUserId().HasValue ?
                GetUser(_loggedInUserProvider.GetUserId().Value) : null;
            // CHECK IF
            // USER IS ADMIN OR MANAGER
            if (loggedInUser.UserType == UserType.Admin ||
                loggedInUser.UserType == UserType.Manager)
            {
                var auth0Ids = users.Select(u => u.IdToken).ToList();
                foreach (var auth0User in GeneralHelper.GetAll(_memoryCache).Where(u => !auth0Ids.Contains(u.UserId)))
                {
                    var newUser = _userRepository.AddAsync(new User
                    {
                        IdToken = auth0User.UserId,
                        Name = $"{auth0User.FirstName} {auth0User.LastName}",
                        Email = auth0User.Email
                    }).Result;
                    users.Add(newUser);
                }
                return Ok(users);
            } else
            {
                return Unauthorized();
            }

        }

        private bool AllowedWishlists(IEnumerable<Guid> assigned, IEnumerable<Guid> permissions)
        {
            return assigned.Intersect(permissions).Count() == assigned.Count();
        }

        /// <summary>
        ///     Get user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetUser")]
        [SwaggerOperation("Get User")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _mediator.Send(new UserQuery(id));
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        ///     Get user
        /// </summary>
        /// <returns></returns>
        [HttpGet("loggedinuser", Name = "loggedinuser")]
        [SwaggerOperation("Get Logged in user")]
        public Task<IActionResult> GetLoggedInUser() => Task.Run<IActionResult>(() =>
        {
            var userId = _loggedInUserProvider.GetUserId();
            if (userId == null) return NotFound();
            var user = _mediator.Send(new UserQuery(userId.Value)).Result;
            if (user == null) return NotFound();
            return Ok(user);
        });

        /// <summary>
        ///     Add user
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("Post User")]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Post([FromBody] User value)
        {
            var user = await _mediator.Send(new AddUserCommand(value));
            if (user != null)
            {
                return CreatedAtRoute($"Get{typeof(User).Name}",
                    new { controller = value.GetType().Name.TrimEnd("Controller"), id = user.Id }, user);
            }
            throw new Exception("Error while posting object.");
        }

        /// <summary>
        ///     Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerOperation("Put User")]
        public async Task<IActionResult> Put(Guid id, [FromBody] User value)
        {
            if (await _userRepository.UpdateAsync(value))
            {
                return NoContent();
            }
            throw new Exception("Error while updating the object.");
        }

        /// <summary>
        ///     Set permissions for user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wishlistIds"></param>
        /// <returns></returns>
        [HttpPut("{id}/updatepermissions")]
        [SwaggerOperation("Put User")]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> PutRights(Guid id, [FromBody] IEnumerable<Guid> wishlistIds)
        {
            var user = _userRepository.GetAsync(id).Result;
            var wishlists =
                _repositoryFactory.GetRepository<Wishlist>().AsQueryable().Where(w => wishlistIds.Contains(w.Id));
            var fw = wishlists.FirstOrDefault();
            if (user.SelectedWishlist != null && !wishlistIds.Contains(user.SelectedWishlist.Id))
                user.SelectedWishlist = fw != null ? new KeyValue { Id = fw.Id, Value = fw.Title } : null;
            user.AllowedWishlists = wishlists.Select(w => new KeyValue { Id = w.Id, Value = w.Title }).ToArray();
            if (await _userRepository.UpdateAsync(user))
            {
                return NoContent();
            }
            throw new Exception("Error while updating the object.");
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wishlistId"></param>
        /// <returns></returns>
        [HttpPost("{id}/{wishlistId}/updateselectedwishlist")]
        [SwaggerOperation("Put User")]
        public async Task<IActionResult> PutSelectedWishList(Guid id, Guid wishlistId)
        {
            if (!IsAllowed(id)) return Unauthorized();
            var user = await _mediator.Send(new UserQuery(id));
            if (user == null) return NotFound();
            var wishlist = _repositoryFactory.GetRepository<Wishlist>().GetAsync(wishlistId).Result;
            user.SelectedWishlist = new KeyValue { Id = wishlist.Id, Value = wishlist.Title };
            if (await _userRepository.UpdateAsync(user)) return NoContent();
            throw new Exception("Error while updating the object.");
        }

        /// <summary>
        ///     Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [SwaggerOperation("Delete User")]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _userRepository.DeleteAsync(id))
            {
                return NoContent();
            }
            throw new Exception("Error while deleting the object.");
        }

        /// <summary>
        ///     Get ItemStatus for user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wishlistId"></param>
        /// <returns></returns>
        [HttpGet("{id}/{wishlistId}/itemstatuscounts")]
        [SwaggerOperation("Get item count per state")]
        public async Task<IActionResult> GetItemStatusCount(Guid id, Guid wishlistId)
        {
            return await Task.Run<IActionResult>(() =>
            {
                if (!IsAllowed(id)) return Unauthorized();
                return
                    Ok(_userStatusCountRepository.AsQueryable().Where(i => i.UserId == id && i.WishlistId == wishlistId));
            });
        }

        /// <summary>
        ///     Get list of item summaries with the specified status for this user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wishlistId"></param>
        /// <param name="itemStatus"></param>
        /// <returns></returns>
        [HttpGet("{id}/{wishlistId}/itemsummariesbystatus/{itemStatus}")]
        [SwaggerOperation("Get item summaries with the specified state for this user.")]
        public async Task<IActionResult> GetItemState(Guid id, Guid wishlistId, ItemStatus itemStatus)
        {
            return await Task.Run<IActionResult>(() =>
            {
                var user = GetUser(id);
                if (user == null) return NotFound();
                if (!IsAllowed(user)) return Unauthorized();
                var itemSummaryRepository = _repositoryFactory.GetRepository<ItemSummary>();
                var result =
                    itemSummaryRepository.AsQueryable()
                        .Where(i => i.CreatedByUserId == id && i.ItemStatus == itemStatus && i.WishListId == wishlistId)
                        .ToList();
                return Ok(result);
            });
        }

        /// <summary>
        ///     Get item summaries with the specified state and latest screening by this user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wishlistId"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        [HttpGet("{id}/{wishlistId}/itemsummariesbylatestscreeningandstates/{states}")]
        [SwaggerOperation("Get item summaries with the specified state and latest screening by this user.")]
        public async Task<IActionResult> GetItemSummariesByLatestScreeningAndStates(Guid id, Guid wishlistId,
            string states)
        {
            return await Task.Run<IActionResult>(() =>
            {
                var user = GetUser(id);
                if (user == null) return NotFound();
                if (!IsAllowed(user)) return Unauthorized();
                var itemSummaryRepository = _repositoryFactory.GetRepository<ItemSummary>();
                //var screeningsRepository = _repositoryFactory.GetRepository<Screening>(_memoryCache, _loggerFactory);
                var stateList =
                    states.Split(Convert.ToChar("|"))
                        .Select(t => Convert.ToInt16(t))
                        .Select(i => (ItemStatus)i)
                        .ToList();
                var result = itemSummaryRepository.AsQueryable()
                    .Where(i => i.LatestScreeningId != null && i.WishListId == wishlistId
                                && stateList.Contains(i.ItemStatus)
                                && i.LatestScreeningAuthorId == user.Id)
                    .OrderByDescending(i => i.LastModified).ToList();
                return Ok(result);
            });
        }

        /// <summary>
        ///     Get item summaries of items that are in review or are ready for review by this user (test expert).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wishlistId"></param>
        /// <returns></returns>
        [HttpGet("{id}/{wishlistId}/itemsummariestodofortestexpert")]
        [SwaggerOperation(
            "Get item summaries of items that are in review or are ready for review by this user (test expert).")]
        public Task<IActionResult> GetItemSummariesToDoForTestExpert(Guid id, Guid wishlistId) => Task.Run<IActionResult>(() =>
            {
                var user = GetUser(id);
                if (user == null) return NotFound();
                if (!IsAllowed(user)) return Unauthorized();
                var itemSummaryRepository = _repositoryFactory.GetRepository<ItemSummary>();
                var result = itemSummaryRepository.AsQueryable()
                    .Where(i => i.WishListId == wishlistId && ((i.ItemStatus == ItemStatus.ReadyForReview && !i.LatestScreeningAuthorId.HasValue) ||
                                                                ((i.ItemStatus == ItemStatus.InReview || i.ItemStatus == ItemStatus.ReadyForReview)
                                                                   && i.LatestScreeningAuthorId.HasValue
                                                                   && i.LatestScreeningAuthorId == user.Id)))
                    .ToList();
                return Ok(result);
            });


        #region Private Methods

        private User GetUser(Guid id) =>
             _repositoryFactory.GetRepository<User>().GetAsync(id).Result;


        private bool IsAllowed(User user) =>
            user.UserType == UserType.Toetsdeskundige ||
            user.UserType == UserType.Constructeur && user.Id == _loggedInUserProvider.GetUserId();

        private bool IsAllowed(Guid id) => id == _loggedInUserProvider.GetUserId();


        #endregion
    }
}