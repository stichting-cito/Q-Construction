using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Item;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Screening;
using Citolab.QConstruction.Logic.DomainLogic.Commands.ScreeningsList;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Wishlist;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace Seed
{
    /// <summary>
    ///     Class to seed initial data
    /// </summary>
    public class Seed
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _memoryCache;
        private readonly ILoggedInUserProvider _loggedInUserProvider;
        private readonly IRepositoryFactory _repositoryFactory;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="mediator"></param>
        /// <param name="memoryCache"></param>
        public Seed(IRepositoryFactory repositoryFactory, IMediator mediator, IMemoryCache memoryCache, ILoggedInUserProvider loggedInUserProvider)
        {
            _repositoryFactory = repositoryFactory;
            _mediator = mediator;
            _memoryCache = memoryCache;
            _loggedInUserProvider = loggedInUserProvider;
        }

        /// <summary>
        ///     Runs the seed
        /// </summary>
        public void Run()
        {
            var userRepository = _repositoryFactory.GetRepository<User>();
            if (userRepository.GetCountAsync().Result != 0) return;
            Constants.FillDefaulValues = false;
           
            GeneralHelper.SyncAuth0Users(_repositoryFactory, _memoryCache);
            var admin = _repositoryFactory.GetRepository<User>()
                .AsQueryable()
                .FirstOrDefault(u => u.UserType == UserType.Admin);
            _loggedInUserProvider.UserObject = admin;
            //
            AddScreeningsAndWishlists();
            AddItems();
            Constants.FillDefaulValues = true;
            //});
        }


        private void AddScreeningsAndWishlists()
        {
            //English DEMO wishList
            var screeningsListEnglish =
                _mediator.Send(
                    new FillScreeningByCsvCommand(
                        "Citolab.QConstruction.Logic.Data.SCREENINGS_LIST_EN.csv",
                        "Default EN")).Result;
            AddWishlist("DEMO International", "Citolab.QConstruction.Logic.Data.DEMO_Q_LO_EN.csv",
                screeningsListEnglish.Id);

            //Dutch DEMO wishList
            var screeningsListDutch =
                _mediator.Send(
                    new FillScreeningByCsvCommand(
                        "Citolab.QConstruction.Logic.Data.SCREENINGS_LIST_NL.csv",
                        "Default NL")).Result;
            AddWishlist("DEMO Nederlands", "Citolab.QConstruction.Logic.Data.DEMO_Q_LO_NL.csv",
                screeningsListDutch.Id);
        }

        private async void AddWishlist(string title, string resourceName, Guid screeningListId)
        {
            var wishlist =
                _mediator.Send(
                    new AddWishListCommand(new Wishlist
                    {
                        Title = title,
                        ScreeningsListId = screeningListId,
                        Created = DateTime.Now.AddDays(-14)
                    })).Result;
            await _mediator.Send(new FillWishlistByCsvCommand(resourceName, wishlist.Id, null));
            var userRepository = _repositoryFactory.GetRepository<User>();
            var allUsers = userRepository.AsQueryable().ToList();
            allUsers.ForEach(u =>
            {
                if (u.AllowedWishlists != null && u.AllowedWishlists.Select(aw => aw.Id).Contains(wishlist.Id))
                    return;
                var wl = new List<KeyValue> { new KeyValue { Id = wishlist.Id, Value = wishlist.Title } };
                u.AllowedWishlists = u.AllowedWishlists?.ToList().Concat(wl).ToArray() ?? wl.ToArray();
                userRepository.UpdateAsync(u).Wait();
            });
        }

        


        private void AddItems()
        {
            var userRepository = _repositoryFactory.GetRepository<User>();
            var allUsers = userRepository.AsQueryable().ToList();
            var cFast =
                allUsers.FirstOrDefault(
                    u => u.Email.Equals("Casper.Fast@citolab.nl", StringComparison.OrdinalIgnoreCase));
            var cCreative =
                allUsers.FirstOrDefault(
                    u => u.Email.Equals("Connor.Creative@citolab.nl", StringComparison.OrdinalIgnoreCase));
            var cNewbie =
                allUsers.FirstOrDefault(
                    u => u.Email.Equals("Charlotte.Newbie@citolab.nl", StringComparison.OrdinalIgnoreCase));
            var cCopy =
                allUsers.FirstOrDefault(
                    u => u.Email.Equals("Chantal.Copy@citolab.nl", StringComparison.OrdinalIgnoreCase));
            var tStrict =
                allUsers.FirstOrDefault(u => u.Email.Equals("Tom.Strict@citolab.nl", StringComparison.OrdinalIgnoreCase));
            var tLean =
                allUsers.FirstOrDefault(u => u.Email.Equals("Tara.Lean@citolab.nl", StringComparison.OrdinalIgnoreCase));
            var users = new List<User> {cFast, cCreative, cNewbie, cCopy, tStrict, tLean};

            if (tStrict == null || tLean == null) return;
            var experts = new List<Guid> {tStrict.Id, tStrict.Id, tStrict.Id, tLean.Id, tLean.Id};

            var itemRepository = _repositoryFactory.GetRepository<Item>();
            if (itemRepository.GetCountAsync().Result != 0) return;
            var wishListRepository = _repositoryFactory.GetRepository<Wishlist>();

            foreach (var wishList in wishListRepository.AsQueryable())
            {

                var i1 = CreateItemsForConstructor(cFast, 0.09, wishList, _mediator);
                CreateScreenings(i1, 0.7, 0.9, _mediator, experts);
                var i2 = CreateItemsForConstructor(cCopy, 0.10, wishList, _mediator);
                CreateScreenings(i2, 2.5, 0.6, _mediator, experts);
                var i3 = CreateItemsForConstructor(cCreative, 0.03, wishList, _mediator);
                CreateScreenings(i3, 1, 0.6, _mediator, experts);
                var i4 = CreateItemsForConstructor(cNewbie, 0.05, wishList, _mediator);
                CreateScreenings(i4, 3, 0.5, _mediator, experts);
            }
        }

        private async void CreateScreenings(IReadOnlyCollection<Item> items, double avgRounds, double percentageAccepted,
            IMediator mediator, List<Guid> experts)
        {
            var rnd = new Random();
            var totalRounds = Math.Max(items.Count, Convert.ToInt32(Math.Round(items.Count * (avgRounds + 1))));
            var totalAccepted = Convert.ToInt32(Math.Round(items.Count * percentageAccepted));
            var rounds = 0;
            var itemIndex = 0;
            var acceptedCount = 0;
            foreach (var im in items)
            {
                if (im.ItemStatus == ItemStatus.Draft)
                {
                    im.ItemStatus = ItemStatus.ReadyForReview;
                    await mediator.Send(new UpdateItemCommand {Item = im});
                }
                var otherThanTwo = 0 == rnd.Next(0, 3);
                var extra = otherThanTwo ? rnd.Next(1, 5) : 2;
                var nrRounds = totalRounds >= rounds + (items.Count - itemIndex) ? extra : 1;
                if (itemIndex == items.Count - 1) nrRounds = totalRounds - rounds;
                for (var i = 0; i < nrRounds; i++)
                {
                    var isAccepted = i == nrRounds - 1 && totalAccepted != acceptedCount;
                    var isRejected = !isAccepted && i == nrRounds - 1 && 0 != rnd.Next(0, 3);
                    await CreateScreening(im,
                        experts[rnd.Next(0, experts.Count)], isAccepted, isRejected, mediator);
                    if (isAccepted) acceptedCount++;
                    if (i != nrRounds - 1 && im.ItemStatus == ItemStatus.NeedsWork) await ModifyItem(im, mediator);
                    itemIndex++;
                    rounds++;
                }
                var random = rnd.Next(0, 3);
                if (im.ItemStatus == ItemStatus.NeedsWork && 0 == random)
                    await DeleteItemByConstructor(im, mediator);
                else if (im.ItemStatus == ItemStatus.NeedsWork && 1 == random)
                    await ModifyItem(im, mediator);
                itemIndex++;
            }
        }

        private async Task<bool> ModifyItem(Item item, IMediator mediator)
        {
            item.BodyText = item.BodyText.EndsWith(" ") ? item.BodyText.Trim() : item.BodyText + " ";
            item.ItemStatus = ItemStatus.ReadyForReview;
            return await mediator.Send(new UpdateItemCommand {Item = item});
        }

        private async Task<bool> DeleteItemByConstructor(Item item, IMediator mediator)
        {
            item.ItemStatus = ItemStatus.Deleted;
            return await mediator.Send(new UpdateItemCommand {Item = item});
        }


        private List<Item> CreateItemsForConstructor(User constructor, double percentageOfWorkLeft, Wishlist wishList,
            IMediator mediator)
        {
            var items = new List<Item>();
            var totalItems = wishList.WishListItems.Sum(wi => wi.NumberOfItems);
            var days = Convert.ToInt32((DateTime.Now - wishList.Created).TotalDays);
            var rnd = new Random();
            for (int i = 0; i < Convert.ToInt32(totalItems * percentageOfWorkLeft); i++)
            {
                var itemsNotFull = wishList.WishListItems.Where(wi => wi.Todo != 0).ToList();
                var wItem = itemsNotFull[rnd.Next(0, itemsNotFull.Count)];
                var learningObjectiveRepository = _repositoryFactory.GetRepository<LearningObjective>();
                var lo = learningObjectiveRepository.GetAsync(wItem.LearningObjectiveId).Result;
                var created = wishList.Created.AddDays(rnd.Next(0, days));
                var item = CreateItem(constructor.Id, wishList, wItem, lo, created, 0 == rnd.Next(0, 2));
                item = mediator.Send(new AddItemCommand {Item = item}).Result;
                items.Add(item);
            }
            return items;
        }

        private Item CreateItem(Guid userId, Wishlist w, WishlistItem wi, LearningObjective lo, DateTime created,
            bool isMc)
        {
            var rnd = new Random();
            var operatorEnum = (OperatorEnum) rnd.Next(1, 4);
            var max = operatorEnum == OperatorEnum.Divide || operatorEnum == OperatorEnum.Multiply ? 10 : 100;
            var number1 = rnd.Next(1, max);
            var number2 = rnd.Next(1, max);
            var key = 0;
            var distractors = new List<int>();
            var operatorString = "";
            switch (operatorEnum)
            {
                case OperatorEnum.Sum:
                {
                    key = number1 + number2;
                    for (var i = 0; i < 3; i++)
                    {
                        var d = key;
                        while (d == key || distractors.Contains(d))
                        {
                            d = rnd.Next(key - 10, key + 10);
                        }
                        distractors.Add(d);
                    }
                    operatorString = "+";
                    break;
                }
                case OperatorEnum.Minus:
                {
                    var n1 = number1;
                    number1 = Math.Max(n1, number2);
                    if (number1 == number2) number2 = n1;
                    key = number1 - number2;
                    for (var i = 0; i < 3; i++)
                    {
                        var d = key;
                        while (d == key || distractors.Contains(d))
                        {
                            d = rnd.Next(key - 10, key + 10);
                        }
                        distractors.Add(d);
                    }
                    operatorString = "-";
                    break;
                }
                case OperatorEnum.Divide:
                    key = number1;
                    number1 = number1 * number2;
                    for (var i = 0; i < 3; i++)
                    {
                        var d = number2;
                        while (d == key || distractors.Contains(d))
                        {
                            d = rnd.Next(key - 3, key + 3);
                        }
                        distractors.Add(d);
                    }
                    operatorString = "/";
                    break;
                case OperatorEnum.Multiply:
                    key = number1 * number2;
                    for (var i = 0; i < 3; i++)
                    {
                        var n1 = rnd.Next(number1 - 2, number1 + 2);
                        var n2 = rnd.Next(number2 - 2, number2 + 2);
                        var d = n1 * n2;
                        while (d == key || distractors.Contains(d))
                        {
                            n1 = rnd.Next(number1 - 2, number1 + 2);
                            n2 = rnd.Next(number2 - 2, number2 + 2);
                            d = n1 * n2;
                        }
                        distractors.Add(d);
                    }
                    operatorString = "*";
                    break;
            }
            var ds = !isMc
                ? null
                : new[]
                {
                    new SimpleChoice {Title = key.ToString(), IsKey = true},
                    new SimpleChoice {Title = distractors[0].ToString()},
                    new SimpleChoice {Title = distractors[1].ToString()},
                    new SimpleChoice {Title = distractors[2].ToString()}
                };
            return new Item
            {
                Created = created,
                CreatedByUserId = userId,
                BodyText = $"{number1} {operatorString} {number2} = ",
                SimpleChoices = ds,
                Key = key.ToString(),
                ItemStatus = ItemStatus.Draft,
                Deadline = wi.Deadline,
                DomainTitle = lo.DomainTitle,
                LearningObjectiveCode = lo.Code,
                LearningObjectiveTitle = lo.Title,
                IsDeleted = false,
                Title = "Pamcakes",
                ItemType = isMc ? ItemType.MC : ItemType.SA,
                LastModified = created,
                LastModifiedByUserId = userId,
                LearningObjectiveId = lo.Id,
                Version = 1,
                WishListId = w.Id,
                WishListTitle = w.Title,
            };
        }


        private async Task<bool> CreateScreening(Item item, Guid testExpertId, bool isAccepted, bool isRejected,
            IMediator mediator)
        {
            var rnd = new Random();
            var ldate = item.LastModified;
            var screeningsItems = new List<Guid>();
            // var catToExclude = (item.ItemType == ItemType.MC) ? ItemType.SA : ItemType.MC;


            var screeningListId =
                _repositoryFactory.GetRepository<Wishlist>().GetAsync(item.WishListId).Result.ScreeningsListId;
            var slist =
                _repositoryFactory.GetRepository<ScreeningList>()
                    .GetAsync(screeningListId)
                    .Result.Items.Where(si => si.ItemType != item.ItemType)
                    .ToList();
            for (var i = 0; i < rnd.Next(1, 4); i++)
            {
                var added = false;
                while (!added)
                {
                    var siToAdd = slist[rnd.Next(0, slist.Count)];
                    if (screeningsItems.Contains(siToAdd.Id)) continue;
                    screeningsItems.Add(siToAdd.Id);
                    added = true;
                }
            }
            var feedback =
                screeningsItems.Select(si => new Feedback {ScreeningItemId = si, Value = "Have a look at this point."});
            if (item.LatestScreeningId.HasValue)
            {
                var screeningRepository = _repositoryFactory.GetRepository<Screening>();
                var s = await screeningRepository.GetAsync(item.LatestScreeningId.Value);
                ldate = s.LastModified;
            }
            var days = Convert.ToInt32((DateTime.Now - ldate).TotalDays);
            var created = ldate.AddDays(rnd.Next(0, days));
            var screening = new Screening
            {
                Created = created,
                CreatedByUserId = testExpertId,
                LastModified = created,
                LastModifiedByUserId = testExpertId,
                BasedOnVersion = item.Version,
                ItemId = item.Id,
                FeedbackList = isAccepted || isRejected ? null : feedback.ToArray(),
                IsDeleted = false,
                NextItemStatus =
                    isAccepted || isRejected
                        ? (isAccepted ? ItemStatus.Accepted : ItemStatus.Rejected)
                        : ItemStatus.NeedsWork,
                Status = ScreeningStatus.Final
            };
            await mediator.Send(new AddScreeningCommand {Screening = screening});
            return true;
        }



        enum OperatorEnum
        {
            Sum = 1,
            Multiply = 2,
            Minus = 3,
            Divide = 4
        }
    }
}