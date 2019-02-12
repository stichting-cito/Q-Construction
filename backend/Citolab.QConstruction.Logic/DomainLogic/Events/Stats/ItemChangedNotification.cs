using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Events.Stats
{
    /// <summary>
    ///     Item changed notification
    /// </summary>
    public class ItemChangedNotification : INotification
    {
        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="updateStatsEvent"></param>
        public ItemChangedNotification(UpdateStatsEvent updateStatsEvent)
        {
            UpdateStatsEvent = updateStatsEvent;
        }

        /// <summary>
        ///     Update stats
        /// </summary>
        public UpdateStatsEvent UpdateStatsEvent { get; set; }
    }
}