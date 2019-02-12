import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ReviewComponent } from './reviewitem.page.component';
import { ItemResolver } from '../../shared/resolvers/item.resolver';
import { ScreeningResolver } from '../../shared/resolvers/screening.resolver';
import { TestExpertAuthGuard } from './../../shared/guards/testexpert-guard.service';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: 'screen/:id', component: ReviewComponent, resolve: { item: ItemResolver, screening: ScreeningResolver },
                canActivate: [TestExpertAuthGuard],
            }
        ])
    ],
    exports: [RouterModule]
})
export class ReviewRoutingModule { }
