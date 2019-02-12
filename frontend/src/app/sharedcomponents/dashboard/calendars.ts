import { Component, Input } from '@angular/core';
@Component({
    moduleId: module.id,
    selector: 'app-calendar-icons',
    templateUrl: 'calendars.html'
})

export class CalendarsComponent {
    @Input() overdeadline: number;
    @Input() oneday: number;
    @Input() sevendays: number;
    @Input() oversevendays: number;
}
