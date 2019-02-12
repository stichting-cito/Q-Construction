using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Wishlist
{
    /// <summary>
    ///     Command to add a wishlist
    /// </summary>
    public class AddWishListCommand : IRequest<Model.Wishlist>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        public AddWishListCommand(Model.Wishlist value)
        {
            Value = value;
        }

        /// <summary>
        ///     Value
        /// </summary>
        public Model.Wishlist Value { get; set; }
    }
}