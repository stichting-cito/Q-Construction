import { Component, Input } from '@angular/core';
@Component({
    moduleId: module.id,
    selector: 'app-calendar-icon',
    templateUrl: 'calendar.html',
})

export class CalendarComponent {
    @Input() itemcount: number;
    @Input() text: string;
    @Input() color: string;
    @Input() icon: string;
}
