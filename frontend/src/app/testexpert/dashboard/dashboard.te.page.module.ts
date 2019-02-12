import { NgModule } from '@angular/core';
import { DashboardTEComponent } from './dashboard.te.page.component';
import { DashboardTeRoutingModule } from './dashboard.te.page.routing.module';
import { SharedModule } from './../../shared/shared.module';
import { TestExpertItemListResolver } from './../../shared/resolvers/testexpertitemlist.resolver';
import { ScreeningService } from './../../shared/services/screening.service';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
@NgModule({
    imports: [SharedModule, DashboardTeRoutingModule, TableModule, MultiSelectModule],
    declarations: [DashboardTEComponent],
    providers: [TestExpertItemListResolver, ScreeningService],
    exports: [DashboardTEComponent]
})

export class DashboardTEModule { }
