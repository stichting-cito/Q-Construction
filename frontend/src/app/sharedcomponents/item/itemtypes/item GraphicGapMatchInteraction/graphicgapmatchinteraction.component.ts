import { Component, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ItemService } from '../../../../shared/services/item.service';
import { Item } from '../../../../shared/model/model';
import { FormBuilder } from '@angular/forms';
import { HotSpotInteractionComponent } from '../item HotSpotInteraction/hotspotinteraction.component';
import { IImageHandler } from '../../../../sharedcomponents/editor/quill.interfaces';

@Component({
    moduleId: module.id,
    selector: 'app-graphicgapmatchinteraction-template',
    templateUrl: '../../description.template.html'
})

export class GraphicGapMatchInteractionComponent extends HotSpotInteractionComponent {
    @Input() imageHandler: IImageHandler;
    @Input() readonly = false;
    @Input() item: Item;

    identifier = 'graphicGapMatchInteraction';
    constructor(
        formBuilder: FormBuilder,
        itemService: ItemService,
        domSanitizer: DomSanitizer) {
        super(
            formBuilder,
            itemService,
            domSanitizer
        );
    }
}
