using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Screening
{
    /// <summary>
    ///     Update command for screening
    /// </summary>
    public class UpdateScreeningCommand : IRequest<bool>
    {
        /// <summary>
        ///     The screening
        /// </summary>
        public Model.Screening Screening { get; set; }
    }
}