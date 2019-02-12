import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalModule } from 'ngx-bootstrap/modal';
import { HtmlViewerComponent } from './htmlviewer';
@NgModule({
    imports: [CommonModule, ModalModule],
    exports: [HtmlViewerComponent],
    declarations: [HtmlViewerComponent]
})
export class HtmlViewerModule { }
