import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../../shared/services/user.service';
import { DashboardData } from '../../../shared/model/frontendmodel';
import { UserType, User, KeyValue } from '../../../shared/model/model';
import { UserAdminDialogComponent } from '../../../sharedcomponents/admin/user.admin.component';
import { Subscription } from 'rxjs';

@Component({
    templateUrl: 'users.dashboard.component.html'
})

export class DashboardUsersComponent implements OnInit, OnDestroy {
    @ViewChild('constuctorModal', { static: false }) constuctorModal: UserAdminDialogComponent;
    @ViewChild('testExpertModal', { static: false }) testExpertModal: UserAdminDialogComponent;
    @ViewChild('managerModal', { static: false }) managerModal: UserAdminDialogComponent;

    dashboardData: DashboardData;
    managers = new Array<User>();
    testExperts = new Array<User>();
    constructors = new Array<User>();
    managersNotAssigned = new Array<User>();
    testExpertsNotAssigned = new Array<User>();
    constructorsNotAssigned = new Array<User>();
    routeSubscription: Subscription;

    constructor(private userService: UserService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.routeSubscription = this.route.parent.data.subscribe(data => {
            this.dashboardData = data.data;
            this.setupUsers();
        });
    }

    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
    }

    newConstructor = () => this.constuctorModal.show(false);
    newTestExpert = () => this.testExpertModal.show(false);
    newManager = () => this.managerModal.show(false);
    existingTestExpert = () => this.testExpertModal.show(true);
    existingConstructor = () => this.constuctorModal.show(true);
    existingManager = () => this.managerModal.show(true);


    setupUsers() {
        if (this.dashboardData) {
            const wishlistId = this.dashboardData.wishlist.id;
            this.managers = this.dashboardData.users.filter((u: User) => (u.userType === UserType.manager ||
                u.userType === UserType.restrictedManager) &&
                u.allowedWishlists && u.allowedWishlists.map((aw: KeyValue) => aw.id).indexOf(wishlistId) !== -1);
            this.testExperts = this.dashboardData.users.filter((u: User) => u.userType === UserType.toetsdeskundige &&
                u.allowedWishlists && u.allowedWishlists.map((aw: KeyValue) => aw.id).indexOf(wishlistId) !== -1);
            this.constructors = this.dashboardData.users.filter((u: User) => u.userType === UserType.constructeur &&
                (u.allowedWishlists && (u.allowedWishlists && u.allowedWishlists.map((aw: KeyValue) => aw.id).indexOf(wishlistId) !== -1)));

            this.managersNotAssigned = this.dashboardData.users.filter((u: User) =>
                ((u.userType === UserType.manager || u.userType === UserType.restrictedManager) &&
                    (!u.allowedWishlists || (u.allowedWishlists &&
                        u.allowedWishlists.map((aw: KeyValue) => aw.id).indexOf(wishlistId) === -1))));
            this.testExpertsNotAssigned = this.dashboardData.users.filter((u: User) =>
                (u.userType === UserType.toetsdeskundige &&
                    (!u.allowedWishlists || (u.allowedWishlists &&
                        u.allowedWishlists.map((aw: KeyValue) => aw.id).indexOf(wishlistId) === -1))));
            this.constructorsNotAssigned = this.dashboardData.users.filter((u: User) =>
                (u.userType === UserType.constructeur && (!u.allowedWishlists || u.allowedWishlists &&
                    u.allowedWishlists.map((aw: KeyValue) => aw.id).indexOf(wishlistId) === -1)));
        }
    }

    testExpertsAdded(users: User[]) {
        users.forEach(u => this.testExperts.push(u));
        users.forEach(u => {
            const userna = this.testExpertsNotAssigned.filter(un => un.id === u.id);
            if (userna && userna.length > 0) {
                this.testExpertsNotAssigned.slice(this.testExpertsNotAssigned.indexOf(userna[0]), 1);
            }
        });
    }

    managersAdded(users: User[]) {
        users.forEach(u => this.managers.push(u));
        users.forEach(u => {
            const userna = this.managersNotAssigned.filter(un => un.id === u.id);
            if (userna && userna.length > 0) {
                this.managersNotAssigned.slice(this.managersNotAssigned.indexOf(userna[0]), 1);
            }
        });
    }

    constructorsAdded(users: User[]) {
        users.forEach(u => this.constructors.push(u));
        users.forEach(u => {
            const userna = this.constructorsNotAssigned.filter(un => un.id === u.id);
            if (userna && userna.length > 0) {
                this.constructorsNotAssigned.slice(this.constructorsNotAssigned.indexOf(userna[0]), 1);
            }
        });
    }

    removePermission(user: User) {
        const awToRemove = user.allowedWishlists.filter(aw => aw.id === this.userService.user.selectedWishlist.id);
        if (awToRemove && awToRemove.length > 0) {
            user.allowedWishlists.splice(user.allowedWishlists.indexOf(awToRemove[0]), 1);
            this.userService.UpdatePermissions(user.id, user.allowedWishlists.map(aw => aw.id)).subscribe(() => {
                if (user.userType === UserType.toetsdeskundige && this.testExperts.indexOf(user) !== -1) {
                    this.testExperts.splice(this.testExperts.indexOf(user), 1);
                    this.testExpertsNotAssigned.push(user);
                } else if ((user.userType === UserType.manager || user.userType === UserType.admin)
                    && this.managers.indexOf(user) !== -1) {
                    this.managers.splice(this.managers.indexOf(user), 1);
                    this.managersNotAssigned.push(user);
                } else if (user.userType === UserType.constructeur && this.constructors.indexOf(user) !== -1) {
                    this.constructors.splice(this.constructors.indexOf(user), 1);
                    this.constructorsNotAssigned.push(user);
                }
            });
        }
    }
}
