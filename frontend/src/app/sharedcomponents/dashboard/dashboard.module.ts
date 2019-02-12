import { NgModule,  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarComponent } from './calendar';
import { StatsComponent } from './stats';
import { ProgressDataComponent } from './progress';
import { CalendarsComponent } from './calendars';
import { ProgressbarModule, TooltipModule } from 'ngx-bootstrap';

@NgModule({
    imports: [CommonModule, ProgressbarModule.forRoot(), TooltipModule.forRoot()],
    exports: [CalendarComponent, StatsComponent, ProgressDataComponent, CalendarsComponent],
    declarations: [CalendarComponent, StatsComponent, ProgressDataComponent, CalendarsComponent]
})
export class DashboardModule { }
