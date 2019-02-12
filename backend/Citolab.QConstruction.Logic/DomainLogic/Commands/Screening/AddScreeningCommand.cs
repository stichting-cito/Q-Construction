using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Screening
{
    /// <summary>
    ///     Add screening command
    /// </summary>
    public class AddScreeningCommand : IRequest<Model.Screening>
    {
        /// <summary>
        ///     Screening
        /// </summary>
        public Model.Screening Screening { get; set; }
    }
}