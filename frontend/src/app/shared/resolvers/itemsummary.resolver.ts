import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

import { ItemSummary, ItemStatus } from '../model/model';
import { ItemService } from '../../shared/services/item.service';
import { UserService } from '../../shared/services/user.service';

@Injectable()
export class ItemSummaryResolver implements Resolve<Array<ItemSummary>> {
    constructor(private itemService: ItemService, private userService: UserService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<Array<ItemSummary>> {
        const states: Array<ItemStatus> = new Array<ItemStatus>();
        states.push(+ItemStatus[route.params.state]);
        if (states[0] === ItemStatus.readyForReview) {
            states.push(ItemStatus.inReview);
        }
        return this.itemService.getMyListByStates(states, this.userService.user.id, this.userService.user.selectedWishlist.id);
    }
}
