import { Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { of, Observable, pipe } from 'rxjs';
import { truncate } from '../helpers/utils';
import { map } from 'rxjs/operators';

@Pipe({
    name: 'truncate'
})

export class TruncatePipe implements PipeTransform {
    constructor(private translateService: TranslateService) { }

    transform(text: string, length: number): Observable<string> {
     return this.translateService.get('NOT_FILLED')
        .pipe(map(emptyValue => truncate(text, length, emptyValue)));
    }


}
