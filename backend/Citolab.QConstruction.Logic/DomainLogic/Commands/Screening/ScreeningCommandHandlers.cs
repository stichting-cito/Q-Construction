using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Screening
{
    /// <summary>
    ///     Add screening Handler
    /// </summary>
    public class ScreeningCommandHandlers :
        IRequestHandler<AddScreeningCommand, Model.Screening>,
        IRequestHandler<UpdateScreeningCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILoggedInUserProvider _loggedInUserProvider;
        private readonly IRepositoryFactory _repositoryFactory;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="mediator"></param>
        public ScreeningCommandHandlers(IRepositoryFactory repositoryFactory, IMediator mediator, ILoggedInUserProvider loggedInUserProvider)
        {
            _repositoryFactory = repositoryFactory;
            _mediator = mediator;
            _loggedInUserProvider = loggedInUserProvider;
        }

        public Task<Model.Screening> Handle(AddScreeningCommand request, CancellationToken cancellationToken) => Task.Run(() =>
        {
            var screeningRepository = _repositoryFactory.GetRepository<Model.Screening>();
            var itemRepository = _repositoryFactory.GetRepository<Model.Item>();
            request.Screening.CreatedByUserId = _loggedInUserProvider.GetUserId() ?? Guid.Empty;
            var result = screeningRepository.AddAsync(request.Screening).Result;
            var item = itemRepository.GetAsync(request.Screening.ItemId).Result;

            result.BasedOnVersion = item.Version;
            if (result.Status == ScreeningStatus.Draft)
                result.NextItemStatus = ItemStatus.InReview;
            ScreeningsHelper.UpdateLatestScreeningInItem(result, item, _mediator);
            Debug.Assert(result.Id != Guid.Empty);
            Debug.Assert(result.CreatedByUserId != Guid.Empty);
            return result;
        });

        public Task<bool> Handle(UpdateScreeningCommand request, CancellationToken cancellationToken) => Task.Run(() =>
        {
            var screeningRepository = _repositoryFactory.GetRepository<Model.Screening>();
            var itemRepository = _repositoryFactory.GetRepository<Model.Item>();

            if (request.Screening.Status != ScreeningStatus.Final)
                return screeningRepository.UpdateAsync(request.Screening).Result;
            request.Screening.LastModifiedByUserId = _loggedInUserProvider.GetUserId() ?? Guid.Empty;
            var item = itemRepository.GetAsync(request.Screening.ItemId).Result;
            ScreeningsHelper.UpdateLatestScreeningInItem(request.Screening, item, _mediator);
            Debug.Assert(request.Screening.Id != Guid.Empty);
            Debug.Assert(request.Screening.CreatedByUserId != Guid.Empty);
            Debug.Assert(request.Screening.LastModifiedByUserId != Guid.Empty);
            return screeningRepository.UpdateAsync(request.Screening).Result;
        });
    }
}