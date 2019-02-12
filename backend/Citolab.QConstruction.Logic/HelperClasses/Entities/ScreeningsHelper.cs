using System.Diagnostics;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Item;
using Citolab.QConstruction.Model;
using MediatR;

namespace Citolab.QConstruction.Logic.HelperClasses.Entities
{
    /// <summary>
    ///     Helper for screenings
    /// </summary>
    public static class ScreeningsHelper
    {
        /// <summary>
        ///     Update id of latest screening in item
        /// </summary>
        /// <param name="screening"></param>
        /// <param name="item"></param>
        /// <param name="mediator"></param>
        public static void UpdateLatestScreeningInItem(Screening screening, Item item, IMediator mediator)
        {
            if (screening.Status != ScreeningStatus.Final && item.LatestScreeningId.HasValue) return;
            var screeningId = screening.Id;
            if (item == null) return;
            item.LatestScreeningId = screeningId;
            item.LatestScreeningAuthorId = screening.CreatedByUserId;
            if (screening.NextItemStatus.HasValue) item.ItemStatus = screening.NextItemStatus.Value;
            //Update the item
            Debug.Assert(item.LatestScreeningAuthorId != null);

            mediator.Send(new UpdateItemCommand {Item = item}).Wait();
        }
    }
}