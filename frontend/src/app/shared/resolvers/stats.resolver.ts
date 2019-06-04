import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, forkJoin, of } from 'rxjs';

import { StatsPerWishlist, WishlistItem, UserType } from '../model/model';
import { DashboardData } from '../model/frontendmodel';
import { UserService } from '../../shared/services/user.service';
import { StatsService } from '../../shared/services/stats.service';
import { WishlistService } from '../../shared/services/wishlist.service';
import { ScreeningService } from '../../shared/services/screening.service';
import { map } from 'rxjs/operators';

@Injectable()
export class StatsResolver implements Resolve<StatsPerWishlist> {
    constructor(private wishlistService: WishlistService, private statsService: StatsService,
        // tslint:disable-next-line:align
        private screeningService: ScreeningService, private userService: UserService) { }

    resolve(): Observable<StatsPerWishlist> {
        return this.userService.user.selectedWishlist ?
            this.GetAllData(this.userService.user.selectedWishlist.id) : null;
    }
    // StatsPerWishlist
    private GetAllData(wishlistId: string): Observable<StatsPerWishlist> {
        return forkJoin(
            this.statsService.get(wishlistId),                     // 0
            this.screeningService.getScreeningList(wishlistId),    // 1
            this.userService.user.userType === UserType.restrictedManager ?
                of(null) : this.userService.All(),                 // 2
            this.wishlistService.get(wishlistId)                   // 3
        ).pipe(map(([stats, screeningslist, users, wishlist]) => {
            const deadlines = wishlist.wishListItems.map((wi1: WishlistItem) => wi1.deadline);
            const dashboardData: DashboardData = {
                ...stats,
                wishlist,
                deadlines: deadlines
                    .filter((itm: any, i: any) => deadlines.indexOf(itm) === i).map((d: Date) => {
                        const count = wishlist.wishListItems.filter((wi1: WishlistItem) => wi1.deadline === d)
                            .map((wi2: WishlistItem) => wi2.numberOfItems)
                            .reduce((a: number, b: number) => a + b, 0);
                        return { date: d, count };
                    }),
                users,
                screeningsList: screeningslist
            };
            return dashboardData;
        }));
    }
}
