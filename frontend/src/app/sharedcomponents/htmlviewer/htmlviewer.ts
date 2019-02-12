import { Component, Input, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
@Component({
    moduleId: module.id,
    selector: 'app-htmlviewer',
    template: `<div #htmldiv [innerHTML]="innerHTML"></div>
    <div bsModal #imageModal="bs-modal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="image modal" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title pull-left"></h4>
                <button type="button" class="close pull-right" (click)="imageModal.hide()" aria-label="Close">
                <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <img style="max-width:100%;max-height:100%;"*ngIf="selectedImage" [src]="selectedImage" />
            </div>
            </div>
        </div>
        </div>`
})

export class HtmlViewerComponent implements AfterViewInit {
    @Input() innerHTML = '';
    public selectedImage = '';
    @ViewChild('imageModal') public imageModal: ModalDirective;
    constructor(protected el: ElementRef) { }

    ngAfterViewInit(): void {
        const div = this.findSingle(this.el.nativeElement, 'div');
        div.ondblclick = (e) => {
            if (e && e.srcElement) {
                const img = e.srcElement as HTMLImageElement;
                if (img && img.src && img.src.indexOf('thumbnail') !== -1) {
                    this.selectedImage = img.src.replace('/thumbnail', '');
                    this.imageModal.show();
                }
            }
        };
    }

    findSingle = (element: any, selector: string) => element.querySelector(selector);
}
