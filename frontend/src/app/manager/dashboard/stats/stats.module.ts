import { NgModule } from '@angular/core';
import { DashboardStatsComponent } from './stats.dashboard.component';
import { SharedModule } from './../../../shared/shared.module';
import { DashboardModule } from './../../../sharedcomponents/dashboard/dashboard.module';
import { ChartModule } from 'primeng/components/chart/chart';
import { CarouselModule } from 'ngx-bootstrap';
@NgModule({
    imports: [SharedModule, DashboardModule, ChartModule, CarouselModule],
    declarations: [DashboardStatsComponent],
    providers: [],
    exports: [DashboardStatsComponent]
})

export class StatsModule { }
