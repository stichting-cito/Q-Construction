using System;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Stats per wishlist resource.
    /// </summary>
    [Authorize(Policy = "Manager")]
    [Route("api/[controller]")]
    public class StatsPerWishlistController : Controller
    {
        private readonly IRepository<StatsPerWishlist> _statsPerWishlistRepository;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        public StatsPerWishlistController(IRepositoryFactory repositoryFactory)
        {
            _statsPerWishlistRepository = repositoryFactory.GetRepository<StatsPerWishlist>();
        }

        /// <summary>
        ///     Get the percentage of accepted items for this wishlist.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerOperation("Get all statisics")]
        public async Task<IActionResult> Get(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats);
        }

        /// <summary>
        ///     Get the percentage of accepted items for this wishlist.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/percentageaccepted")]
        [SwaggerOperation("Get the percentage of accepted items for this wishlist.")]
        public async Task<IActionResult> GetPercentageAccepted(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.PercentageAccepted);
        }

        /// <summary>
        ///     Get the percentage of rejected items for this wishlist.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/percentagemortality")]
        [SwaggerOperation("Get the percentage of rejected items for this wishlist.")]
        public async Task<IActionResult> GetPercentageMortality(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.PercentageMortality);
        }

        /// <summary>
        ///     Get the mean number of iterations needed per accepted item.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/meanreviewiterations")]
        [SwaggerOperation("Get the mean number of iterations needed per accepted item.")]
        public async Task<IActionResult> GetMeanReviewIterations(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.MeanReviewIterations);
        }

        /// <summary>
        ///     Get the stats per domain (percentage accepted, mean no of iterations) this wishlist.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/domainstats")]
        [SwaggerOperation("Get the stats per domain (percentage accepted, mean no of iterations) this wishlist.")]
        public async Task<IActionResult> GetDomainStats(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.StatsPerDomain);
        }

        /// <summary>
        ///     Get the stats per user (constructors and test experts).
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/userstats")]
        [SwaggerOperation("Get the stats per user (constructors and test experts).")]
        public async Task<IActionResult> GetUserStats(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.StatsPerUser);
        }

        /// <summary>
        ///     Get the item deadlines with counts.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/itemdeadlineswithcounts")]
        [SwaggerOperation("Get the item deadlines with counts.")]
        public async Task<IActionResult> GetItemDeadlinesWithCounts(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.ItemDeadlinesWithCounts);
        }

        /// <summary>
        ///     Get the cumulative number of items accepted per day.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/itemsacceptedperdaycumulative")]
        [SwaggerOperation("Get the cumulative number of items accepted per day.")]
        public async Task<IActionResult> GetItemsAcceptedPerDayCumulative(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.ItemsAcceptedPerDayCumulative);
        }

        /// <summary>
        ///     Get screening items usage counts.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/screeningitemstats")]
        [SwaggerOperation("Get screening items usage counts.")]
        public async Task<IActionResult> GetScreeningItemStats(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.ScreeningItemStats);
        }

        /// <summary>
        ///     Get the number of items that are accepted.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/itemsacceptedcount")]
        [SwaggerOperation("Get the number of items that are accepted.")]
        public async Task<IActionResult> GetItemsAcceptedCount(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.ItemsAcceptedCount);
        }

        /// <summary>
        ///     Get the number of items that are in the review cycle (ready-for-review, in-review or needs-work).
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/itemsinreviewcount")]
        [SwaggerOperation(
            "Get the number of items that are in the review cycle (ready-for-review, in-review or needs-work).")]
        public async Task<IActionResult> GetItemInReviewCount(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.ItemsInReviewCount);
        }

        /// <summary>
        ///     Get the number of items that are left to be made.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/itemstodocount")]
        [SwaggerOperation("Get the number of items that are left to be made.")]
        public async Task<IActionResult> GetItemsTodoCount(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.ItemsTodoCount);
        }

        /// <summary>
        ///     Get the Total number of items to be made.
        /// </summary>
        /// <param name="id">Wishlist id</param>
        /// <returns></returns>
        [HttpGet("{id}/itemtargetcount")]
        [SwaggerOperation("Get the Total number of items to be made.")]
        public async Task<IActionResult> GetItemTargetCount(Guid id)
        {
            var stats = await _statsPerWishlistRepository.GetAsync(id);
            if (stats == null) return NotFound();
            return Ok(stats.ItemTargetCount);
        }
    }
}