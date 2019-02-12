using System;
using System.Collections.Generic;
using Citolab.QConstruction.Model;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.ScreeningsList
{
    /// <summary>
    ///     Update screeningsList
    /// </summary>
    public class UpdateScreeningsListCommand : IRequest<bool>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="items"></param>
        public UpdateScreeningsListCommand(Guid id, List<ScreeningItem> items)
        {
            Items = items;
            Id = id;
        }

        /// <summary>
        ///     Screening
        /// </summary>
        public List<ScreeningItem> Items { get; set; }

        /// <summary>
        ///     Id
        /// </summary>
        public Guid Id { get; set; }
    }
}