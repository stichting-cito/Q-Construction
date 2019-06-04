import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { ItemWrapperComponent } from '../../sharedcomponents/item/item.component';
import { Router, ActivatedRoute } from '@angular/router';
import { Item } from '../../shared/model/model';
import { Subscription } from 'rxjs';

@Component({
    templateUrl: 'viewitem.page.component.html'
})
export class ViewComponent implements OnInit, OnDestroy {
    @ViewChild(ItemWrapperComponent, { static: false }) itemComponent: ItemWrapperComponent;
    item = new Item();
    routeSubscription: Subscription;
    parentRouteSubscription: Subscription;
    dashboardstate: string;

    constructor(
        private route: ActivatedRoute,
        private router: Router
    ) { }

    // We gaan er hier vanuit dat je altijd een item hebt.
    ngOnInit() {
        this.routeSubscription = this.route.data.subscribe(data => this.item = data.item);
        this.parentRouteSubscription = this.route.parent.params.subscribe(params => {
            this.dashboardstate = params.state;
        });
    }

    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
        if (this.parentRouteSubscription) {
            this.parentRouteSubscription.unsubscribe();
        }
    }

    back = () => this.router.navigate(['/dashboard_co/', this.dashboardstate]);
}





