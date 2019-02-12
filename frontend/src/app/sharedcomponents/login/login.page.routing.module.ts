import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './login.page.component';
import { LoginAuthGuard } from './../../shared/guards/login-guard.service';
@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                redirectTo: '/login',
                pathMatch: 'full'
            },
            {
                path: 'login',
                component: LoginComponent,
                canActivate: [LoginAuthGuard]
            }
        ])
    ],
    exports: [RouterModule]
})
export class LoginRoutingModule { }
