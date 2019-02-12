import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../../shared/services/user.service';
import { WishlistService } from '../../../shared/services/wishlist.service';
import { WishlistItem, ItemSummary } from '../../../shared/model/model';
import { ManagerWishlistItem } from '../../../shared/model/frontendmodel';
import { Table } from 'primeng/table';
import { environment } from './../../../../environments/environment';
import { Subscription, forkJoin } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { truncate } from 'src/app/shared/helpers/utils';


@Component({
    moduleId: module.id,
    templateUrl: 'list.page.component.html'
})

export class ListComponent implements OnInit, OnDestroy {
    @ViewChild(Table) table: Table;
    listOfWishlistItem = new Array<WishlistItem>();
    listOfItems = new Array<ItemSummary>();
    showsidebar = false;
    listIsEmpty = false;
    wishListId: string;
    itemCodes: string;
    cols: any[] = [];
    private routeSubscription: Subscription;

    constructor(private route: ActivatedRoute,
        private userService: UserService,
        private wishListService: WishlistService, private translateService: TranslateService) { }

    ngOnInit() {
        if (this.userService.user.selectedWishlist) {
            this.wishListService.get(this.userService.user.selectedWishlist.id).subscribe(w => this.wishListId = w.id);
            this.routeSubscription = this.route.data.subscribe(data => {
                this.listOfWishlistItem = data['list'];
                this.listIsEmpty = (!this.listOfWishlistItem || this.listOfWishlistItem.length === 0);
            });
            this.translateService.get('COLUMN_HEADER_MA_LEARNINGOBJECTIVE_CODE').subscribe(lo => {
                this.cols = [
                    { field: 'learningobjectiveCode', header: lo },
                    { field: 'learningobjective', header: this.translateService.instant('COLUMN_HEADER_MA_LEARNINGOBJECTIVE') },
                    { field: 'deadline', header: this.translateService.instant('COLUMN_HEADER_MA_DEADLINE') },
                    { field: 'totalCount', header: this.translateService.instant('COLUMN_HEADER_MA_ITEMS') },
                    { field: 'doneCount', header: this.translateService.instant('COLUMN_HEADER_MA_COMPLETED') },
                    { field: 'constructionCount', header: this.translateService.instant('COLUMN_HEADER_MA_@CONSTRUCTOR') },
                    { field: 'screeningCount', header: this.translateService.instant('COLUMN_HEADER_MA_@TESTEXPERT') },
                    { field: 'rejectedCount', header: this.translateService.instant('COLUMN_HEADER_MA_REJECTED') }
                ];
            });
        }
    }
    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
    }
    handleRowExpand(row: any) {
        const wishlistItem = row.data as ManagerWishlistItem;
        this.listOfItems = new Array<ItemSummary>();
        if (wishlistItem.itemsCreated === true) {
            forkJoin(
                this.wishListService.getItems(wishlistItem.wishlistId, wishlistItem.learningObjectiveId),
                this.translateService.get('NOT_FILLED'))
                .subscribe(([l, emptyValue]) =>
                    this.listOfItems = l.map(i => ({ ...i, bodyText: truncate(i.bodyText, 40, emptyValue) }))
                );
        }
    }

    download21 = () => window.open(`${environment.api}/${this.wishListId}/qtipackage21`, '_blank', '');
    downloadQuestifyBackoffice = () => window.open(`${environment.api}Wishlists/${this.wishListId}/questifybackoffice`, '_blank', '');
    getFeedback = () => window.open(`${environment.api}Wishlists/${this.wishListId}/feedback`, '_blank', '');
    downloadItem21 = (itemId: string) => window.open(`${environment.api}Items/${itemId}/qtipackage21`, '_blank', '');
    download = () => window.open(`${environment.api}Wishlists/${this.wishListId}/qtipackage`, '_blank', '');
    getWordAccepted = () => window.open(`${environment.api}Wishlists/${this.wishListId}/WordExportForAcceptedItems`, '_blank', '');
    getWordAll = () => window.open(`${environment.api}Wishlists/${this.wishListId}/WordExport`, '_blank', '');
    downloadItem = (itemId: string) => window.open(`${environment.api}Items/${itemId}/qtipackage`, '_blank', '');
    getWordDocument() {
        const codes = this.itemCodes.split(/\r?\n/);
        this.wishListService.getWordExport(this.wishListId, codes);
    }
}


