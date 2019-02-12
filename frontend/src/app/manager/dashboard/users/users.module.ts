import { NgModule } from '@angular/core';
import { DashboardUsersComponent } from './users.dashboard.component';
import { SharedModule } from './../../../shared/shared.module';
import { UserAdminDialogModule } from './../../../sharedcomponents/admin/user.admin.module';

@NgModule({
    imports: [SharedModule, UserAdminDialogModule],
    declarations: [DashboardUsersComponent],
    providers: [],
    exports: [DashboardUsersComponent]
})

export class UsersModule { }
