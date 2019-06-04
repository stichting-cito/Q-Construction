import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MenuItemTestExpert, TestExportListItem } from '../../shared/model/frontendmodel';
import { ScreeningService } from '../../shared/services/screening.service';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { SelectItem } from 'primeng/components/common/selectitem';

@Component({
    templateUrl: 'dashboard.te.page.component.html',
    styleUrls: ['dashboard.te.page.component.scss']
})
export class DashboardTEComponent implements OnInit, OnDestroy {
    cols: any[] = [];
    public listOfItems: Array<TestExportListItem> = new Array<TestExportListItem>();
    public listIsEmpty = false;
    public authors: SelectItem[] = [];
    public state: SelectItem[] = [];
    public menuItem: number;
    private routeSubscription: Subscription;
    private routeDataSubscription: Subscription;

    constructor(private route: ActivatedRoute, private router: Router,
        // tslint:disable-next-line:align
        private screeningService: ScreeningService, private translateService: TranslateService) { }

    ngOnInit() {
        this.routeSubscription = this.route.params
            .pipe(map(params => params.menuItem))
            .subscribe((menuItem) => this.menuItem = +MenuItemTestExpert[menuItem]);
        this.routeDataSubscription = this.route.data.subscribe(data => {
            this.listOfItems = data.list;
            this.authors = Array.from(new Set(this.listOfItems.map(i => i.author)))
                .map(v => ({ label: v, value: v }));
            this.state = Array.from(new Set(this.listOfItems.map(i => i.state)))
                .map(v => ({ label: v, value: v }));
        });
        this.translateService.get('COLUMN_HEADER_TE_CODE').subscribe(header => {
            this.cols = [{ field: 'uniqueCode', header },
            { field: 'name', header: this.translateService.instant('COLUMN_HEADER_TE_ITEM') },
            { field: 'learningobjective', header: this.translateService.instant('COLUMN_HEADER_TE_LEARNINGOBJECTIVE') },
            { field: 'deadline', header: this.translateService.instant('COLUMN_HEADER_TE_DEADLINE') },
            { field: 'state', header: this.translateService.instant('COLUMN_HEADER_TE_STATE') },
            { field: 'author', header: this.translateService.instant('COLUMN_HEADER_TE_AUTHOR') },
            { field: 'lastModified', header: this.translateService.instant('COLUMN_HEADER_TE_LAST_MODIFIED') }];
        });
    }

    ngOnDestroy(): void {
        if (this.routeSubscription) { this.routeSubscription.unsubscribe(); }
        if (this.routeDataSubscription) { this.routeDataSubscription.unsubscribe(); }
    }

    isNumeric(n: string) {
        return !isNaN(parseFloat(n)) && isFinite(+n);
    }

    getItemsByMenuItem(menuItem: MenuItemTestExpert) {
        this.router.navigate(['/dashboard_te', MenuItemTestExpert[menuItem]]);
    }

    handleRowClick(testExportListItem: TestExportListItem) {
        // create screening if none available
        if (testExportListItem.latestScreeningId && testExportListItem.latestScreeningId !== undefined) {
            this.router.navigate(['/screen/' + testExportListItem.id]);
        } else {
            this.screeningService.createNew(testExportListItem.id).subscribe(l => {
                this.router.navigate(['/screen/' + testExportListItem.id]);
            });
        }
    }
}
