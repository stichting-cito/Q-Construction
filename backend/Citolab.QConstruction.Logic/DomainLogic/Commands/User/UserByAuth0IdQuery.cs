using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.User
{
    public class UserByAuth0IdQuery : IRequest<Model.User>
    {
        public UserByAuth0IdQuery(string externalId)
        {
            ExternalId = externalId;
        }
        public string ExternalId { get; set; }
    }
}
