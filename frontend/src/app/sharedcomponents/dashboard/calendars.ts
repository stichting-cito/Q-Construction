import { Component, Input } from '@angular/core';
@Component({
    selector: 'app-calendar-icons',
    templateUrl: 'calendars.html'
})

export class CalendarsComponent {
    @Input() overdeadline: number;
    @Input() oneday: number;
    @Input() sevendays: number;
    @Input() oversevendays: number;
}
