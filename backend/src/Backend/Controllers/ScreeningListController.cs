using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Logic.DomainLogic.Commands.ScreeningsList;

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Screeningslist controller
    /// </summary>
    [Route("api/[controller]")]
    public class ScreeningListController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IRepository<ScreeningList> _screeningListRepository;
        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="mediator"></param>
        public ScreeningListController(IRepositoryFactory repositoryFactory, IMediator mediator)
        {
            _screeningListRepository = repositoryFactory.GetRepository<ScreeningList>();
            _mediator = mediator;
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("Post screeningList")]
        public async Task<IActionResult> Post([FromBody] ScreeningList value)
        {
            if (await _screeningListRepository.AddAsync(value) != null)
            {
                return CreatedAtRoute($"Get{typeof(ScreeningList).Name}",
                    new {controller = value.GetType().Name.TrimEnd("Controller"), id = value.Id}, value);
            }
            throw new Exception("Error while adding the object.");
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetScreeningList")]
        [SwaggerOperation("Get screeeninglist")]
        public Task<IActionResult> Get(Guid id) => Task.Run<IActionResult>(() =>
            {
                var screeningList = _screeningListRepository.GetAsync(id);
                if (screeningList == null) return NotFound();
                return Ok(screeningList);
            });

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{id}/FillScreeningList")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> FillScreeningList(Guid id, ICollection<IFormFile> file)
        {
            var screeninglist = _screeningListRepository.GetAsync(id).Result;
            if (screeninglist == null || file?.FirstOrDefault() == null)
                return BadRequest("Unknown wishlistId or no file uploaded");
            var stream = file.First().OpenReadStream();
            var fillScreeningList = new FillScreeningByCsvCommand(stream, id);
            var screeningList = await _mediator.Send(fillScreeningList);
            return Ok(screeningList);
        }

        /// <summary>
        ///     Get list of screeningsitems.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("Get ScreeningItems")]
        public Task<IEnumerable<ScreeningList>> Get() =>
            Task.Run<IEnumerable<ScreeningList>>(() => _screeningListRepository.AsQueryable());
    }
}