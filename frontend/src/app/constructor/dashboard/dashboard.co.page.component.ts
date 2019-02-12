import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UserType, ItemStatusCount } from '../../shared/model/model';
import { ItemService } from '../../shared/services/item.service';
import { UserService } from '../../shared/services/user.service';
import { ItemStatus } from '../../shared/model/model';
import { Subscription } from 'rxjs';

@Component({
    moduleId: module.id,
    templateUrl: 'dashboard.co.page.component.html',
    styles: [`
        .list-group-item {
            white-space: nowrap;
        }`]
})

export class DashboardCOComponent implements OnInit, OnDestroy {
    itemCountPerStateList: Array<ItemStatusCount> = new Array<ItemStatusCount>();
    showitemsummary: boolean;
    selectedState: ItemStatus;
    showsidebar = false;
    listIsEmpty = false;
    routeSubscription: Subscription;
    userType: UserType;
    routeParamsSubscription: Subscription;
    routeQueryParamsSubscription: Subscription;

    constructor(
        private route: ActivatedRoute,
        private userService: UserService,
        private itemService: ItemService,
        private router: Router
    ) { }

    ngOnInit() {
        this.userType = this.userService.user.userType;
        this.routeSubscription = this.route.data.subscribe(data => this.itemCountPerStateList = data['itemstatecounts']);
        this.routeParamsSubscription = this.route.params.subscribe(params => this.selectedState = <ItemStatus>+ItemStatus[params['state']]);
        this.routeQueryParamsSubscription =  this.route.queryParams.subscribe(() => {
            // ngrxstore could help us out here. this is a workaround to force reloading the component.
            this.itemService.getStateCount(this.userService.user.id, this.userService.user.selectedWishlist.id)
                .subscribe(data => this.itemCountPerStateList = data);
        });
    }

    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
        if (this.routeParamsSubscription) {
            this.routeParamsSubscription.unsubscribe();
        }
        if (this.routeQueryParamsSubscription) {
            this.routeQueryParamsSubscription.unsubscribe();
        }
    }

    getCountByState(state: number, recursive: boolean): number {
        let returnValue = 0;
        // Check if the state requested is in the array
        const filteredState = this.itemCountPerStateList.filter(l => l.itemStatus === state);
        // If so, return the count
        if (filteredState && filteredState.length === 1) { returnValue = filteredState[0].count; }
        return (!recursive && state === ItemStatus.readyForReview) ?
            returnValue + this.getCountByState(ItemStatus.inReview, true) : returnValue;
    }

    getItemsByState(state: number) {
        this.showsidebar = false;
        this.router.navigate(['/dashboard_co/', ItemStatus[state]]);
    }

    newItem() {
        this.showsidebar = false;
        this.router.navigate(['/dashboard_co/', ItemStatus[this.selectedState], 'learningobjective']);
    }

    toggleSidebar() {
        this.showsidebar = !this.showsidebar;
    }
}


