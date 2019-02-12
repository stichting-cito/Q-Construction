using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Item
{
    /// <summary>
    ///     Update Item
    /// </summary>
    public class UpdateItemCommand : IRequest<bool>
    {
        /// <summary>
        ///     Updated Item
        /// </summary>
        public Model.Item Item { get; set; }
    }
}