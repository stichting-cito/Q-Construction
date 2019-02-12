import { Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { of } from 'rxjs';

@Pipe({
    name: 'emptystate'
})
export class EmptyStatePipe implements PipeTransform {
    constructor(private translateService: TranslateService) { }
    transform = (value: string) => (!!value) ? of(value) : this.translateService.get('NOT_FILLED');
}
