using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Queries
{
    public class ItemQueryHandlers : IRequestHandler<GetAttachmentForItemQuery, Attachment>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ItemQueryHandlers(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public Task<Attachment> Handle(GetAttachmentForItemQuery request, CancellationToken cancellationToken) => Task.Run(() =>
        {
            var itemRepository = _repositoryFactory.GetRepository<Item>();
            var item = itemRepository.GetAsync(request.ItemId).Result;
            if (item == null) throw new DomainException($"Item with id {request.ItemId} does not exist.", false);
            var attachment = _repositoryFactory.GetRepository<Attachment>().GetAsync(request.AttachmentId).Result;
            if (attachment == null) throw new DomainException($"Attachment with id {request.AttachmentId} does not exist for item {request.ItemId}", false);
            return attachment;
        });
    }
}