import {
    Component, Input, OnInit, Output, AfterViewInit, ViewChild,
    ViewEncapsulation, EventEmitter, AfterViewChecked, OnDestroy
} from '@angular/core';
import { ItemService } from '../../shared/services/item.service';
import { Item, ItemType } from '../../shared/model/model';
import { ItemTypeToChoose } from '../../shared/model/frontendmodel';
import { IItemType } from './itemtypes/Iitemtype';
import { Subscription } from 'rxjs';
import { AddImageToItemHandler } from './../../shared/helpers/image.handlers';
import { IImageHandler } from './../../sharedcomponents/editor/quill.interfaces';
import { debounceTime } from 'rxjs/operators';
@Component({
    selector: 'app-edit-item',
    styleUrls: ['item.component.scss'],
    encapsulation: ViewEncapsulation.None,
    templateUrl: 'item.component.html'
})

export class ItemWrapperComponent implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy {
    @ViewChild('componentType', { static: false }) itemComponent: IItemType;
    @Input() readonly = false;
    @Input() item: Item;
    @Input() disabledItemTypes = new Array<ItemType>();
    @Output() formValidChanged: EventEmitter<boolean> = new EventEmitter();
    @Output() itemChanged: EventEmitter<boolean> = new EventEmitter();

    public itemType = ItemType;
    public isValid = false;
    public imageHandler: IImageHandler;
    public itemTypesToChoose = new Array<ItemTypeToChoose>();
    public selectedItemType: ItemTypeToChoose;
    private valueChangedSubScription: Subscription;
    private valueChangedForSaveSubScription: Subscription;
    private selectedFormIdentifier = '';
    private itemTypes: Array<ItemTypeToChoose> =
        [{ type: ItemType.mc, icon: 'glyphicon-th-list', textRef: 'ITEM_TYPE_MC' },
        { type: ItemType.sa, icon: 'glyphicon-text-background', textRef: 'ITEM_TYPE_SA' },
        { type: ItemType.hotspot, icon: 'glyphicon-th-large', textRef: 'ITEM_TYPE_HOTSPOT' },
        { type: ItemType.graphicgapmatch, icon: 'glyphicon-modal-window', textRef: 'ITEM_TYPE_DRAGDROP' }];
    constructor(private itemService: ItemService) { }

    ngOnInit() {
        this.imageHandler = new AddImageToItemHandler(this.item, this.itemService);
        this.itemTypesToChoose = this.itemTypes
            .filter(it => (!this.disabledItemTypes) || !this.disabledItemTypes
                .find(i => i === it.type));
        if (!this.item.itemType && this.itemTypesToChoose.length > 0) {
            this.item.itemType = this.itemTypesToChoose[0].type;
        }
        this.selectedItemType = this.itemTypesToChoose.find(it => it.type === this.item.itemType);
    }
    ngOnDestroy(): void {
        if (this.valueChangedSubScription) {
            this.valueChangedSubScription.unsubscribe();
        }
        if (this.valueChangedForSaveSubScription) {
            this.valueChangedForSaveSubScription.unsubscribe();
        }
    }
    ngAfterViewChecked() {
        const that = this;
        if (this.itemComponent) {
            if (this.selectedFormIdentifier !== this.itemComponent.identifier) {
                this.selectedFormIdentifier = this.itemComponent.identifier;
                this.checkAndSetFormValueChange();
                if (this.valueChangedSubScription) {
                    this.valueChangedSubScription.unsubscribe();
                }
                if (this.valueChangedForSaveSubScription) {
                    this.valueChangedForSaveSubScription.unsubscribe();
                }
                this.valueChangedSubScription = this.itemComponent.form.valueChanges
                    .subscribe(() => {
                        that.checkAndSetFormValueChange();
                    });

                this.valueChangedForSaveSubScription = this.itemComponent.form.valueChanges
                    .pipe(debounceTime(1000))
                    .subscribe(_ => {
                        this.itemComponent.copyFormModelPropertiesToItemProperties();
                        this.itemChanged.emit();
                    });
            }
        }
    }

    checkAndSetFormValueChange() {
        if (this.isValid !== this.itemComponent.form.valid) {
            this.isValid = this.itemComponent.form.valid;
            // This is a hack! you shouldn't change the model in the ngAfterViewChecked
            Promise.resolve().then(() => this.formValidChanged.emit(this.isValid));
        }
    }

    ngAfterViewInit() {
        if (this.itemComponent) {
            this.formValidChanged.emit(this.itemComponent.form.valid);
        }
    }

    selectType(value: ItemType) {
        // this.itemComponent.isFormValid.unsubscribe();
        this.item.itemType = value;
        this.itemChanged.emit();
    }

    get identifier() {
        return (this.itemComponent) ?
            this.itemComponent.identifier : '';
    }
}
