import { FormGroup } from '@angular/forms';

export interface IItemType {
    identifier: string;
    form: FormGroup;
    copyFormModelPropertiesToItemProperties(): void;
}
