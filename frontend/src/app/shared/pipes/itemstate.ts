import { Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ItemStatus } from '../model/model';
import { Observable } from 'rxjs';

@Pipe({
    name: 'itemstate'
})

export class ItemStatePipe implements PipeTransform {
    constructor(private translateService: TranslateService) {  }

    transform(state: ItemStatus): Observable<string> {
        return this.translateService.get('ITEMSTATE_' + (ItemStatus[state]).toUpperCase());
    }
}
