import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

@NgModule(
    {
        imports: [
            RouterModule.forRoot([
                //  () => import(`./cart/cart.module`).then(m => m.CartModule) }
                {
                    path: 'dashboard_ma', loadChildren: () =>
                        import(`./manager/dashboard/dashboard.ma.page.module`).then(m => m.DashboardMAModule)
                },
                {
                    path: 'dashboard_co', loadChildren: () =>
                        import(`./constructor/dashboard/dashboard.co.page.module`).then(m => m.DashboardCOModule)
                },
                {
                    path: 'dashboard_te', loadChildren: () =>
                        import(`./testexpert/dashboard/dashboard.te.page.module`).then(m => m.DashboardTEModule)
                }
            ])
        ],
        exports: [RouterModule]
    })
export class AppRoutingModule { }
