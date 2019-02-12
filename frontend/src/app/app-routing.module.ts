import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { environment } from './../environments/environment';

@NgModule(
    {
        imports: [
            RouterModule.forRoot([
                { path: 'dashboard_ma', loadChildren: './manager/dashboard/dashboard.ma.page.module#DashboardMAModule' },
                { path: 'dashboard_co', loadChildren: './constructor/dashboard/dashboard.co.page.module#DashboardCOModule' },
                { path: 'dashboard_te', loadChildren: './testexpert/dashboard/dashboard.te.page.module#DashboardTEModule' }
            ])
        ],
        exports: [RouterModule]
    })
export class AppRoutingModule { }
