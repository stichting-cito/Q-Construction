import { NgModule } from '@angular/core';
import { DashboardCOComponent } from './dashboard.co.page.component';
import { DashboardCORoutingModule } from './dashboard.co.page.routing.module';
import { EditModule } from './../edititem/edititem.page.module';
import { ViewModule } from './../viewitem/viewitem.page.module';
import { LearningObjectiveModule } from './../learningobjective/learningobjective.page.module';
import { ItemsummaryModule } from './itemsummary.module';
import { SharedModule } from './../../shared/shared.module';
import { ItemResolver } from './../../shared/resolvers/item.resolver';
import { ScreeningResolver } from './../../shared/resolvers/screening.resolver';
import { ItemStatusCountResolver } from './../../shared/resolvers/itemstatuscount.resolver';
import { LearningObjectivesResolver } from './../../shared/resolvers/learningobjectives.resolver';
import { ItemSummaryResolver } from './../../shared/resolvers/itemsummary.resolver';
import { ScreeningService } from '../../shared/services/screening.service';
@NgModule({
    imports: [SharedModule, DashboardCORoutingModule, EditModule, ViewModule, LearningObjectiveModule, ItemsummaryModule],
    declarations: [DashboardCOComponent],
    providers: [LearningObjectivesResolver, ItemResolver, ScreeningResolver, ScreeningService, ItemStatusCountResolver,
         ItemSummaryResolver],
    exports: [DashboardCOComponent]
})

export class DashboardCOModule { }
