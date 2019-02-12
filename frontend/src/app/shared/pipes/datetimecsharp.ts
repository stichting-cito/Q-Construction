import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'datetimecsharp'
})
export class DateTimeCSharpPipe implements PipeTransform {
    transform(value: any): any {
        if (value) {
            return new Date(Date.parse(value.toString()));
        }
    }
}
