import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DashboardMAComponent } from './dashboard.ma.page.component';
import { StatsResolver } from '../../shared/resolvers/stats.resolver';
import { ManagerAuthGuard } from './../../shared/guards/manager-guard.service';
import { DashboardQualityComponent } from './quality/quality.dashboard.component';
import { DashboardUsersComponent } from './users/users.dashboard.component';
import { DashboardStatsComponent } from './stats/stats.dashboard.component';
import { DashboardSetupComponent } from './setup/setup.dashboard.component';
import { ListComponent } from './list/list.page.component';
import { ManagerWishlistItemListResolver } from './../../shared/resolvers/managerwishlistitemlist.resolver';
import { OrganisationAdministrationComponent } from './organisation/organisation.admin.page.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '', component: DashboardMAComponent,
                resolve: { data: StatsResolver }, canActivate: [ManagerAuthGuard], children: [
                    { path: '', component: DashboardStatsComponent },
                    { path: 'stats', component: DashboardStatsComponent },
                    { path: 'quality', component: DashboardQualityComponent },
                    { path: 'users', component: DashboardUsersComponent },
                    { path: 'setup', component: DashboardSetupComponent },
                    { path: 'list', component: ListComponent, resolve: { list: ManagerWishlistItemListResolver } },
                    { path: 'organisations', component: OrganisationAdministrationComponent }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class DashboardMARoutingModule { }
