import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ViewComponent } from './viewitem.page.component';
import { ConstructorAuthGuard } from './../../shared/guards/constructor-guard.service';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: 'view/:id',
                component: ViewComponent,
                canActivate: [ConstructorAuthGuard]
            }
        ])
    ],
    exports: [RouterModule]
})
export class ViewItemRoutingModule { }
