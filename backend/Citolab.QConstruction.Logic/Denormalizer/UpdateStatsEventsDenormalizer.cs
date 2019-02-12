using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Logic.DomainLogic.Events.Stats;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Citolab.QConstruction.Logic.Denormalizer
{
    /// <summary>
    ///     Take care of updating stats
    /// </summary>
    public class UpdateStatsEventsDenormalizer : DenormalizerBase,
        IUpdateStatsEventProcessor<UpdateStatsEvent>,
        INotificationHandler<ItemChangedNotification>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="repositoryFactory"></param>
        public UpdateStatsEventsDenormalizer(ILoggerFactory loggerFactory, IRepositoryFactory repositoryFactory)
            : base(loggerFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public Task Handle(ItemChangedNotification notification, CancellationToken cancellationToken) => Task.Run(() =>
        {
            EnqueueEvent(notification.UpdateStatsEvent);
        });

        /// <summary>
        ///     Process
        /// </summary>
        /// <param name="updateStatsEvent"></param>
        /// <returns></returns>
        public bool Process(UpdateStatsEvent updateStatsEvent)
        {
            WishlistHelper.UpdateWishListWhenItemChanged(updateStatsEvent.Id, updateStatsEvent.ItemAfterSave,
                updateStatsEvent.ItemBeforeSave, _repositoryFactory);
            return true;
        }
    }
}