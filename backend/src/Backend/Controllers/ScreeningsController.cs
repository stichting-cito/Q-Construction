using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Screening;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.Repository;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Controller for screeningss
    /// </summary>
    [Route("api/[controller]")]
    public class ScreeningsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Screening> _screeningRepository;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="repositoryFactory"></param>
        public ScreeningsController(IMediator mediator, IRepositoryFactory repositoryFactory)
        {
            _mediator = mediator;
            _screeningRepository = repositoryFactory.GetRepository<Screening>();
        }

        /// <summary>
        ///     Get screening
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("Get screenings")]
        public async Task<IEnumerable<Screening>> Get() =>
            await Task.Run<IEnumerable<Screening>>(() => _screeningRepository.AsQueryable());

        /// <summary>
        ///     Get screening by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetScreening")]
        [SwaggerOperation("Get screening")]
        public async Task<IActionResult> Get(Guid id) =>
            await Task.Run<IActionResult>(() =>
            {
                var screening = _screeningRepository.GetAsync(id).Result;
                if (screening == null) return NotFound();
                return Ok(screening);
            });
        
        /// <summary>
        ///     Post screening
        /// </summary>
        /// <param name="screening"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("Add screening")]
        public async Task<IActionResult> Post([FromBody] Screening screening)
        {
            var savedScreening = await _mediator.Send(new AddScreeningCommand { Screening = screening });
            if (savedScreening != null)
                return CreatedAtRoute($"Get{typeof(Screening).Name}",
                    new { controller = screening.GetType().Name.TrimEnd("Controller"), id = screening.Id }, screening);
            throw new Exception("Error while posting object.");
        }

        /// <summary>
        ///     Update screening
        /// </summary>
        /// <param name="id"></param>
        /// <param name="screening"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerOperation("Update screening")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Screening screening)
        {
            if (await _mediator.Send(new UpdateScreeningCommand { Screening = screening }))
                return NoContent();
            throw new Exception("Error while updating the object.");
        }

        /// <summary>
        ///     Delete screening
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [SwaggerOperation("Delete Screening")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _screeningRepository.DeleteAsync(id))
                return NoContent();
            throw new Exception("Error while deleting the object.");
        }
    }
}