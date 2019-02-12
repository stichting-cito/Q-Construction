import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'daysleft'
})

export class DaysLeftPipe implements PipeTransform {
    transform(date: Date): any {
        if (date) {
            const oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
            const toDay = new Date();
            return Math.round(Math.abs((date.getTime() - toDay.getTime()) / (oneDay)));
        }
    }
}
