using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Item summary class. Get the summary of items for grids
    /// </summary>
    [Route("api/[controller]")]
    public class ItemSummariesController : Controller
    {
        private readonly IRepository<ItemSummary> _itemSummaryRepository;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        public ItemSummariesController(IRepositoryFactory repositoryFactory)
        {
            _itemSummaryRepository = repositoryFactory.GetRepository<ItemSummary>();
        }

        /// <summary>
        ///     Get items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("Get item summaries")]
        public async Task<IEnumerable<ItemSummary>> Get() =>
            await Task.Run<IEnumerable<ItemSummary>>(() => _itemSummaryRepository.AsQueryable());

        /// <summary>
        ///     Get item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetItemSummary")]
        [SwaggerOperation("Get item summary")]
        public async Task<IActionResult> Get(Guid id) =>
            await Task.Run<IActionResult>(() =>
            {
                var itemSummary = _itemSummaryRepository.GetAsync(id).Result;
                if (itemSummary == null) return NotFound();
                return Ok(itemSummary);
            });

        /// <summary>
        ///     Get item status by state
        /// </summary>
        /// <param name="wishlistId"></param>
        /// <param name="itemStatus"></param>
        /// <returns></returns>
        [HttpGet("{wishlistId}/bystatus/{itemStatus}")]
        [SwaggerOperation("Get items by state")]
        public async Task<IEnumerable<ItemSummary>> ItemSummariesByState(Guid wishlistId, ItemStatus itemStatus) =>
            await Task.Run<IEnumerable<ItemSummary>>(
                () =>
                {
                    return
                        _itemSummaryRepository.AsQueryable()
                            .Where(i => i.WishListId == wishlistId && i.ItemStatus == itemStatus)
                            .OrderByDescending(i => i.Deadline);
                });
    }
}