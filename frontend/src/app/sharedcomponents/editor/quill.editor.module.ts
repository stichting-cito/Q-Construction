import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuillEditorComponent } from './quill.editor';
import { HeaderComponent } from './quill.header';
import { ModalModule } from 'ngx-bootstrap/modal';
@NgModule({
    imports: [CommonModule, ModalModule],
    exports: [QuillEditorComponent, HeaderComponent],
    declarations: [QuillEditorComponent, HeaderComponent]
})
export class QuillEditorModule { }
