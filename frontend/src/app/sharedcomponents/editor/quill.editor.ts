/*
    Copied and edited from https://github.com/primefaces/primeng/blob/master/src/app/components/editor/editor.ts
    The MIT License (MIT)
    Copyright (c) 2016-2018 PrimeTek
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
    associated documentationfiles (the "Software"), to deal in the Software without restriction, including without
    limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
    Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
    INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
    ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
    IN THE SOFTWARE.
*/
import {
    Component, ElementRef, AfterViewInit, OnInit, Input, Output, EventEmitter,
    ContentChild, forwardRef, ViewChild
} from '@angular/core';
import { HeaderComponent } from './quill.header';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { IImageHandler } from './quill.interfaces';
import { ImageToBase64Handler } from './../../shared/helpers/image.handlers';
import { ModalDirective } from 'ngx-bootstrap/modal';
import Quill from 'quill';

export const EDITOR_VALUE_ACCESSOR: any = {
    provide: NG_VALUE_ACCESSOR,
    // tslint:disable-next-line
    useExisting: forwardRef(() => QuillEditorComponent),
    multi: true
};

@Component({
    selector: 'app-quill-editor',
    templateUrl: 'quill.editor.html',
    styleUrls: ['quill.editor.scss'],
    providers: [EDITOR_VALUE_ACCESSOR]
})
export class QuillEditorComponent implements AfterViewInit, OnInit, ControlValueAccessor {
    @Output() attachmentAdded: EventEmitter<any> = new EventEmitter(); // emits attachment id
    @Output() textChanged: EventEmitter<any> = new EventEmitter();
    @Output() selectionChanged: EventEmitter<any> = new EventEmitter();

    @ContentChild(HeaderComponent, { static: false }) toolbar: any;
    @ViewChild('imageModal', { static: false }) public imageModal: ModalDirective;

    @Input() showToolbar = true;
    @Input() toggleToolbar = false;
    @Input() style: any;
    @Input() acceptTab = false;
    @Input() styleClass: string;
    @Input() placeholder: string;
    @Input() readOnly: boolean;
    @Input() formats: string[];
    @Input() mode: string;
    @Input() imageHandler: IImageHandler;
    @Input() maxCharacters: 2000;
    public selectedImage = '';
    hideToolbar = true;
    value: string;
    quill: Quill;
    private systemPasting = false;
    // overidde add image function to be able to place filename in alt attribute.
    private handleImage = ((value: any) => {
        const that = this;
        const container = (this.quill as any).container;
        let fileInput = container.querySelector('input.ql-image[type=file]');
        if (fileInput === null) {
            fileInput = document.createElement('input');
            fileInput.setAttribute('style', 'display:none;');
            fileInput.setAttribute('type', 'file');
            fileInput.setAttribute('accept', 'image/png, image/gif, image/jpeg, image/svg+xml');
            fileInput.classList.add('ql-image');
            fileInput.addEventListener('change', () => {
                that.imageHandler.handle(fileInput.files[0]).subscribe((src) => {
                    if (src) {
                        const filename = fileInput.files[0].name.replace(/^.*[\\\/]/, '');
                        const range = this.quill.getSelection(true);
                        this.systemPasting = true;
                        this.quill.clipboard.dangerouslyPasteHTML(range.index, `<img alt=${filename} src=${src}>`, 'user');
                        this.systemPasting = false;
                    }
                });
            });
        }
        container.appendChild(fileInput);
        fileInput.click();
    }).bind(this);
    onModelChange = (html?: string): any => {
        //
    }
    onModelTouched = (): any => {
        //
    }
    constructor(protected el: ElementRef) { }
    ngOnInit() {
        this.hideToolbar = (!this.showToolbar || this.toggleToolbar);
        this.placeholder = (this.readOnly) ? '' : this.placeholder;
        if (!this.imageHandler) {
            this.imageHandler = new ImageToBase64Handler(); // default base64
        }
    }
    ngAfterViewInit() {
        const editorElement = this.findSingle(this.el.nativeElement, 'div.ui-editor-content');
        const toolbarElement = this.findSingle(this.el.nativeElement, 'div.ui-editor-toolbar');
        this.quill = new Quill(editorElement, {
            modules: {
                toolbar: toolbarElement

            },
            placeholder: this.placeholder,
            readOnly: this.readOnly,
            theme: 'snow',
            formats: this.formats
        });
        if (!this.acceptTab) {
            const keyboard = this.quill.getModule('keyboard');
            delete keyboard.bindings[9];
        }
        this.quill.clipboard.addMatcher(Node.ELEMENT_NODE, (node, delta) => {
            if (!this.systemPasting && node.outerHTML.trim().indexOf('<') === 0) {
                delta.ops = [];
                return delta.insert(node.innerText, []);
            }
            return delta;
        });
        if (this.value) {
            this.systemPasting = true;
            this.quill.clipboard.dangerouslyPasteHTML(this.value);
            this.systemPasting = false;
        }
        editorElement.ondblclick = (e) => {
            if (e && e.srcElement) {
                const img = e.srcElement as HTMLImageElement;
                if (img && img.src && img.src.indexOf('thumbnail') !== -1) {
                    this.selectedImage = img.src.replace('/thumbnail', '');
                    this.imageModal.show();
                }
            }
        };

        this.quill.on('text-change', (delta: any, source: any) => {
            let html = editorElement.children[0].innerHTML;
            const text = this.quill.getText();
            if (text.length > this.maxCharacters) { // exit if max chars is reached.
                return this.quill.deleteText(this.maxCharacters, this.quill.getLength());
            }
            if (html === '<p><br></p>') {
                html = null;
            }

            this.textChanged.emit({
                htmlValue: html,
                textValue: text,
                delta,
                source
            });

            this.onModelChange(html);
        });
        this.quill.on('selection-change', (range: any, oldRange: any, source: any) => {
            this.hideToolbar = (!this.showToolbar || (this.toggleToolbar && (range === null)));
            this.selectionChanged.emit({
                range,
                oldRange,
                source
            });
        });
        const toolbar = this.quill.getModule('toolbar');
        toolbar.addHandler('image', this.handleImage);
    }

    findSingle = (element: any, selector: string) => element.querySelector(selector);

    writeValue(value: any): void {
        this.value = value;

        if (this.quill) {
            this.systemPasting = true;
            if (value) {
                this.quill.clipboard.dangerouslyPasteHTML(value);
            } else {
                this.quill.setText('');
            }
            this.systemPasting = false;
        }
    }
    registerOnChange = (fn: any) => this.onModelChange = fn;
    registerOnTouched = (fn: any) => this.onModelTouched = fn;
}

