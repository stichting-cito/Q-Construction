using System;
using Citolab.QConstruction.Model;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Queries
{
    public class GetAttachmentForItemQuery : IRequest<Attachment>
    {
        public GetAttachmentForItemQuery(Guid itemId, Guid attachmentId, string fileName)
        {
            ItemId = itemId;
            AttachmentId = attachmentId;
            FileName = fileName;
        }

        public Guid ItemId { get; set; }
        public Guid AttachmentId { get; set; }
        public string FileName { get; set; }
    }
}