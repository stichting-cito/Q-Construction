import { Component, OnInit, TemplateRef, OnDestroy } from '@angular/core';
import { UserService } from '../../shared/services/user.service';
import { WishlistService } from '../../shared/services/wishlist.service';
import { UserType, User, Wishlist, KeyValue } from '../../shared/model/model';
import { TranslateService } from '@ngx-translate/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { Subscription } from 'rxjs';

@Component({
    moduleId: module.id,
    selector: 'app-nav-bar',
    templateUrl: 'navbar.component.html'
})
export class NavbarComponent implements OnInit, OnDestroy {
    newWishlist = new Wishlist();
    userMenuOpen = false;
    username = '';
    isManager = false;
    wishlists: Wishlist[];
    selectedWishlist: Wishlist;
    modalRef: BsModalRef;
    config = {
        backdrop: true,
        ignoreBackdropClick: false
    };
    userRetrievedSubscription: Subscription;
    constructor(public userService: UserService, private wishlistService: WishlistService,
        private translateService: TranslateService, private modalService: BsModalService) { }

    ngOnInit() {
        this.username = this.getUsername();
        this.userRetrievedSubscription = this.userService.userRetrieved.subscribe((u: User) => {
            this.isManager = (u.userType === UserType.manager || u.userType === UserType.admin);
            this.username = this.getUsername();
        });
    }

    ngOnDestroy(): void {
        if (this.userRetrievedSubscription) {
            this.userRetrievedSubscription.unsubscribe();
        }
    }

    addWishlist = (template: TemplateRef<any>) =>
        this.modalRef = this.modalService.show(template, this.config)

    changeWishlist(wishlist: KeyValue) {
        this.userService.UpdateSelectedWishlist(wishlist).subscribe(() => this.userService.routeUser(true));
    }

    get routeDashboard() {
        return (this.userService.user && this.userService.user.userType === UserType.constructeur) ?
            '/dashboard_co/draft' : this.userService.user && this.userService.user.userType === UserType.toetsdeskundige ?
                '/dashboard_co/draft' : '/';
    }

    getUsername() {
        return this.userService.user ? this.userService.user.name : '';
    }

    toggleLanguage() {
        let selectedLanguage = 'en';
        if (this.translateService.currentLang === 'en') {
            selectedLanguage = 'nl';
        }
        localStorage.setItem('selectedLanguage', selectedLanguage);
        this.translateService.use(selectedLanguage);
    }

    logout() {
        this.userMenuOpen = false;
        this.userService.logout();
        this.isManager = false;
        this.username = '';
    }

    onSubmit() {
        if (this.newWishlist.title) {
            this.wishlistService.addNew(this.newWishlist).subscribe((w: Wishlist) => {
                this.changeWishlist({ id: w.id, value: w.title });
                this.modalRef.hide();
            });
        }
    }
}
