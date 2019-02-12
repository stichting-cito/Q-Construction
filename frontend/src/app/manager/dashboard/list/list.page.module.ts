import { NgModule } from '@angular/core';
import { ListComponent } from './list.page.component';
import { SharedModule } from './../../../shared/shared.module';
import { TableModule } from 'primeng/table';
import { ManagerWishlistItemListResolver } from './../../../shared/resolvers/managerwishlistitemlist.resolver';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
@NgModule({
    imports: [SharedModule, TableModule, ModalModule, BsDropdownModule],
    declarations: [ListComponent],
    providers: [ManagerWishlistItemListResolver],
    exports: [ListComponent]
})

export class ListModule { }
