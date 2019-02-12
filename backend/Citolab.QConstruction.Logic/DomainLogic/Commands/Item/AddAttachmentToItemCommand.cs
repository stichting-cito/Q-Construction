using System;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Item
{
    public class AddAttachmentToItemCommand : IRequest<Guid>
    {
        public AddAttachmentToItemCommand(Guid itemId, IFormFile formFile)
        {
            ItemId = itemId;
            FormFile = formFile;
        }

        public Guid ItemId { get; set; }
        public IFormFile FormFile { get; set; }
    }
}