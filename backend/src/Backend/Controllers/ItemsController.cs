using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Backend.Exports.QTI;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Item;
using Citolab.QConstruction.Logic.DomainLogic.Queries;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Item controller
    /// </summary>
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ViewRender _renderer;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly string _wwwRootPath;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="env"></param>
        /// <param name="repositoryFactory"></param>
        /// <param name="renderer"></param>
        public ItemsController(IMediator mediator, IHostingEnvironment env, IRepositoryFactory repositoryFactory,ViewRender renderer)
        {
            _renderer = renderer;
            _mediator = mediator;
            _repositoryFactory = repositoryFactory;
            _wwwRootPath = !string.IsNullOrWhiteSpace(env.WebRootPath)
                ? env.WebRootPath
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        /// <summary>
        ///     Get items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("Get items")]
        public async Task<IEnumerable<Item>> Get()
        {
            return await Task.Run<IEnumerable<Item>>(() =>
            {
                var itemRepository = _repositoryFactory.GetRepository<Item>();
                return itemRepository.AsQueryable();
            });
        }

        /// <summary>
        ///     Get item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetItem")]
        [SwaggerOperation("Get Item")]
        public async Task<IActionResult> Get(Guid id)
        {
            return await Task.Run<IActionResult>(() =>
            {
                var itemRepository = _repositoryFactory.GetRepository<Item>();
                var item = itemRepository.GetAsync(id).Result;
                if (item == null) return NotFound();
                return Ok(item);
            });
        }

        /// <summary>
        ///     Add item
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("Post Item")]
        public async Task<IActionResult> Post([FromBody] Item value)
        {
            value.Version = 1;
            if (value.ItemStatus != ItemStatus.Draft)
                return new BadRequestObjectResult("Item not valid, the item status should be draft");
            var savedItem = await _mediator.Send(new AddItemCommand {Item = value});
            return CreatedAtRoute($"Get{typeof(Item).Name}",
                new {controller = value.GetType().Name.TrimEnd("Controller"), id = savedItem.Id}, savedItem);
        }

        /// <summary>
        ///     Update items
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerOperation("Put Item")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Item value)
        {
            // Get current item
            var itemRepository = _repositoryFactory.GetRepository<Item>();
            var item = itemRepository.GetAsync(id).Result;
            if (item == null) return NotFound();
            // State transitions
            // If the current Item is in draft, and you want to go to the Ready for review state.
            if (value.ItemStatus != item.ItemStatus)
            {
                if (item.ItemStatus == ItemStatus.Draft &&
                    !(value.ItemStatus == ItemStatus.ReadyForReview || value.ItemStatus == ItemStatus.Deleted))
                    return BadRequest("Itemstatus not valid, can only go from draft to ready-for-review ");
            }
            else if (item.ItemStatus != ItemStatus.Draft && item.ItemStatus != ItemStatus.NeedsWork &&
                     (value.BodyText == "" || value.Key == ""))
            {
                return BadRequest("Item not valid, bodytext or key is empty");
            }
            var result = await _mediator.Send(new UpdateItemCommand {Item = value});
            if (!result) throw new Exception($"Error while updating item: {item.Id}");
            return NoContent();
        }

        #region reviews

        /// <summary>
        ///     GetScreenings for item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet("{itemId:Guid}/screenings")]
        [SwaggerOperation("get Screening for item")]
        public IActionResult GetScreeningsForItem(Guid itemId)
        {
            var itemRepository = _repositoryFactory.GetRepository<Item>();
            var screeningRepository = _repositoryFactory.GetRepository<Screening>();
            var item = itemRepository.GetAsync(itemId).Result;
            if (item == null) return NotFound();
            return Ok(screeningRepository.AsQueryable().Where(r => r.ItemId == itemId));
        }


        /// <summary>
        ///     Get screening by id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet("{itemId:Guid}/screenings/latest")]
        [SwaggerOperation("Get latest screening by ItemId")]
        public async Task<Screening> GetScreening(Guid itemId)
        {
            return await Task.Run(() =>
            {
                var itemRepository = _repositoryFactory.GetRepository<Item>();
                var screeningRepository = _repositoryFactory.GetRepository<Screening>();
                var item = itemRepository.GetAsync(itemId).Result;
                return item?.LatestScreeningId == null
                    ? null
                    : screeningRepository.GetAsync(item.LatestScreeningId.Value).Result;
            });
        }

        /// <summary>
        ///     Get all accepted items in wishlist in QTI format
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/qtipackage")]
        [SwaggerOperation("Get qti package by itemId")]
        public FileStreamResult GetQtiPackageByWishListId(Guid id)
        {
            var itemRepository = _repositoryFactory.GetRepository<Item>();
            var wishListRepos = _repositoryFactory.GetRepository<Wishlist>();
            HttpContext.Response.ContentType = "application/zip";
            var item = itemRepository.GetAsync(id).Result;
            if (item == null) throw new Exception("Item not found.");
            var testName = wishListRepos.GetAsync(item.WishListId).Result?.Title;
            var qtiConverter = new QtiConverter(_wwwRootPath, testName, _renderer, "22", _repositoryFactory);
            var zipFile = qtiConverter.ConvertItems(new List<Item> { item }, _wwwRootPath);
            var fileStream = new FileStream(zipFile, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(zipFile)}");
            return File(fileStream, "application/zip");
        }

        [HttpGet("{id}/qtipackage21")]
        [SwaggerOperation("Get qti package by itemId")]
        public FileStreamResult GetQtiPackage21ByWishListId(Guid id)
        {
            var itemRepository = _repositoryFactory.GetRepository<Item>();
            var wishListRepos = _repositoryFactory.GetRepository<Wishlist>();
            HttpContext.Response.ContentType = "application/zip";
            var item = itemRepository.GetAsync(id).Result;
            if (item == null) throw new Exception("Item not found.");
            var testName = wishListRepos.GetAsync(item.WishListId).Result?.Title;
            var qtiConverter = new QtiConverter(_wwwRootPath, testName, _renderer, "21", _repositoryFactory);
            var zipFile = qtiConverter.ConvertItems(new List<Item> { item }, _wwwRootPath);
            var fileStream = new FileStream(zipFile, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(zipFile)}");
            return File(fileStream, "application/zip");
        }

        #endregion

        #region  Item attachments

        [HttpPost("{itemId:Guid}/attachment")]
        [SwaggerOperation("addattachmenttoitem")]
        public async Task<IActionResult> AddAttachmentToItem(Guid itemId)
        {
            if (Request.Form?.Files?.Count > 1)
            {
                return BadRequest("Only one attachment allowed per request.");
            }
            var file = Request.Form?.Files?[0];
            if (file != null && file.Length > 0)
            {
                var attachmentId = await _mediator.Send(new AddAttachmentToItemCommand(itemId, file));
                var currentUri = $"{Request.Path}/{attachmentId}/{file.FileName}";
                return Created(currentUri, new JsonResult(new {AttachmentId = attachmentId}));
            }
            return BadRequest("No files found to attach.");
        }

        [HttpGet("{itemId:Guid}/attachment/{attachmentId:Guid}/{fileName?}")]
        [SwaggerOperation("Get an attachment for an item")]
        public async Task<FileContentResult> GetAttachmentForItem(Guid itemId, Guid attachmentId, string fileName = "")
        {
            var attachment = await _mediator.Send(new GetAttachmentForItemQuery(itemId, attachmentId, fileName));
            return new FileContentResult(attachment.Bytes, attachment.ContentType);
        }

        [HttpGet("{itemId:Guid}/attachment/{attachmentId:Guid}/thumbnail/{fileName?}")]
        [SwaggerOperation("Get the thumbnail of an attachment for an item")]
        public async Task<FileContentResult> GetThumbnailOfAttachmentForItem(Guid itemId, Guid attachmentId,
            string fileName = "")
        {
            var attachment = await _mediator.Send(new GetAttachmentForItemQuery(itemId, attachmentId, fileName));
            return new FileContentResult(attachment.ThumbnailBytes, "image/jpeg");
        }

        [HttpDelete("{itemId:guid}/attachment/{attachmentId:Guid}")]
        [SwaggerOperation("deleteattachmentforitem")]
        public async Task<IActionResult> DeleteAttachmentForItem(Guid itemId, Guid attachmentId)
        {
            if (await _mediator.Send(new DeleteAttachmentForItemCommand(itemId, attachmentId)))
            {
                return NoContent();
            }
            throw new Exception("Error while deleting the object.");
        }

        #endregion
    }
}