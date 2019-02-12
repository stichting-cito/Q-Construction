import { NgModule } from '@angular/core';
import { SharedModule } from './../../shared/shared.module';
import { NavbarComponent } from './navbar.component';
import { ModalModule, BsModalService } from 'ngx-bootstrap';

@NgModule({
    imports: [SharedModule, ModalModule],
    exports: [NavbarComponent],
    providers: [BsModalService],
    declarations: [NavbarComponent]
})
export class NavbarModule { }
