import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Item } from '../../../../shared/model/model';
import { ItemService } from '../../../../shared/services/item.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IItemType } from '../Iitemtype';
import { FileItem } from 'ng2-file-upload/file-upload/file-item.class';
import { CustomFileUploader } from '../../../../shared/helpers/custom.fileuploader';
import { IImageHandler } from '../../../../sharedcomponents/editor/quill.interfaces';
import { ItemTypeUtilities } from '../itemUtilities';
import { environment } from './../../../../../environments/environment';
import { map } from 'rxjs/operators';

@Component({
    moduleId: module.id,
    selector: 'app-hotspotinteraction-template',
    styles: [`
        .my-drop-zone {
            height:200px;
            line-height:200px;
            border:2px dotted #eeeeee;
            text-align:center;
        }
    `],
    templateUrl: '../../description.template.html'
})

export class HotSpotInteractionComponent implements OnInit, IItemType {
    @Input() imageHandler: IImageHandler;
    @Input() readonly = false;
    @Input() item: Item;

    public editItemForm: FormGroup;

    public isEmpty = ItemTypeUtilities.isEmpty;

    identifier = 'hotSpotInteraction';
    public uploaderAttachments: CustomFileUploader;
    public hasBaseDropZoneOver = false;
    public thumbnails = new Array<SafeUrl>();

    constructor(
        private formBuilder: FormBuilder,
        private itemService: ItemService,
        private domSanitizer: DomSanitizer
    ) { }

    get form() {
        return this.editItemForm;
    }

    ngOnInit() {
        this.editItemForm = this.formBuilder.group({
            bodyText: [this.item.bodyText, Validators.required],
            description: [this.item.description, Validators.required],
            attachments: [this.item.attachmentIds]
        });
        const url = `${environment.api}/Items/${this.item.id}/attachment`;
        this.uploaderAttachments = new CustomFileUploader(url);
        this.uploaderAttachments.onAfterAddingFile = (fi: FileItem) => {
            this.uploaderAttachments.upload(fi).subscribe(attachmentId => this.filesUploaded(attachmentId));
        };
        this.getThumbnails();
    }
    public copyFormModelPropertiesToItemProperties(): void {
        // copy properties from model form to item form
        this.item.bodyText = this.editItemForm.value.bodyText;
        this.item.description = this.editItemForm.value.description;
    }
    public fileOverBase(e: any): void {
        this.hasBaseDropZoneOver = e;
    }
    focusNext(el: HTMLInputElement) {
        el.select();
        el.focus();
    }

    private filesUploaded(attachmentId: string) {
        this.item.attachmentIds = (this.item.attachmentIds) ?
            this.item.attachmentIds.concat([attachmentId]) :
            [attachmentId];
        this.itemService.save(this.item);
        this.thumbnails.push(this.domSanitizer.bypassSecurityTrustUrl(this.itemService.getAttachmentUrl(this.item.id, attachmentId, true)));
    }

    private getThumbnails() {
        if (this.item.attachmentIds) {
            this.thumbnails = this.item.attachmentIds
                .map(id => this.itemService.getAttachmentUrl(this.item.id, id, true))
                .map(s => this.domSanitizer.bypassSecurityTrustUrl(s));
        }
    }
}
