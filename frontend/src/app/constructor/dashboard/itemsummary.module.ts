import { NgModule } from '@angular/core';
import { ItemsummaryComponent } from './itemsummary.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
    imports: [SharedModule],
    declarations: [ItemsummaryComponent],
    exports: [ItemsummaryComponent]
})

export class ItemsummaryModule { }
