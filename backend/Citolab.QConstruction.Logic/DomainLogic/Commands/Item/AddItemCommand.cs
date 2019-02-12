using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Item
{
    /// <summary>
    ///     Add item command
    /// </summary>
    public class AddItemCommand : IRequest<Model.Item>
    {
        /// <summary>
        ///     Updated Item
        /// </summary>
        public Model.Item Item { get; set; }
    }
}