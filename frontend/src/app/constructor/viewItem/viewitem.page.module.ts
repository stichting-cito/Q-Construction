import { NgModule } from '@angular/core';
import { ViewComponent } from './viewitem.page.component';
import { ViewItemRoutingModule } from './viewitem.page.routing.module';
import { SharedModule } from './../../shared/shared.module';
@NgModule({
    imports: [SharedModule, ViewItemRoutingModule],
    declarations: [ViewComponent],
    exports: [ViewComponent]
})

export class ViewModule { }
