import { NgModule } from '@angular/core';
import { SharedModule } from './../../shared/shared.module';
import { DashboardMAComponent } from './dashboard.ma.page.component';
import { TabMenuModule } from 'primeng/components/tabmenu/tabmenu';

import { StatsResolver } from './../../shared/resolvers/stats.resolver';
import { ManagerWishlistItemListResolver } from './../../shared/resolvers/managerwishlistitemlist.resolver';

import { StatsService } from '../../shared/services/stats.service';
import { ScreeningService } from '../../shared/services/screening.service';

import { StatsModule } from './stats/stats.module';
import { ListModule } from './list/list.page.module';
import { QualityModule } from './quality/quality.module';
import { SetupModule } from './setup/setup.module';
import { UsersModule } from './users/users.module';

import { DashboardMARoutingModule } from './dashboard.ma.page.routing.module';
import { OrganisationAdministrationModule } from './organisation/organisation.admin.page.module';

@NgModule({
    imports: [SharedModule, DashboardMARoutingModule, ListModule, StatsModule, QualityModule,
        SetupModule, UsersModule, OrganisationAdministrationModule, TabMenuModule],
    declarations: [DashboardMAComponent],
    providers: [StatsResolver, ManagerWishlistItemListResolver, StatsService, ScreeningService],
    exports: [DashboardMAComponent]
})

export class DashboardMAModule { }
