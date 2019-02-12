import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DashboardTEComponent } from './dashboard.te.page.component';
import { TestExpertItemListResolver } from '../../shared/resolvers/testexpertitemlist.resolver';
import { TestExpertAuthGuard } from './../../shared/guards/testexpert-guard.service';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: ':menuItem', component: DashboardTEComponent,
                resolve: { list: TestExpertItemListResolver },
                canActivate: [TestExpertAuthGuard]
            }
        ])
    ],
    exports: [RouterModule]
})
export class DashboardTeRoutingModule { }
