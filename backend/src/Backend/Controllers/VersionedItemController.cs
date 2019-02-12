using System;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Controller to get items with version info and more.
    /// </summary>
    [Route("api/[controller]")]
    public class VersionedItemController : Controller
    {
        /// <summary>
        ///     Respository to get the item from storage
        /// </summary>
        private readonly IRepository<VersionedItem> _repository;

        /// <summary>
        ///     Constructor fullitem
        /// </summary>
        /// <param name="repositoryFactory"></param>
        public VersionedItemController(IRepositoryFactory repositoryFactory)
        {
            _repository = repositoryFactory.GetRepository<VersionedItem>();
        }

        /// <summary>
        ///     Get the item with version info
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/domains/{id}
        [HttpGet("{id}")]
        [SwaggerOperation("Get item with history")]
        public async Task<IActionResult> Get(Guid id) =>
             await Task.Run<IActionResult>(() =>
            {
                var fullItem = _repository.GetAsync(id).Result;
                if (fullItem == null) return NotFound();
                return Ok(fullItem);
            });
    }
}