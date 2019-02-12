using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.Repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Learing objective controller
    /// </summary>
    [Route("api/[controller]")]
    public class LearningObjectivesController : Controller
    {
        private readonly IRepository<LearningObjective> _learningObjectiveRepository;
        private readonly ILoggedInUserProvider _loggedInUserProvider;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="loggedInUserProvider"></param>
        public LearningObjectivesController(IRepositoryFactory repositoryFactory, ILoggedInUserProvider loggedInUserProvider)
        {
            _learningObjectiveRepository = repositoryFactory.GetRepository<LearningObjective>();
            _loggedInUserProvider = loggedInUserProvider;
        }

        /// <summary>
        ///     Get learningObjectives
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("Get LearningObjectives")]
        public Task<IEnumerable<LearningObjective>> Get() =>
            Task.Run(() => _learningObjectiveRepository.AsQueryable().AsEnumerable());

        /// <summary>
        ///     Get learningObjective
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetLearningObjective")]
        [SwaggerOperation("Get LearningObjective")]
        public Task<IActionResult> Get(Guid id) => Task.Run<IActionResult>(() =>
            {
                var learningObjective = _learningObjectiveRepository.GetAsync(id).Result;
                if (learningObjective == null) return NotFound();
                return Ok(learningObjective);
            });

        /// <summary>
        ///     Add LearningObjective
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("Post LearningObjective")]
        public async Task<IActionResult> Post([FromBody] LearningObjective value)
        {
            if (await _learningObjectiveRepository.AddAsync(value) != null)
            {
                return CreatedAtRoute($"Get{typeof(LearningObjective).Name}",
                    new { controller = value.GetType().Name.TrimEnd("Controller"), id = value.Id }, value);
            }
            throw new Exception("Error while adding the object.");
        }

        /// <summary>
        ///     Update learning objective
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerOperation("Put LearningObjective")]
        public async Task<IActionResult> Put(Guid id, [FromBody] LearningObjective value)
        {
            if (await _learningObjectiveRepository.UpdateAsync(value))
            {
                return NoContent();
            }
            throw new Exception("Error while updating the object.");
        }

        /// <summary>
        ///     Delete learning objective
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [SwaggerOperation("Delete LearningObjective")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _learningObjectiveRepository.DeleteAsync(id))
            {
                return NoContent();
            }
            throw new Exception("Error while deleting the object.");
        }
    }
}