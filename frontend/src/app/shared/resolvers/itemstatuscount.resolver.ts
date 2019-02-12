import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

import { ItemStatusCount } from '../model/model';
import { ItemService } from '../../shared/services/item.service';
import { UserService } from '../../shared/services/user.service';

@Injectable()
export class ItemStatusCountResolver implements Resolve<Array<ItemStatusCount>> {
    constructor(private itemService: ItemService, private userService: UserService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Array<ItemStatusCount>> {
        return this.itemService.getStateCount(this.userService.user.id, this.userService.user.selectedWishlist.id);
    }
}
