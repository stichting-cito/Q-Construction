import { NgModule } from '@angular/core';
import { LoginComponent } from './login.page.component';
import { LoginRoutingModule } from './login.page.routing.module';
import { SharedModule } from '../../shared/shared.module';
import { LoginAuthGuard } from './../../shared/guards/login-guard.service';

@NgModule({
    imports: [ SharedModule, LoginRoutingModule],
    providers: [LoginAuthGuard],
    declarations: [LoginComponent],
    exports: [LoginComponent]
})

export class LoginModule { }
