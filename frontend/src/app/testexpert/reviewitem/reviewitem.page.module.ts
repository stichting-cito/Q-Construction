import { NgModule } from '@angular/core';
import { ReviewComponent } from './reviewitem.page.component';
import { ReviewRoutingModule } from './reviewitem.page.routing.module';
import { SharedModule } from './../../shared/shared.module';
import { ItemResolver } from '../../shared/resolvers/item.resolver';
import { ScreeningResolver } from '../../shared/resolvers/screening.resolver';
import { ScreeningService } from '../../shared/services/screening.service';

@NgModule({
    imports: [SharedModule, ReviewRoutingModule],
    declarations: [ReviewComponent],
    providers: [ItemResolver, ScreeningService, ScreeningResolver],
    exports: [ReviewComponent]
})

export class ReviewModule { }
