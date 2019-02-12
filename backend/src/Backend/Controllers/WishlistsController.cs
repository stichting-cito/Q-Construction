using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.Exports.MsWord;
using Citolab.QConstruction.Backend.Exports.QTI;
using Citolab.QConstruction.Backend.Exports.Questify;
using Citolab.QConstruction.Logic.DomainLogic;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Wishlist;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.Repository;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Citolab.QConstruction.Backend.Controllers
{
    /// <summary>
    ///     Controller for wishlist
    /// </summary>
    [Route("api/[controller]")]
    public class WishlistsController : Controller
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<ItemSummary> _itemSummaryRepository;
        private readonly IRepository<LearningObjective> _learningObjectiveRepository;
        private readonly IMediator _mediator;
        private readonly ViewRender _renderer;
        private readonly IRepository<ScreeningList> _screeningListRepository;
        private readonly IRepository<WishlistItem> _wishlistItemRepository;
        private readonly IRepository<Wishlist> _wishlistRepository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly string _wwwRootPath;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="mediator"></param>
        /// <param name="env"></param>
        /// <param name="renderer"></param>
        public WishlistsController(IRepositoryFactory repositoryFactory, IMediator mediator, IHostingEnvironment env,
            ViewRender renderer)
        {
            _repositoryFactory = repositoryFactory;
            _wishlistRepository = repositoryFactory.GetRepository<Wishlist>();
            repositoryFactory.GetRepository<User>();
            _wishlistItemRepository = repositoryFactory.GetRepository<WishlistItem>();
            _itemSummaryRepository = repositoryFactory.GetRepository<ItemSummary>();
            _itemRepository = repositoryFactory.GetRepository<Item>();
            _screeningListRepository = repositoryFactory.GetRepository<ScreeningList>();
            _learningObjectiveRepository = repositoryFactory.GetRepository<LearningObjective>();
            _wwwRootPath = !string.IsNullOrWhiteSpace(env.WebRootPath)
                ? env.WebRootPath
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _mediator = mediator;
            _renderer = renderer;
        }

        /// <summary>
        ///     Get wishlists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("Get Wishlists")]
        public Task<IEnumerable<Wishlist>> Get() =>
            Task.Run<IEnumerable<Wishlist>>(() => _wishlistRepository.AsQueryable());


        [HttpGet("{id}/disabledItemTypes")]
        [SwaggerOperation("Get Disabled ItemTypes")]
        public Task<IActionResult> GetDisabledItemTypes(Guid id) =>
            Task.Run<IActionResult>(() =>
            {
                var wishlist = _wishlistRepository.GetAsync(id).Result;
                if (wishlist == null) return NotFound();
                return Ok(wishlist.DisabledItemTypes);
            });

        /// <summary>
        ///     Get wishlist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetWishlist")]
        [SwaggerOperation("Get Wishlist")]
        public async Task<IActionResult> Get(Guid id)
        {
            return await Task.Run<IActionResult>(() =>
            {
                var wishlist = _wishlistRepository.GetAsync(id).Result;
                if (wishlist == null) return NotFound();
                return Ok(wishlist);
            });
        }

        /// <summary>
        ///     Post wishlist
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("Post Wishlist")]
        public async Task<IActionResult> Post([FromBody] Wishlist value)
        {
            if (await _mediator.Send(new AddWishListCommand(value)) != null)
            {
                return CreatedAtRoute($"Get{typeof(Wishlist).Name}",
                    new { controller = value.GetType().Name.TrimEnd("Controller"), id = value.Id }, value);
            }
            throw new Exception("Error while adding the object.");
        }

        /// <summary>
        ///     update wishlist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerOperation("Put Wishlist")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Wishlist value)
        {
            if (await _wishlistRepository.UpdateAsync(value))
            {
                return NoContent();
            }
            throw new Exception("Error while updating the object.");
        }

        /// <summary>
        ///     Delete wishlist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [SwaggerOperation("Delete Wishlist")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteWishlistCommand(id));
            if (result) return NoContent();
            throw new Exception("Error while deleting the object.");
        }

        #region Other

        /// <summary>
        ///     Add wishlist content by csv
        /// </summary>
        /// <param name="id">Id of the assessment</param>
        /// <param name="file">The QTI package</param>
        /// <returns></returns>
        [HttpPost("{id}/FillWishlist")]
        public async Task<IActionResult> FillWishlist(Guid id, ICollection<IFormFile> file)
        {
            var wishlist = _wishlistRepository.GetAsync(id).Result;
            if (wishlist == null || file?.FirstOrDefault() == null)
                return BadRequest("Unknown wishlistId or no file uploaded");
            var stream = file.First().OpenReadStream();
            var fillWishlistCommand = new FillWishlistByCsvCommand(stream, wishlist.Id);
            var savedWishlist = await _mediator.Send(fillWishlistCommand);
            return Ok(savedWishlist);
        }
        /// <summary>
        ///     Get items by id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/wishlistitems")]
        [SwaggerOperation("Get WishList by id")]
        public async Task<IEnumerable<WishlistItem>> GetWishlistItemsById(Guid id)
        {
            return
                await Task.Run<IEnumerable<WishlistItem>>(
                    () => { return _wishlistItemRepository.AsQueryable().Where(wi => wi.WishlistId == id); });
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/screeninglist")]
        [SwaggerOperation("Get screeningList by wishListId")]
        public async Task<ScreeningList> GetScreeningList(Guid id)
        {
            return await Task.Run(() =>
            {
                var screeningsListId = _wishlistRepository.GetAsync(id).Result?.ScreeningsListId;
                return screeningsListId.HasValue
                    ? _screeningListRepository.GetAsync(screeningsListId.Value).Result
                    : null;
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/learningobjectives")]
        [SwaggerOperation("Get screeningList by wishListId")]
        public async Task<IEnumerable<LearningObjective>> LearningObjectives(Guid id)
        {
            return
                await
                    Task.Run<IEnumerable<LearningObjective>>(
                        () => _learningObjectiveRepository.AsQueryable().Where(l => l.WishlistId == id));
        }

        /// <summary>
        ///     Get open learning objectives
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/openlearningobjectives")]
        [SwaggerOperation("Get Open LearningObjectives")]
        public async Task<IEnumerable<LearningObjective>> OpenLearningObjectives(Guid id)
        {
            return
                await Task.Run<IEnumerable<LearningObjective>>(
                    () => _learningObjectiveRepository.AsQueryable().Where(l =>
                            l.WishlistId == id && !l.IsDeleted &&
                            (l.Deadline == null || l.Deadline.Value > DateTime.Now) && l.Todo > 0)
                        .OrderBy(l => l.Deadline));
        }

        /// <summary>
        ///     Get ItemSummary by wishList and learningObjective
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/itemsummary/bylearningobjective/{learningObjectiveId}")]
        [SwaggerOperation("Get WishList by id")]
        public async Task<IEnumerable<ItemSummary>> GetItemSummaryByWishListAndLearningObjective(Guid id,
            Guid learningObjectiveId)
        {
            return await Task.Run<IEnumerable<ItemSummary>>(() =>
            {
                var value =
                    _itemSummaryRepository.AsQueryable()
                        .Where(i => i.WishListId == id && i.LearningObjectiveId == learningObjectiveId);
                return value;
            });
        }

        /// <summary>
        ///     Get all accepted items in wishlist in QTI format
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/qtipackage")]
        [SwaggerOperation("Get qti package by wishlist id")]
        public FileStreamResult GetQtiPackageByWishListId(Guid id)
        {
            HttpContext.Response.ContentType = "application/zip";
            var items =
                _itemRepository.AsQueryable().Where(i => i.WishListId == id && i.ItemStatus == ItemStatus.Accepted);
            var testName = _wishlistRepository.GetAsync(id).Result?.Title;
            var qtiConverter = new QtiConverter(_wwwRootPath, testName, _renderer, "22", _repositoryFactory);
            var zipFile = qtiConverter.ConvertItems(items, _wwwRootPath);
            var fileStream = new FileStream(zipFile, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(zipFile)}");
            return File(fileStream, "application/zip");
        }

        [HttpGet("{id}/feedback")]
        [SwaggerOperation("Get feedback by wishlist id")]
        [Produces("text/csv")]
        public FileStreamResult GetFeedback(Guid id)
        {
            HttpContext.Response.ContentType = "text/csv";
            var items = _itemRepository.AsQueryable().Where(i => i.WishListId == id).ToList();
            var itemGuids = items.Select(i => i.Id).ToList();
            var screening = _repositoryFactory.GetRepository<Screening>()
                .AsQueryable()
                .Where(s => itemGuids.Contains(s.ItemId) && s.FeedbackList != null && s.FeedbackList.Length > 0)
                .ToList();
            screening = screening.OrderBy(f => f.CreatedByUserId).ThenByDescending(f => f.Created).ToList();

            var memoryStream = new MemoryStream();

            var streamWriter = new StreamWriter(memoryStream);
            var csv = new CsvWriter(streamWriter,
                new Configuration { Delimiter = ";", Encoding = Encoding.GetEncoding(0) });

            var userDictionary = _repositoryFactory.GetRepository<User>()
                .AsQueryable()
                .ToDictionary(u => u.Id, u => u.Name);
            csv.WriteHeader<ScreeningExport>();
            csv.NextRecord();
            screening.ForEach(s =>
            {
                s.FeedbackList.ToList()
                    .ForEach(f =>
                    {
                        var item = items.FirstOrDefault(i => i.Id == s.ItemId);
                        if (item == null) return;
                        var screeningExport = new ScreeningExport
                        {
                            TextExpert = userDictionary.ContainsKey(s.CreatedByUserId)
                                ? userDictionary[s.CreatedByUserId]
                                : string.Empty,
                            Author = userDictionary.ContainsKey(item.CreatedByUserId)
                                ? userDictionary[item.CreatedByUserId]
                                : string.Empty,
                            Feedback = f.Value,
                            ItemId = item.UniqueCode,
                            ItemTitle = item.Title,
                            LearningObjective = item.LearningObjectiveTitle,
                            Created = s.Created
                        };
                        csv.WriteRecord(screeningExport);
                        csv.NextRecord();
                    });
            });
            //streamWriter.Flush(); // flush the buffered text to stream
            memoryStream.Seek(0, SeekOrigin.Begin); // reset stream position
            var csvFileName = $"feedbacklist-{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.csv";
            Response.Headers.Add("content-disposition", $"attachment; filename={csvFileName}");
            return File(memoryStream, "text/csv");

        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/qtipackage21")]
        [SwaggerOperation("Get qti package by wishlist id")]
        public FileStreamResult GetQtiPackage21ByWishListId(Guid id)
        {
            HttpContext.Response.ContentType = "application/zip";
            var items =
                _itemRepository.AsQueryable().Where(i => i.WishListId == id && i.ItemStatus == ItemStatus.Accepted);
            var testName = _wishlistRepository.GetAsync(id).Result?.Title;
            var qtiConverter = new QtiConverter(_wwwRootPath, testName, _renderer, "21", _repositoryFactory);
            var zipFile = qtiConverter.ConvertItems(items, _wwwRootPath);
            var fileStream = new FileStream(zipFile, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(zipFile)}");
            return File(fileStream, "application/zip");
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/questifybackoffice")]
        [SwaggerOperation("Get questify backoffice export by wishlist id")]
        public FileStreamResult GetQuestifyBackOfficeExportFile(Guid id)
        {
            HttpContext.Response.ContentType = "application/zip";
            var items =
                _itemRepository.AsQueryable().Where(i => i.WishListId == id && i.ItemStatus == ItemStatus.Accepted
                    // && i.LastModified < new DateTime(2017, 7, 10) && i.LastModified > new DateTime(2017, 7, 17)
                    ).ToList();
            var questifyBackofficeConverter = new QuestifyBackOfficeConverter(_wwwRootPath, _repositoryFactory, _renderer);
            var zipFile = questifyBackofficeConverter.ConvertItems(items);
            var fileStream = new FileStream(zipFile, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(zipFile)}");
            return File(fileStream, "application/zip");
        }

        [HttpPost("{id}/WordExportByItemCodes")]
        [SwaggerOperation("Get word export by unique itemcodes")]
        public FileStreamResult GetWordDocByWishListIdAndUniqueItemCodes(Guid id, [FromBody] string[] itemCodes)
        {
            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var codes = itemCodes.ToList();
            var items = _itemRepository.AsQueryable()
                .Where(i => i.WishListId == id && codes.Contains(i.UniqueCode))
                .ToList()
                .OrderBy(i => i.UniqueCode);

            var wordConverter = new WordConverter(_wwwRootPath, _repositoryFactory);
            var wordDoc = wordConverter.Process(items);
            var fileStream = new FileStream(wordDoc, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(wordDoc)}");
            return File(fileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        [HttpGet("{id}/WordExport")]
        [SwaggerOperation("Get word export by unique itemcodes")]
        public FileStreamResult GetWordDocByWishListId(Guid id)
        {
            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var items = _itemRepository.AsQueryable()
                .Where(i => i.WishListId == id)
                .ToList()
                .OrderBy(i => i.UniqueCode);

            var wordConverter = new WordConverter(_wwwRootPath, _repositoryFactory);
            var wordDoc = wordConverter.Process(items);
            var fileStream = new FileStream(wordDoc, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(wordDoc)}");
            return File(fileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        [HttpGet("{id}/WordExportForAcceptedItems")]
        [SwaggerOperation("Get accepted items in word")]
        public FileStreamResult GetAcceptedItemsInWordDocByWishListId(Guid id)
        {
            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            //var items = _itemRepository.AsQueryable()
            //    .Where(i => i.WishListId == id && i.ItemStatus == ItemStatus.Accepted &&
            //            i.LastModified < new DateTime(2017, 7, 10) && i.LastModified > new DateTime(2017, 7, 17))
            var items = _itemRepository.AsQueryable()
                    .Where(i => i.WishListId == id && i.ItemStatus == ItemStatus.Accepted)
                .OrderBy(i => i.UniqueCode).ToList();
            var wordConverter = new WordConverter(_wwwRootPath, _repositoryFactory);
            var wordDoc = wordConverter.Process(items);
            var fileStream = new FileStream(wordDoc, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(wordDoc)}");
            return File(fileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }


        /// <summary>
        ///     Get all accepted items in wishlist in QTI format
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        [HttpGet("qtipackage/{itemIds}")]
        [SwaggerOperation("Get qti package by itemIds")]
        public FileStreamResult GetQtiPackageByWishListId(string itemIds)
        {
            var iIds = itemIds.Split(Convert.ToChar("|"))
                    .Select(i => Guid.TryParse(i, out var guidId) ? guidId : default(Guid))
                    .Where(i => i != default(Guid));
            HttpContext.Response.ContentType = "application/zip";
            var items = _itemRepository.AsQueryable().Where(i => iIds.Contains(i.Id));
            if (items.FirstOrDefault() == null)
                throw new DomainException("No items found", false);

            // ReSharper disable once PossibleNullReferenceException
            var testName = _wishlistRepository.GetAsync(items.FirstOrDefault().WishListId).Result?.Title;
            var qtiConverter = new QtiConverter(_wwwRootPath, testName, _renderer, "22", _repositoryFactory);
            var zipFile = qtiConverter.ConvertItems(items, _wwwRootPath);
            var fileStream = new FileStream(zipFile, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(zipFile)}");
            return File(fileStream, "application/zip");
        }

        [HttpGet("qtipackage21/{itemIds}")]
        [SwaggerOperation("Get qti package by itemIds")]
        public FileStreamResult GetQtiPackage21ByWishListId(string itemIds)
        {
            var iIds = itemIds.Split(Convert.ToChar("|")).Select(i => Guid.TryParse(i, out var guidId) ? guidId : default(Guid)).Where(i => i != default(Guid));
            HttpContext.Response.ContentType = "application/zip";
            var items = _itemRepository.AsQueryable().Where(i => iIds.Contains(i.Id));
            if (items.FirstOrDefault() == null) throw new DomainException("No items found", false);
            // ReSharper disable once PossibleNullReferenceException
            var testName = _wishlistRepository.GetAsync(items.FirstOrDefault().WishListId).Result?.Title;
            var qtiConverter = new QtiConverter(_wwwRootPath, testName, _renderer, "21", _repositoryFactory);
            var zipFile = qtiConverter.ConvertItems(items, _wwwRootPath);
            var fileStream = new FileStream(zipFile, FileMode.Open) { Position = 0 };
            Response.Headers.Add("content-disposition", $"attachment; filename={Path.GetFileName(zipFile)}");
            return File(fileStream, "application/zip");
        }

        #endregion
    }
}