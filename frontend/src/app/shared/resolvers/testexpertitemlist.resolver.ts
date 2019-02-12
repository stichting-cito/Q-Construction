import { MenuItemTestExpert, TestExportListItem } from '../../shared/model/frontendmodel';
import { Injectable } from '@angular/core';
import { DatePipe } from '@angular/common';

import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, Observer, forkJoin, of } from 'rxjs';

import { TranslateService } from '@ngx-translate/core';
import { ItemSummary, ItemStatus } from '../model/model';

import { ItemService } from '../../shared/services/item.service';
import { UserService } from '../../shared/services/user.service';

import { TruncatePipe } from '../../shared/pipes/truncate';
import { ItemStatePipe } from '../../shared/pipes/itemstate';
import { DateTimeCSharpPipe } from '../../shared/pipes/datetimecsharp';
import { flatMap, map } from 'rxjs/operators';

@Injectable()
export class TestExpertItemListResolver implements Resolve<Array<TestExportListItem>> {
    constructor(private itemService: ItemService, private userService: UserService,
        private translateService: TranslateService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Array<TestExportListItem>> {
        const menuItem = <MenuItemTestExpert>+MenuItemTestExpert[route.params['menuItem']];
        const states: ItemStatus[] = [ItemStatus.accepted, ItemStatus.readyForReview, ItemStatus.needsWork, ItemStatus.rejected];
        const callToGetList = (menuItem === MenuItemTestExpert.todo) ?
            this.itemService.getItemSummariesToDoForTestExpert(this.userService.user.id, this.userService.user.selectedWishlist.id) :
            this.itemService.getMyListByLatestScreeningAndStates(this.userService.user.id,
                this.userService.user.selectedWishlist.id, states);
        return callToGetList.pipe(flatMap(l => this.mapItemSummaryOnTestExportListItem(l)));
    }
    mapItemSummaryOnTestExportListItem(summary: ItemSummary[]): Observable<TestExportListItem[]> {
        if (summary && summary.length > 0) {
            const list = summary.map(s => {
                return forkJoin(
                    new TruncatePipe(this.translateService).transform(s.bodyText, 40),
                    new ItemStatePipe(this.translateService).transform(s.itemStatus)
                ).pipe(map(([name, state]) => {
                    const teItem: TestExportListItem = {
                        id: s.id,
                        uniqueCode: s.uniqueCode,
                        name: name,
                        learningobjective: s.learningObjectiveTitle,
                        latestScreeningId: s.latestScreeningId,
                        deadline: new DatePipe('nl').transform(new DateTimeCSharpPipe().transform(s.deadline), 'dd-MM-yyyy'),
                        author: s.author,
                        state: state,
                        lastModified: new DatePipe('nl')
                            .transform(new DateTimeCSharpPipe().transform(s.lastModified), 'dd-MM-yyyy')
                    };
                    return teItem;
                }));
            });
            return forkJoin(list).pipe(map((data: Array<TestExportListItem>) => data));
        } else {
            return of(new Array<TestExportListItem>());
        }
    }
}
