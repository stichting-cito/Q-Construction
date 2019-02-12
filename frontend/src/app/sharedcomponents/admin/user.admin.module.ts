import { NgModule } from '@angular/core';
import { SharedModule } from './../../shared/shared.module';
import { UserAdminDialogComponent } from './user.admin.component';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    imports: [SharedModule, ModalModule],
    declarations: [UserAdminDialogComponent],
    providers: [],
    exports: [UserAdminDialogComponent]
})

export class UserAdminDialogModule { }

