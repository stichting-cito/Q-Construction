import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../../shared/services/user.service';
import { WishlistService } from '../../../shared/services/wishlist.service';
import { ScreeninglistService } from '../../../shared/services/screeninglist.service';
import { TranslateService } from '@ngx-translate/core';
import { ScreeningList, KeyValue } from '../../../shared/model/model';
import { DashboardData } from '../../../shared/model/frontendmodel';
import { FileUploader, FileUploaderOptions } from 'ng2-file-upload/file-upload/file-uploader.class';
import { ConfirmationService, Confirmation } from 'primeng/components/common/api';
import { environment } from './../../../../environments/environment';
import { Subscription } from 'rxjs';
@Component({
    templateUrl: 'setup.dashboard.component.html'
})

export class DashboardSetupComponent implements OnInit, OnDestroy {
    isopen = false;
    itemsCreated = true;
    uploaderWishlist: FileUploader;
    uploaderScreeninglist: FileUploader;
    screeningsLists: KeyValue[];
    newScreeningslist = new ScreeningList();
    selectedScreeningList: KeyValue;
    private routeSubscription: Subscription;
    public dashboardData: DashboardData;
    constructor(private translateService: TranslateService, private confirmationService: ConfirmationService, private route: ActivatedRoute,
        // tslint:disable-next-line:align
        private screeninglistService: ScreeninglistService, private wishlistService: WishlistService, private userService: UserService) {
    }

    ngOnInit() {
        if (this.userService.user.selectedWishlist) {
            this.routeSubscription = this.route.parent.data.subscribe(data => {
                this.dashboardData = data.data;
                this.itemsCreated = this.dashboardData.wishlist.wishListItems.filter(w => w.numberOfItems !== w.todo).length > 0;
                this.setScreeningListInfo(this.dashboardData.wishlist.screeningsListId);
                const wishlistUploadOptions: FileUploaderOptions = {
                    url: `${environment.api}${this.wishlistService.endPoint}/
                  ${this.userService.user.selectedWishlist.id}/FillWishList`,
                    authToken: localStorage.getItem('id_token')
                };
                const screeninglistUploadOptions: FileUploaderOptions = {
                    url: `${environment.api}${this.wishlistService.endPoint}/
                  ${this.userService.user.selectedWishlist.id}/FillScreeningList`,
                    authToken: localStorage.getItem('id_token')
                };
                this.uploaderWishlist = new FileUploader(wishlistUploadOptions);
                this.uploaderScreeninglist = new FileUploader(screeninglistUploadOptions);
            });
        }
    }

    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
    }

    fillWishlist() {
        this.uploaderWishlist.onCompleteAll = () => {
            const conf: Confirmation = {
                icon: 'fa fa-check-circle',
                message: this.translateService.instant('SUCCESFULLY_ADDED_DATA_TO_WISHLIST'),
                accept: () => {
                    this.uploaderWishlist.clearQueue();
                },
                rejectVisible: false
            };
            this.confirmationService.confirm(conf);
        };
        this.uploaderWishlist.uploadAll();
    }

    addScreeningList() {
        this.screeninglistService.addNew(this.newScreeningslist).subscribe(s => {
            this.uploaderScreeninglist.queue[0].url = `${environment.api}${this.screeninglistService.endPoint}/${s.id}/FillScreeningList`;
            this.uploaderScreeninglist.onCompleteAll = () => {
                const conf: Confirmation = {
                    icon: 'fa fa-check-circle',
                    message: this.translateService.instant('SUCCESFULLY_ADDED_SCREENING_LIST'),
                    accept: () => {
                        this.uploaderScreeninglist.clearQueue();
                    },
                    rejectVisible: false
                };
                this.wishlistService.setScreeningList(this.userService.user.selectedWishlist.id, s.id).subscribe(() => {
                    this.setScreeningListInfo(s.id);
                    this.confirmationService.confirm(conf);
                });
            };
            this.uploaderScreeninglist.uploadAll();
        });
    }

    setScreeningListInfo(id: string) {
        this.screeninglistService.getAll().subscribe((sls) => {
            const l = new Array<KeyValue>();
            sls.forEach(sl => l.push({ id: sl.id, value: sl.title }));
            this.screeningsLists = l;
            const selected = this.screeningsLists.filter(sl => sl.id === id);
            this.selectedScreeningList = (selected && selected.length > 0) ?
                selected[0] : { id: '', value: this.translateService.instant('SELECT_SCREENINGLIST') };
        });
    }

    setSelectedScreeningList(value: KeyValue) {
        this.selectedScreeningList = value;
        this.wishlistService.setScreeningList(this.userService.user.selectedWishlist.id, value.id).subscribe();
    }

    deleteWishlist() {
        this.translateService.get('CONFIRMATION_DELETE_WISHLIST').subscribe((text: string) => {
            const conf: Confirmation = {
                icon: 'fa fa-trash',
                message: text,
                accept: () => {
                    this.wishlistService.delete(this.userService.user.selectedWishlist.id).subscribe(() => {
                        if (this.userService.user.allowedWishlists && this.userService.user.allowedWishlists.length !== 0) {
                            this.userService.user.selectedWishlist = this.userService.user.allowedWishlists[0];
                        }
                        this.userService.routeUser(true);
                    });
                }
            };
            this.confirmationService.confirm(conf);
        });
    }
}
