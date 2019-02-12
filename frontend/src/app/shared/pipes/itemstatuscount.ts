import { Pipe, PipeTransform } from '@angular/core';
import { ItemStatus, ItemStatusCount } from '../model/model';

@Pipe({
    name: 'itemstatuscount'
})

export class ItemStatusCountPipe implements PipeTransform {
    transform(states: ItemStatus[], statusCount: ItemStatusCount[]): any {
        let count = 0;
        for (const state of states) {
            count += this.getStateCount(state, statusCount);
        }
        return count;
    }

    getStateCount(state: ItemStatus, statusCount: ItemStatusCount[]): number {
        return (statusCount && statusCount.filter(s => s.itemStatus === state).length > 0) ?
            statusCount.filter(s => s.itemStatus === state)[0].count : 0;
    }
}
