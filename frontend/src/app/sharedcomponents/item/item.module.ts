import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { QuillEditorModule } from './../../sharedcomponents/editor/quill.editor.module';
import { HtmlViewerModule } from './../../sharedcomponents/htmlviewer/htmlviewer.module';
import { CommonModule } from '@angular/common';
import { FileUploadModule } from 'ng2-file-upload/file-upload/file-upload.module';
import { SortablejsModule } from 'angular-sortablejs/dist/src/sortablejs.module';

import { ItemWrapperComponent } from './item.component';
import { ChoiceInteractionComponent } from './itemtypes/item ChoiceInteraction/choiceinteraction.component';
import { HotSpotInteractionComponent } from './itemtypes/item HotSpotInteraction/hotspotinteraction.component';
import { TextEntryInteractionComponent } from './itemtypes/item TextEntryInteraction/textentryinteraction.component';
import { GraphicGapMatchInteractionComponent } from './itemtypes/item GraphicGapMatchInteraction/graphicgapmatchinteraction.component';
import { EditNotesComponent } from './notes/notes';

@NgModule({
    imports: [ReactiveFormsModule, TranslateModule, QuillEditorModule, HtmlViewerModule,
        CommonModule, FormsModule, FileUploadModule, SortablejsModule],
    declarations: [
        ItemWrapperComponent,
        ChoiceInteractionComponent,
        HotSpotInteractionComponent,
        TextEntryInteractionComponent,
        GraphicGapMatchInteractionComponent,
        EditNotesComponent
    ],
    exports: [ItemWrapperComponent, EditNotesComponent]
})

export class ItemModule { }
