using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Backend.HelperClasses;
using Citolab.QConstruction.Logic.DomainLogic.Commands.Item;
using Citolab.QConstruction.Logic.DomainLogic.Events.Stats;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Item
{
    /// <summary>
    ///     Add ietm command handler
    /// </summary>
    public class ItemCommandHandlers :
        IRequestHandler<AddItemCommand, Model.Item>,
        IRequestHandler<UpdateItemCommand, bool>,
        IRequestHandler<AddAttachmentToItemCommand, Guid>,
        IRequestHandler<DeleteAttachmentForItemCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILoggedInUserProvider _loggedInUserProvider;
        private readonly IRepositoryFactory _repositoryFactory;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="mediator"></param>
        public ItemCommandHandlers(IRepositoryFactory repositoryFactory, IMediator mediator, ILoggedInUserProvider loggedInUserProvider)
        {
            _mediator = mediator;
            _loggedInUserProvider = loggedInUserProvider;
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        ///     Add an attachment (image, e.g.) to an item
        /// </summary>
        /// <param name="command">Command containing the form file and item id.</param>
        /// <returns>A unique identifier for the attachment that can be used in a URI.</returns>
        public Task<Guid> Handle(AddAttachmentToItemCommand command, CancellationToken cancellationToken) => Task.Run(() =>
       {
           // check if item exists
           var itemRepository = _repositoryFactory.GetRepository<Model.Item>();
           var item = itemRepository.GetAsync(command.ItemId).Result;
           if (item == null) throw new DomainException($"Item with id {command.ItemId} does not exist.", false);
           // add attachment to attachment repo
           var attachmentRepository = _repositoryFactory.GetRepository<Attachment>();

           var stream = new MemoryStream();
           command.FormFile.CopyTo(stream);
           var attachment = new Attachment
           {
               Bytes = stream.ToArray(),
           };
           attachment.ComputeHash();
           // check if the file already exists in the repo
           var existingAttachment = attachmentRepository.AsQueryable().FirstOrDefault(a => a.Hash == attachment.Hash);
           if (existingAttachment != null)
           {
               if (!existingAttachment.UsedInItemIds.Contains(command.ItemId))
               {
                   existingAttachment.UsedInItemIds.Add(command.ItemId);
                   attachmentRepository.UpdateAsync(existingAttachment).Wait();
               }
               return existingAttachment.Id;
           }
           attachment.FileName = command.FormFile.FileName;
           attachment.UsedInItemIds = new List<Guid> { command.ItemId };
           attachment.ContentType = command.FormFile.ContentType;
           if (attachment.ContentType.IsImageOrVideoContentType())
           {
               attachment.ThumbnailBytes = MediaHelper.GetThumbnail(stream);
           }
           return attachmentRepository.AddAsync(attachment).Result.Id;
       });

        /// <summary>
        ///     Handler the add
        /// </summary>
        /// <param name="addItemCommand"></param>
        /// <returns></returns>
        public Task<Model.Item> Handle(AddItemCommand addItemCommand, CancellationToken cancellationToken) => Task.Run(() =>
        {
            var itemRepository = _repositoryFactory.GetRepository<Model.Item>();
            ItemHelper.AddRelatedInfo(addItemCommand.Item, _repositoryFactory, _loggedInUserProvider);
            var addedItem = itemRepository.AddAsync(addItemCommand.Item).Result;
            ItemHelper.UpdateStatus(null, addedItem, _repositoryFactory);
            ItemHelper.UpdateItemSummary(null, addedItem, _repositoryFactory);
            ItemHelper.UpdateVersionedItem(addedItem, _repositoryFactory);
            _mediator.Publish(new ItemChangedNotification(new UpdateStatsEvent(addItemCommand.Item.Id, null, addItemCommand.Item))).Wait();
            Debug.Assert(addedItem.Id != Guid.Empty);
            Debug.Assert(addedItem.CreatedByUserId != Guid.Empty);
            return addedItem;
        });

        public Task<bool> Handle(DeleteAttachmentForItemCommand command, CancellationToken cancellationToken) => Task.Run(() =>
        {
            // check if item exists
            var itemRepository = _repositoryFactory.GetRepository<Model.Item>();
            var item = itemRepository.GetAsync(command.ItemId).Result;
            if (item == null)
                throw new DomainException($"Item with id {command.ItemId} does not exist.", false);
            var attachmentRepository = _repositoryFactory.GetRepository<Attachment>();
            var attachment = attachmentRepository.GetAsync(command.AttachmentId).Result;
            if (attachment == null)
                throw new DomainException($"Attachment with id {command.AttachmentId} does not exist.", false);
            if (!attachment.UsedInItemIds.Contains(command.ItemId))
                throw new DomainException($"Attachment with id {command.AttachmentId} is not linked to item with id {command.ItemId}", true);
            attachment.UsedInItemIds.Remove(command.ItemId);
            return !attachment.UsedInItemIds.Any() ?
                        attachmentRepository.DeleteAsync(command.AttachmentId).Result :
                        attachmentRepository.UpdateAsync(attachment).Result;
        });

        /// <summary>
        ///     Do the update
        /// </summary>
        /// <param name="updatedItemCommand"></param>
        /// <returns></returns>
        public async Task<bool> Handle(UpdateItemCommand updatedItemCommand, CancellationToken cancellationToken)
        {
            var itemRepository = _repositoryFactory.GetRepository<Model.Item>();

            if (updatedItemCommand.Item.ItemStatus == ItemStatus.ReadyForReview)
            {
                updatedItemCommand.Item.LatestScreeningId = null;
            }
            updatedItemCommand.Item.LastModifiedByUserId = _loggedInUserProvider?.GetUserId() != null ?
                // ReSharper disable once PossibleInvalidOperationException
                _loggedInUserProvider.GetUserId().Value :
                Guid.Empty;
            updatedItemCommand.Item = ItemHelper.SetVersion(updatedItemCommand.Item, _repositoryFactory);
            var beforeSave = itemRepository.GetAsync(updatedItemCommand.Item.Id).Result;
            var updated = await itemRepository.UpdateAsync(updatedItemCommand.Item);
            ItemHelper.UpdateStatus(beforeSave, updatedItemCommand.Item, _repositoryFactory);
            ItemHelper.UpdateItemSummary(beforeSave, updatedItemCommand.Item, _repositoryFactory);
            ItemHelper.UpdateVersionedItem(updatedItemCommand.Item, _repositoryFactory);
            //if (beforeSave.AttachmentIds != null)
            //{
            //    foreach (var removedAttachmentId in beforeSave.AttachmentIds.Except(updatedItemCommand.Item?.AttachmentIds))
            //    {
            //        await _mediator.Send(new DeleteAttachmentForItemCommand(updatedItemCommand.Item.Id, removedAttachmentId));
            //    }
            //}
            await _mediator.Publish(
                new ItemChangedNotification(new UpdateStatsEvent(updatedItemCommand.Item.Id, beforeSave,
                    updatedItemCommand.Item)));
            DebugAssert(updatedItemCommand.Item);
            return updated;
        }

        // ReSharper disable once UnusedParameter.Local
        private static void DebugAssert(Model.Item item)
        {
            Debug.Assert(
                !((item.ItemStatus == ItemStatus.Accepted || item.ItemStatus == ItemStatus.Rejected) &&
                  item.LatestScreeningId == null));
            Debug.Assert(
                !((item.ItemStatus == ItemStatus.Accepted || item.ItemStatus == ItemStatus.Rejected) &&
                  item.LatestScreeningAuthorId == null));
            Debug.Assert(item.Id != Guid.Empty);
            Debug.Assert(item.CreatedByUserId != Guid.Empty);
            Debug.Assert(item.LastModifiedByUserId != Guid.Empty);
        }

    }
}