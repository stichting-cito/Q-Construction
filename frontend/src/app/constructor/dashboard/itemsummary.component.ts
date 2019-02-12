import { Component, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { Item, ItemSummary, ItemStatus } from '../../shared/model/model';
import { TranslateService } from '@ngx-translate/core';
import { UserService } from '../../shared/services/user.service';
import { ItemService } from '../../shared/services/item.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';


@Component({
    moduleId: module.id,
    selector: 'app-item-summary',
    templateUrl: 'itemsummary.component.html'
})
export class ItemsummaryComponent implements OnInit, OnDestroy {
    listOfItems: Array<ItemSummary> = new Array<ItemSummary>();
    selectitem: EventEmitter<any> = new EventEmitter();
    selectedState: ItemStatus;
    routeSubscription: Subscription;
    constructor(
        private route: ActivatedRoute,
        private itemService: ItemService,
        private userService: UserService,
        private router: Router
    ) { }

    ngOnInit() {
       this.routeSubscription = this.route.data.subscribe(data => {
            this.selectedState = +ItemStatus[this.route.parent.snapshot.params['state']];
            this.listOfItems = data['listofitems'];
            this.route.params.subscribe(params => {
                this.routeChanged(+ItemStatus[params['state']]);
            });
        });
    }

    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
    }

    routeChanged(currentState: number) {
        if (this.selectedState !== currentState) {
            const states: Array<ItemStatus> = new Array<ItemStatus>();
            states.push(currentState);
            if (states[0] === ItemStatus.readyForReview) {
                states.push(ItemStatus.inReview);
            }
            this.itemService.getMyListByStates(states, this.userService.user.id, this.userService.user.selectedWishlist.id)
                .subscribe((l: any) => this.listOfItems = l);
        }
        this.selectedState = currentState;
    }

    select(item: Item) {
        const status: number = +item.itemStatus;
        let url = '/view';
        switch (status) {
            case ItemStatus.draft:
                url = '/concept';
                break;
            case ItemStatus.needsWork:
                url = '/edit';
                break;
        }
        this.router.navigate(['/dashboard_co/' + ItemStatus[status] + '/' + url + '/' + item.id]);
    }

    daysLeftRelavant(item: ItemSummary) {
        if (item) {
            switch (item.itemStatus) {
                case ItemStatus.draft:
                case ItemStatus.readyForReview:
                case ItemStatus.inReview:
                case ItemStatus.needsWork: {
                    return true;
                }
            }
        }
        return false;
    }
}
