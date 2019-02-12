using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Wishlist
{
    /// <summary>
    ///     Wishlist command handler
    /// </summary>
    public class WishlistCommandHandlers :
        IRequestHandler<FillWishlistByCsvCommand, Model.Wishlist>,
        IRequestHandler<AddWishListCommand, Model.Wishlist>,
        IRequestHandler<DeleteWishlistCommand, bool>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ILoggedInUserProvider _loggedInUserProvider;

        /// <summary>
        ///     Add WishlistCommandHandler
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="loggedInUserProvider"></param>
        public WishlistCommandHandlers(IRepositoryFactory repositoryFactory, ILoggedInUserProvider loggedInUserProvider)
        {
            _repositoryFactory = repositoryFactory;
            _loggedInUserProvider = loggedInUserProvider;
        }

        public Task<Model.Wishlist> Handle(FillWishlistByCsvCommand request, CancellationToken cancellationToken) => Task.Run(() =>
        {
            var wishlistRepository = _repositoryFactory.GetRepository<Model.Wishlist>();
            var wishlistItemRepository = _repositoryFactory.GetRepository<WishlistItem>();
            var domainRepository = _repositoryFactory.GetRepository<Domain>();
            var learningObjectiveRepository = _repositoryFactory.GetRepository<LearningObjective>();
            var wishlistItems = new List<WishlistItem>();
            var wishlist = request.Id.HasValue
                ? wishlistRepository.GetAsync(request.Id.Value).Result
                : wishlistRepository.AddAsync(new Model.Wishlist { Title = request.Title }).Result;

            using (var reader = new StreamReader(request.CsvData, Encoding.GetEncoding(1252)))
            {
                var previousDomain = new Domain { Title = "" };
                var parser = new CsvParser(reader, new Configuration() { Delimiter = ";", Encoding = Encoding.GetEncoding(1252) });
                var i = 0;
                while (true)
                {
                    var currentRecord = parser.Read();
                    if (i != 0)
                    {
                        if (currentRecord == null) break;
                        if (currentRecord.Length < 5) continue;
                        int todos;
                        todos = Convert.ToInt32(currentRecord[4]);
                        if (previousDomain.Title != currentRecord[2].Trim())
                        {

                            var domainTitle = currentRecord[2].Trim();
                            //check if already exists by title. If so, don't add again
                            var existingDomain =
                                domainRepository.AsQueryable().FirstOrDefault(d => d.Title == domainTitle);
                            if (existingDomain != null && existingDomain.Title != domainTitle)
                            {
                                existingDomain.Title = domainTitle;
                                domainRepository.UpdateAsync(existingDomain).Wait();
                            }
                            previousDomain = existingDomain ??
                                             domainRepository.AddAsync(new Domain { Title = domainTitle }).Result;
                        }
                        var learningObjectiveCode = currentRecord[0].Trim();
                        var existingLearningObjective =
                            learningObjectiveRepository.AsQueryable()
                                .FirstOrDefault(d => d.WishlistId == wishlist.Id && d.Code == learningObjectiveCode);
                        var lo = existingLearningObjective;
                        var cultureinfo = new System.Globalization.CultureInfo("nl-NL");
                        var deadline = request.DaysToDeadline.HasValue
                            ? DateTime.Now.AddDays(request.DaysToDeadline.Value) :
                            DateTime.Parse(currentRecord[3].Trim(), cultureinfo);
                        if (existingLearningObjective != null)
                        {
                            var diff = todos - existingLearningObjective.Total;
                            existingLearningObjective.Total = todos;
                            existingLearningObjective.Deadline = deadline;
                            existingLearningObjective.DomainTitle = previousDomain.Title;
                            existingLearningObjective.Todo = existingLearningObjective.Todo + diff;
                            existingLearningObjective.Title = currentRecord[1].Trim();
                            learningObjectiveRepository.UpdateAsync(existingLearningObjective).Wait();

                            var wishlistItem =
                                wishlistItemRepository.AsQueryable()
                                    .FirstOrDefault(wi => wi.WishlistId == wishlist.Id &&
                                                          wi.DomainId == lo.DomainId &&
                                                          wi.LearningObjectiveId ==
                                                          existingLearningObjective.Id);
                            if (existingLearningObjective.Total == todos && wishlistItem.Deadline == deadline) continue;
                            wishlistItem.Todo = existingLearningObjective.Todo;
                            wishlistItem.Deadline = deadline;
                            wishlistItemRepository.UpdateAsync(wishlistItem).Wait();
                            wishlist.WishListItems.Remove(
                                wishlist.WishListItems.FirstOrDefault(w => w.Id == wishlistItem.Id));
                            wishlist.WishListItems.Add(wishlistItem);
                        }
                        else
                        {
                            lo = learningObjectiveRepository.AddAsync(new LearningObjective
                            {
                                DomainId = previousDomain.Id,
                                WishlistId = wishlist.Id,
                                Deadline = deadline,
                                DomainTitle = previousDomain.Title,
                                Code = learningObjectiveCode,
                                Title = currentRecord[1].Trim(),
                                Total = todos,
                                Todo = todos
                            }).Result;
                            wishlistItems.Add(wishlistItemRepository.AddAsync(new WishlistItem
                            {
                                DomainId = lo.DomainId,
                                LearningObjectiveCode = lo.Code,
                                LearningObjectiveTitle = lo.Title,
                                LearningObjectiveId = lo.Id,
                                NumberOfItems = lo.Todo,
                                Todo = lo.Todo,
                                WishlistId = wishlist.Id,
                                Deadline = deadline
                            }).Result);
                        }
                    }
                    i++;
                }
            }
            wishlistRepository.UpdateAsync(wishlist).Wait(cancellationToken);
            WishlistHelper.CreateNewWishListStats(_repositoryFactory, wishlist.Id);
            return wishlist;
        });

        public Task<Model.Wishlist> Handle(AddWishListCommand request, CancellationToken cancellationToken) => Task.Run(() =>
         {
              if (request.Value.ScreeningsListId == default(Guid))
             {
                 var screeningList = _repositoryFactory.GetRepository<ScreeningList>().FirstOrDefaultAsync().Result;
                 if (screeningList != null) request.Value.ScreeningsListId = screeningList.Id;
             }
             var wishlistRepository = _repositoryFactory.GetRepository<Model.Wishlist>();
             var userRepository = _repositoryFactory.GetRepository<Model.User>();
             var wishlistItemRepository = _repositoryFactory.GetRepository<WishlistItem>();
             if (!_loggedInUserProvider.GetUserId().HasValue) throw new DomainException("Unknown user", true);
             var loggedInUser = userRepository.GetAsync(_loggedInUserProvider.GetUserId().Value).Result;
             if (!(loggedInUser.UserType == UserType.Admin || loggedInUser.UserType == UserType.Manager)) throw new UnauthorizedAccessException();

             var savedWiList = new List<WishlistItem>();
             request.Value.WishListItems?.ForEach(wi => savedWiList.Add(wishlistItemRepository.AddAsync(wi).Result));
             request.Value.WishListItems = savedWiList;
             var newWishlist = wishlistRepository.AddAsync(request.Value).Result;
             // Add wishlist to allowed lists and set as selected.
             var allowedWishlists = loggedInUser.AllowedWishlists != null ? loggedInUser.AllowedWishlists.ToList() : new List<KeyValue>();
             allowedWishlists.Add(new KeyValue {Id = newWishlist.Id, Value = newWishlist.Title });
             loggedInUser.AllowedWishlists = allowedWishlists.ToArray();
             loggedInUser.SelectedWishlist = new KeyValue {Id = newWishlist.Id, Value = newWishlist.Title};
             userRepository.UpdateAsync(loggedInUser);
             WishlistHelper.CreateNewWishListStats(_repositoryFactory, newWishlist.Id);
             return newWishlist;
         });

        public Task<bool> Handle(DeleteWishlistCommand request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                var wishlistRepository = _repositoryFactory.GetRepository<Model.Wishlist>();
                var userRepository = _repositoryFactory.GetRepository<Model.User>();
                var wishlist = wishlistRepository.GetAsync(request.Id).Result;
                if (wishlist.WishListItems != null && wishlist.WishListItems.Any(wi => wi.NumberOfItems != wi.Todo))
                    return false; //cannot delete if it has already items
                if (wishlistRepository.DeleteAsync(request.Id).Result)
                {
                    //Remove from selected and available wishlist
                    var users =
                        userRepository.AsQueryable().Where(u => u.AllowedWishlists.Any(aw => aw.Id == request.Id));
                    foreach (var user in users)
                    {
                        user.AllowedWishlists = user.AllowedWishlists.Where(aw => aw.Id != request.Id).ToArray();
                        userRepository.UpdateAsync(user).Wait();
                    }
                    var us = userRepository.AsQueryable().Where(u => u.SelectedWishlist.Id == request.Id);
                    foreach (var user in us)
                    {
                        user.SelectedWishlist = null;
                        userRepository.UpdateAsync(user).Wait();
                    }
                }
                return true;
            });
    }
}