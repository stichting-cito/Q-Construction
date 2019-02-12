import { NgModule } from '@angular/core';
import { EditComponent } from './edititem.page.component';
import { SharedModule } from './../../shared/shared.module';
import { ScreeningService } from './../../shared/services/screening.service';
import { EditItemRoutingModule } from './edititem.page.routing.module';
import { ItemResolver } from './../../shared/resolvers/item.resolver';
import { DisabledItemTypeResolver } from './../../shared/resolvers/disableditemtypes.resolver';
@NgModule({
    imports: [ SharedModule, EditItemRoutingModule],
    declarations: [EditComponent],
    providers: [ScreeningService, ItemResolver, DisabledItemTypeResolver],
    exports: [EditComponent]
})

export class EditModule { }
