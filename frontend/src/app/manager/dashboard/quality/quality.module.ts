import { NgModule } from '@angular/core';
import { DashboardQualityComponent } from './quality.dashboard.component';
import { SharedModule } from './../../../shared/shared.module';
import { DataListModule } from 'primeng/components/datalist/datalist';
import { DropdownModule } from 'primeng/components/dropdown/dropdown';
import { ChartModule } from 'primeng/components/chart/chart';
@NgModule({
    imports: [SharedModule, DataListModule, DropdownModule, ChartModule],
    declarations: [DashboardQualityComponent],
    providers: [],
    exports: [DashboardQualityComponent]
})

export class QualityModule { }
