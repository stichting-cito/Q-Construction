import { Component, Input, OnInit } from '@angular/core';
import { Item } from '../../../../shared/model/model';
import { FormArray, FormGroup, FormControl, Validators } from '@angular/forms';
import { IItemType } from '../Iitemtype';
import { SortablejsOptions } from 'angular-sortablejs';
import { IImageHandler } from '../../../../sharedcomponents/editor/quill.interfaces';
import { ItemTypeUtilities } from '../itemUtilities';

interface SimpleChoiceFormModel {
    title: string;
    key: boolean;
}

@Component({
    moduleId: module.id,
    selector: 'app-choiceinteraction-template',
    styleUrls: ['./choiceinteraction.component.scss'],
    templateUrl: './choiceinteraction.component.html'
})
export class ChoiceInteractionComponent implements OnInit, IItemType {
    @Input() imageHandler: IImageHandler;
    @Input() readonly = false;
    @Input() multiple = false;
    @Input() item: Item;


    public editItemForm: FormGroup;
    public isEmpty = ItemTypeUtilities.isEmpty;

    simpleChoicesFormArray: FormArray;
    identifier = 'choiceInteraction';
    sortoptions: SortablejsOptions = {
        animation: 250,
        handle: '.handle',
        onRemove: () => {
            console.log('remove');
        }
    };
    simpleChoicesIndex = 0;
    selectedSimpleChoice: SimpleChoiceFormModel = null;

    get simpleChoices() {
        return (this.editItemForm.controls as any).simpleChoices;
    }

    sortRandom() {
        const arr = this.simpleChoicesFormArray;
        let i = arr.length;
        let randomIndex: number;

        // While there remain elements to shuffle...
        while (0 !== i) {
            // Pick a remaining element...
            randomIndex = Math.floor(Math.random() * i);
            i -= 1;
            // And swap it with the current element.
            const temp = arr.at(i);
            arr.removeAt(i);
            arr.insert(randomIndex, temp);
        }
    }

    validateKeys(formArray: FormArray): { [key: string]: any } {
        let minimal = 1;
        let maximum = 1;

        if (this.multiple) {
            minimal = 2;
            maximum = formArray.length - 1;
        }

        let nrchecked = 0;

        if (formArray && formArray.controls) {
            for (let x = 0; x < formArray.length; ++x) {
                if (formArray.at(x).get('key').value) {
                    nrchecked++;
                }
            }
        }
        let valid = false;
        if (nrchecked >= minimal && nrchecked <= maximum) {
            valid = true;
        }
        return valid ? null : { 'required': true };
    }

    get form() {
        return this.editItemForm;
    }

    public initSimpleChoicesFormGroup() {
        const group = this.item.simpleChoices.map((sc) => this.addSimpleChoice(sc.title, sc.isKey));
        return new FormArray(group, (formArray: FormArray) => this.validateKeys(formArray));
    }

    sortAlpha() {
        const arr = this.simpleChoicesFormArray;
        let swapped = true;
        let j = 0;
        while (swapped) {
            swapped = false;
            j++;
            for (let i = 0; i < arr.length - j; i++) {
                // Sorry for the duplicate regexp. Strip everything so we can sort on numbers if there are any
                const res1 = arr.at(i).get('title').value.replace(/(<([^>]+)>)/ig, '');
                const res2 = arr.at(i + 1).get('title').value.replace(/(<([^>]+)>)/ig, '');

                // parse to int if possible
                if ((isNaN(res1) ? res1 : +res1) > (isNaN(res2) ? res2 : +res2)) {
                    const temp = arr.at(i);
                    arr.removeAt(i);
                    arr.insert(i + 1, temp);
                    swapped = true;
                }
            }
        }
    }

    ngOnInit() {
        // When an item is created, create 3 default simpleChoices
        if (this.item.simpleChoices === null || this.item.simpleChoices.length === 0) {
            this.item.simpleChoices = [0, 1, 2].map(() => ({ 'title': '', 'isKey': false }));
        }
        this.simpleChoicesFormArray = this.initSimpleChoicesFormGroup();
        this.editItemForm = new FormGroup({
            'question': new FormControl(this.item.bodyText, [Validators.required]),
            'simpleChoices': this.simpleChoicesFormArray
        });
    }

    setRadio(index: number) {
        const simpleChoices = (this.form.get('simpleChoices') as FormArray).controls;
        for (let i = 0; i < simpleChoices.length; i++) {
            simpleChoices[i].get('key').setValue(false);
        }
        simpleChoices[index].get('key').setValue(true);
    }

    focusNext(el: HTMLInputElement) {
        el.select();
        el.focus();
    }

    public copyFormModelPropertiesToItemProperties() {
        // copy properties from model form to item form
        this.item.bodyText = this.form.value.question;
        this.item.simpleChoices = (this.form.value.simpleChoices as SimpleChoiceFormModel[])
            .map((element) => ({ 'title': element.title, 'isKey': element.key }));
    }

    addSimpleChoice(title: string = '', key: boolean = false) {
        this.simpleChoicesIndex++;

        return new FormGroup({
            title: new FormControl(title, [Validators.required]),
            key: new FormControl(key),
            identifier: new FormControl(this.simpleChoicesIndex) // sneaky heack to get an identifier in the simplechoices
        });
    }
}
