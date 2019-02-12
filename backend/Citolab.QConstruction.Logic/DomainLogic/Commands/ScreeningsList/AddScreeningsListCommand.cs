using System.Collections.Generic;
using Citolab.QConstruction.Model;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.ScreeningsList
{
    /// <summary>
    ///     Add screeningsList
    /// </summary>
    public class AddScreeningsListCommand : IRequest<ScreeningList>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="items"></param>
        /// <param name="title"></param>
        public AddScreeningsListCommand(List<ScreeningItem> items, string title)
        {
            Items = items;
            Title = title;
        }

        /// <summary>
        ///     Screening
        /// </summary>
        public List<ScreeningItem> Items { get; set; }

        /// <summary>
        ///     Title
        /// </summary>
        public string Title { get; set; }
    }
}