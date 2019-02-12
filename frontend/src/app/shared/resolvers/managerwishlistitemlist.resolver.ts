
import { Injectable } from '@angular/core';
import { DatePipe } from '@angular/common';

import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

import { TranslateService } from '@ngx-translate/core';
import { WishlistItem } from '../model/model';
import { ManagerWishlistItem } from '../model/frontendmodel';

import { WishlistService } from '../../shared/services/wishlist.service';
import { UserService } from '../../shared/services/user.service';

import { DateTimeCSharpPipe } from '../../shared/pipes/datetimecsharp';
import { ItemStatusCountPipe } from '../../shared/pipes/itemstatuscount';
import { map } from 'rxjs/operators';

@Injectable()
export class ManagerWishlistItemListResolver implements Resolve<Array<ManagerWishlistItem>> {
    constructor(private wishlistService: WishlistService, private userService: UserService) { }

    resolve(): Observable<Array<ManagerWishlistItem>> {
        return this.userService.user.selectedWishlist ?
            this.wishlistService.get(this.userService.user.selectedWishlist.id)
                .pipe(map(w => (w) ? this.mapWishListItemOnManagerWishlistItem(w.wishListItems) : null)) : null;
    }
    mapWishListItemOnManagerWishlistItem(wishListItems: WishlistItem[]): ManagerWishlistItem[] {
        return wishListItems.map(wi => {
            const teItem: ManagerWishlistItem = {
                id: wi.id,
                learningobjectiveCode: wi.learningObjectiveCode,
                learningobjective: wi.learningObjectiveTitle,
                deadline: new DatePipe('nl').transform(new DateTimeCSharpPipe().transform(wi.deadline), 'dd-MM-yyyy'),
                totalCount: wi.numberOfItems,
                doneCount: +(new ItemStatusCountPipe().transform([4], wi.itemStatusCount)),
                rejectedCount: +(new ItemStatusCountPipe().transform([5], wi.itemStatusCount)),
                constructionCount: +(new ItemStatusCountPipe().transform([0, 3], wi.itemStatusCount)),
                screeningCount: +(new ItemStatusCountPipe().transform([1, 2], wi.itemStatusCount)),
                wishlistId: wi.wishlistId,
                learningObjectiveId: wi.learningObjectiveId,
                itemsCreated: wi.todo !== wi.numberOfItems
            };
            return teItem;
        });
    }
}
