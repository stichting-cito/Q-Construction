import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MenuItemManager, DashboardData } from '../../shared/model/frontendmodel';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { UserType } from 'src/app/shared/model/model';

@Component({
    moduleId: module.id,
    styleUrls: ['../../shared/css/sb-admin-2.css'],
    templateUrl: 'dashboard.ma.page.component.html'
})

export class DashboardMAComponent implements OnInit, OnDestroy {
    dashboardData: DashboardData;
    wishlistHasItems = false;
    items: Array<MenuItemManager>;
    activeItem: MenuItemManager;
    routeSubscription: Subscription;
    constructor(private route: ActivatedRoute, private router: Router, private userservice: UserService) {
    }

    ngOnInit() {
        this.routeSubscription = this.route.data.subscribe(data => {
            this.dashboardData = data['data'];
            this.items = this.userservice.user.userType === UserType.restrictedManager ?
                [
                    { label: 'Stats', icon: 'fa fa-bar-chart', command: _ => this.selectTabValue(0) },
                    { label: 'Quality', icon: 'fa fa-check-square-o', command: _ => this.selectTabValue(1) },
                ] : [
                    { label: 'Stats', icon: 'fa fa-bar-chart', command: _ => this.selectTabValue(0) },
                    { label: 'Quality', icon: 'fa fa-check-square-o', command: _ => this.selectTabValue(1) },
                    { label: 'Users', icon: 'fa fa-users', command: _ => this.selectTabValue(2) },
                    { label: 'List', icon: 'fa fa-list', command: _ => this.selectTabValue(3) },
                    { label: 'Setup', icon: 'fa fa-cogs', command: _ => this.selectTabValue(4) }];
            const mi = +localStorage.getItem('management-active-item');
            this.activeItem = (mi && this.items.length > mi) ? this.items[mi] : this.items[0];
            if (this.userservice.user.userType === UserType.admin) {
                this.items.push({ label: 'Organisations', icon: 'fa fa-cogs', command: _ => this.selectTabValue(5) });
            }
            this.router.navigate([`/dashboard_ma/${this.activeItem.label.toLowerCase()}/`]);
        });
    }

    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
    }

    selectTabValue(value: number) {
        this.activeItem = this.items[value];
        localStorage.setItem('management-active-item', value.toString());
        this.router.navigate([`/dashboard_ma/${this.activeItem.label.toLowerCase()}/`]);
    }
}
