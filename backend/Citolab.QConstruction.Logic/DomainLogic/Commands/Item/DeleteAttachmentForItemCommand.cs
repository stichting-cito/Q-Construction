using System;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Item
{
    public class DeleteAttachmentForItemCommand : IRequest<bool>
    {
        public DeleteAttachmentForItemCommand(Guid itemId, Guid attachmentId)
        {
            ItemId = itemId;
            AttachmentId = attachmentId;
        }

        public Guid ItemId { get; set; }
        public Guid AttachmentId { get; set; }
    }
}