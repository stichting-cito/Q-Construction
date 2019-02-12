using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.User
{
    /// <summary>
    ///     Add user
    /// </summary>
    public class AddUserCommand : IRequest<Model.User>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        public AddUserCommand(Model.User value)
        {
            Value = value;
        }

        /// <summary>
        ///     Value
        /// </summary>
        public Model.User Value { get; set; }
    }
}