import { Component, Input, OnInit, AfterViewInit } from '@angular/core';
import { Item } from '../../../../shared/model/model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IItemType } from '../Iitemtype';
import { ItemTypeUtilities } from '../itemUtilities';
import { IImageHandler } from '../../../../sharedcomponents/editor/quill.interfaces';

@Component({
    moduleId: module.id,
    selector: 'app-textentryinteraction-template',
    templateUrl: 'textentryinteraction.component.html'
})

export class TextEntryInteractionComponent implements OnInit, IItemType {
    @Input() imageHandler: IImageHandler;
    @Input() readonly = false;
    @Input() item: Item;

    public isEmpty = ItemTypeUtilities.isEmpty;

    public editItemForm: FormGroup;
    public identifier = 'textEntryInteraction';

    constructor(private formBuilder: FormBuilder) { }

    get form() {
        return this.editItemForm;
    }

    ngOnInit() {
        this.editItemForm = this.formBuilder.group({
            bodyText: [this.item.bodyText, Validators.required],
            key: [this.item.key, Validators.required]
        });
    }

    copyFormModelPropertiesToItemProperties(): void {
        // copy properties from model form to item form
        this.item.bodyText = this.editItemForm.value.bodyText;
        this.item.key = this.editItemForm.value.key;
    }
}
