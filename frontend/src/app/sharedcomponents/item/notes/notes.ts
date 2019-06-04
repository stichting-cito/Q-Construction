import { Component, Input, OnInit, EventEmitter, Output, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Item } from './../../../shared/model/model';
import { Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
    selector: 'app-edit-notes',
    styleUrls: ['notes.scss'],
    template: `<app-quill-editor class="note" [readOnly]="readonly"
                [maxCharacters]="200" mode="simple"
                [showToolbar]="false" placeholder="{{'PLACEHOLDER_NOTES' | translate}}"
                [formControl]="notesControl">
            </app-quill-editor>`
})

export class EditNotesComponent implements OnInit, OnDestroy {

    @Input() readonly = false;
    @Input() item: Item;
    @Output() notesChanged = new EventEmitter<any>();
    public notesControl = new FormControl();
    private formSubscription: Subscription;

    ngOnInit(): void {
        this.notesControl.setValue(this.item.notes);
        this.formSubscription = this.notesControl.valueChanges
            .pipe(debounceTime(1000))
            .subscribe(newValue => {
                this.item.notes = newValue;
                this.notesChanged.emit();
            });
    }

    ngOnDestroy(): void {
        if (this.formSubscription) {
            this.formSubscription.unsubscribe();
        }
    }
}
