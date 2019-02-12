import { Component, ViewChild, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ItemWrapperComponent } from '../../sharedcomponents/item/item.component';
import { ItemService } from '../../shared/services/item.service';
import { ScreeningService } from '../../shared/services/screening.service';
import { Item, Screening, ItemType, Wishlist } from '../../shared/model/model';
import { Subscription } from 'rxjs';
@Component({
    moduleId: module.id,
    templateUrl: 'edititem.page.component.html',
})
export class EditComponent implements OnInit, OnDestroy {
    @ViewChild(ItemWrapperComponent) itemComponentWrapper: ItemWrapperComponent;
    item: Item;
    disabledItemTypes = new Array<ItemType>();
    screening: Screening;
    dashboardstate: string;
    isValid = false;
    routeSubscription: Subscription;
    parentRouteSubscription: Subscription;

    constructor(private route: ActivatedRoute, private itemService: ItemService, private router: Router) { }
    ngOnInit() {
        this.routeSubscription = this.route.data.subscribe(data => {
            this.item = data['item'];
            this.disabledItemTypes = data['disabledItemTypes'];
            this.screening = data['screening'];
        });
        this.parentRouteSubscription = this.route.parent.params.subscribe(params => {
            this.dashboardstate = params['state'];
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

    delete() {
        this.itemService.delete(this.item)
            .subscribe(() => this.router.navigate(['/dashboard_co/', this.dashboardstate],
                { queryParams: { '': Date.now() } }));
    }

    onFormValidChanged(valid: boolean) {
        setTimeout(() => this.isValid = valid);
    }

    save() {
        this.itemService.save(this.item).subscribe();
    }

    file() {
        // Als je je item indient, herlaadt dan het item en waarschijnlijk zie je dan het item in wysiwyg.
        this.itemService.file(this.item).subscribe(() => {
            this.router.navigate(['/dashboard_co/', this.dashboardstate], {
                queryParams: { '': Date.now() }
            });
        });
    }

    back() {
        this.router.navigate(['/dashboard_co/', this.dashboardstate], {
            queryParams: { '': Date.now() }
        });
    }
}





