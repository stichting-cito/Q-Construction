import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'filter'
})
export class FilterPipe implements PipeTransform {
    transform(value: any, args: string[]): any {
        const filter = args[0];

        if (filter && Array.isArray(value)) {
            const filterKeys = Object.keys(filter);
            return value.filter((item: any) =>
                filterKeys.reduce((memo: any, keyName: any) =>
                    memo && item[keyName] === filter[keyName], true));
        } else {
            return value;
        }
    }
}
