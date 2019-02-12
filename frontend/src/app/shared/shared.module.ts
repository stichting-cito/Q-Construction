import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { BsDropdownModule } from 'ngx-bootstrap';
import { ScreeningComponent } from './../sharedcomponents/screening/screening.component';
import { TipsComponent } from './../sharedcomponents/tips/tips.component';
import { ItemModule } from './../sharedcomponents/item/item.module';
import { UserService } from './../shared/services/user.service';
import { ItemService } from './../shared/services/item.service';
import { WishlistService } from './../shared/services/wishlist.service';
import { LearningObjectiveService } from './../shared/services/learningobjective.service';

import { PipesModule } from './pipes/pipes.module';
import { DirectivesModule } from './directives/directives.module';

@NgModule({
    imports: [CommonModule, FormsModule, ReactiveFormsModule, PipesModule, DirectivesModule, RouterModule,
        BsDropdownModule.forRoot(), TranslateModule, ItemModule],
    declarations: [ScreeningComponent, TipsComponent],
    exports: [
        ScreeningComponent, TipsComponent, ItemModule,
        PipesModule, DirectivesModule, CommonModule, FormsModule, ReactiveFormsModule, RouterModule,
        BsDropdownModule, TranslateModule]
})

export class SharedModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: SharedModule,
            providers: [UserService, ItemService, WishlistService, LearningObjectiveService],
        };
    }
}
