using System;
using Citolab.QConstruction.Model;

namespace Citolab.QConstruction.Logic.DomainLogic.Events.Stats
{
    /// <summary>
    ///     Update stats
    /// </summary>
    public class UpdateStatsEvent : BaseEvent
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public UpdateStatsEvent(Guid id, Item before, Item after)
        {
            Id = id;
            ItemBeforeSave = before;
            ItemAfterSave = after;
        }

        /// <summary>
        ///     ItemId
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Item before it was saved
        /// </summary>
        public Item ItemBeforeSave { get; set; }

        /// <summary>
        ///     Item after it was saved
        /// </summary>
        public Item ItemAfterSave { get; set; }
    }
}