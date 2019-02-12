import { Route } from '@angular/router';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DashboardCOComponent } from './dashboard.co.page.component';
import { EditComponent } from '../edititem/edititem.page.component';
import { ViewComponent } from '../viewitem/viewitem.page.component';
import { LearningObjectiveComponent } from '../learningobjective/learningobjective.page.component';
import { ItemsummaryComponent } from './itemsummary.component';
import { ItemResolver } from '../../shared/resolvers/item.resolver';
import { ScreeningResolver } from '../../shared/resolvers/screening.resolver';
import { ItemStatusCountResolver } from '../../shared/resolvers/itemstatuscount.resolver';
import { ItemSummaryResolver } from '../../shared/resolvers/itemsummary.resolver';
import { LearningObjectivesResolver } from '../../shared/resolvers/learningobjectives.resolver';
import { ConstructorAuthGuard } from './../../shared/guards/constructor-guard.service';
import { DisabledItemTypeResolver } from './../../shared/resolvers/disableditemtypes.resolver';
// Volgorde van routes is belangrijk, aangezien :state arbitrary is, resolved
// de router eerst edit/view en learningobjective, en dan pas state. !!!

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: ':state',
                component: DashboardCOComponent, canActivate: [ConstructorAuthGuard],
                resolve: { itemstatecounts: ItemStatusCountResolver }, children: [
                    {
                        path: 'concept/:id', component: EditComponent,
                        resolve: { item: ItemResolver, disabledItemTypes: DisabledItemTypeResolver }
                    },
                    {
                        path: 'edit/:id', component: EditComponent,
                        resolve: { item: ItemResolver, screening: ScreeningResolver, disabledItemTypes: DisabledItemTypeResolver }
                    },
                    { path: 'view/:id', component: ViewComponent, resolve: { item: ItemResolver } },
                    {
                        path: 'learningobjective', component: LearningObjectiveComponent,
                        resolve: { learningobjectives: LearningObjectivesResolver }
                    },
                    { path: '', component: ItemsummaryComponent, resolve: { listofitems: ItemSummaryResolver } }]
            }
        ])
    ],
    exports: [RouterModule]
})

export class DashboardCORoutingModule { }
