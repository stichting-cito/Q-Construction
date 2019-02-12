import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

import { ItemType } from '../model/model';
import { WishlistService } from '../../shared/services/wishlist.service';
import { UserService } from '../../shared/services/user.service';

@Injectable()
export class DisabledItemTypeResolver implements Resolve<Array<ItemType>> {
    constructor(private wishlistService: WishlistService, private userService: UserService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Array<ItemType>> {
        return this.wishlistService.getDisabledItemTypes(this.userService.user.selectedWishlist.id);
    }
}
