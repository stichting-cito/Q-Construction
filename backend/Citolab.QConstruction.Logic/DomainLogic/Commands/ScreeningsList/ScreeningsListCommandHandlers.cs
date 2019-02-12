using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.ScreeningsList
{
    /// <summary>
    ///     AddScreeningsListCommandHandler
    /// </summary>
    public class ScreeningsListCommandHandlers :
        IRequestHandler<AddScreeningsListCommand, ScreeningList>,
        IRequestHandler<UpdateScreeningsListCommand, bool>,
        IRequestHandler<FillScreeningByCsvCommand, ScreeningList>

    {
        private readonly IRepositoryFactory _repositoryFactory;

        /// <summary>
        ///     Add Screenings List CommandHandler
        /// </summary>
        /// <param name="repositoryFactory"></param>
        public ScreeningsListCommandHandlers(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }
        
        public Task<ScreeningList> Handle(AddScreeningsListCommand request, CancellationToken cancellationToken) => Task.Run(() =>
        {
            var screeningItemRepository = _repositoryFactory.GetRepository<ScreeningItem>();
            var screeningsListRepository = _repositoryFactory.GetRepository<ScreeningList>();
            var newList = request.Items.Select(screeningItem => screeningItemRepository.AddAsync(screeningItem).Result).ToList();
            return screeningsListRepository.AddAsync(new ScreeningList { Title = request.Title, Items = newList.ToArray() }).Result;
        });

        public Task<bool> Handle(UpdateScreeningsListCommand request, CancellationToken cancellationToken) => Task.Run(() =>
        {
            var screeningItemRepository = _repositoryFactory.GetRepository<ScreeningItem>();
            var screeningsListRepository = _repositoryFactory.GetRepository<ScreeningList>();
            var list = screeningsListRepository.GetAsync(request.Id).Result;
            if (list == null) return false;
            list.Items =
                request.Items.Select(screeningItem => screeningItemRepository.AddAsync(screeningItem).Result).ToArray();
            return screeningsListRepository.UpdateAsync(list).Result;
        });

        public Task<ScreeningList> Handle(FillScreeningByCsvCommand request, CancellationToken cancellationToken) => Task.Run(() =>
        {
            var list = new List<ScreeningItem>();
            using (var reader = new StreamReader(request.CsvData, Encoding.GetEncoding(0)))
            {
                var parser = new CsvParser(reader, new Configuration() { Delimiter = ";", Encoding = Encoding.GetEncoding("windows-1252") });
                var i = 0;
                while (true)
                {
                    var currentRecord = parser.Read();
                    if (i != 0)
                    {
                        if (currentRecord == null) break;
                        if (!(currentRecord.Length >= 3)) continue;
                        ItemType? itemType = null;
                        if (currentRecord.Length >= 4)
                        {
                            if (Enum.TryParse(currentRecord[3], out ItemType it))
                            {
                                itemType = it;
                            }
                        }
                        list.Add(new ScreeningItem
                        {
                            Category = currentRecord[0].Trim(),
                            Code = currentRecord[1].Trim(),
                            Value = currentRecord[2].Trim(),
                            ItemType = itemType
                        });
                    }
                    i++;
                }
                if (request.Id.HasValue)
                {
                    Handle(new UpdateScreeningsListCommand(request.Id.Value, list), cancellationToken).Wait();
                    return _repositoryFactory.GetRepository<ScreeningList>().GetAsync(request.Id.Value).Result;
                }
                return Handle(new AddScreeningsListCommand(list, request.Title), cancellationToken).Result;
            }
        });
    }
}