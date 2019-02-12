using System;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Wishlist
{
    /// <summary>
    ///     Delete wishlist
    /// </summary>
    public class DeleteWishlistCommand : IRequest<bool>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        public DeleteWishlistCommand(Guid id)
        {
            Id = id;
        }

        /// <summary>
        ///     Value
        /// </summary>
        public Guid Id { get; set; }
    }
}