import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { ItemWrapperComponent } from '../../sharedcomponents/item/item.component';
import { ScreeningComponent } from '../../sharedcomponents/screening/screening.component';
import { Item, Screening } from '../../shared/model/model';
import { Subject, BehaviorSubject, Subscription } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { MenuItemTestExpert } from '../../shared/model/frontendmodel';

@Component({
    templateUrl: 'reviewitem.page.component.html',
})
export class ReviewComponent implements OnInit, OnDestroy {

    @ViewChild(ItemWrapperComponent, { static: false }) itemComponent: ItemWrapperComponent;
    @ViewChild(ScreeningComponent, { static: false }) screeningsComponent: ScreeningComponent;
    public item: Item = new Item();
    public screening: Screening;
    public fileButtonText: Subject<string> = new BehaviorSubject('');
    public deletes = false;

    private routesubscription: Subscription;
    private fileSubscription: Subscription;
    private declineSubscription: Subscription;
    private translateSubscription: Subscription;
    constructor(
        private route: ActivatedRoute, private router: Router,
        public location: Location, private translateService: TranslateService) { }

    ngOnInit() {
        this.translateService.get('BUTTON_ACCEPT').subscribe(t => this.fileButtonText.next(t));
        this.routesubscription = this.route.data.subscribe(data => {
            this.item = data.item;
            this.screening = data.screening;
        });
    }

    ngOnDestroy(): void {
        if (this.routesubscription) { this.routesubscription.unsubscribe(); }
        if (this.fileSubscription) { this.fileSubscription.unsubscribe(); }
        if (this.declineSubscription) { this.declineSubscription.unsubscribe(); }
        if (this.translateSubscription) { this.translateSubscription.unsubscribe(); }
    }

    file() {
        this.fileSubscription = this.screeningsComponent.file().subscribe(() =>
            this.router.navigate(['/dashboard_te', MenuItemTestExpert[MenuItemTestExpert.todo]]));
    }

    decline() {
        this.declineSubscription = this.screeningsComponent.decline().subscribe(() =>
            this.router.navigate(['/dashboard_te', MenuItemTestExpert[MenuItemTestExpert.todo]]));
    }

    feedbackChanged() {
        if (this.screeningsComponent && this.screeningsComponent.hasFeedback) {
            this.translateSubscription = this.translateService.get('BUTTON_TE_SUBMIT').subscribe(t => setTimeout(() => {
                this.fileButtonText.next(t);
            }, 0));
        } else {
            this.translateSubscription = this.translateService.get('BUTTON_ACCEPT').subscribe(t => setTimeout(() => {
                this.fileButtonText.next(t);
            }, 0));
        }
    }
}




