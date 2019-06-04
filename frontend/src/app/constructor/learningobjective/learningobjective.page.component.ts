import { Component, OnInit, OnDestroy } from '@angular/core';
import { LearningObjectiveService } from '../../shared/services/learningobjective.service';
import { LearningObjective } from '../../shared/model/model';
import { CollapsableHeader } from '../../shared/model/frontendmodel';
import { Router, ActivatedRoute } from '@angular/router';
import { ItemService } from '../../shared/services/item.service';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-learningobjective',
    templateUrl: 'learningobjective.page.component.html'
})
export class LearningObjectiveComponent implements OnInit, OnDestroy {
    learningObjectiveList = new Array<LearningObjective>();
    domainHeaders = new Array<CollapsableHeader>();
    dashboardstate: string;
    routeSubscription: Subscription;
    parentRouteSubscription: Subscription;

    constructor(private router: Router, private itemService: ItemService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.routeSubscription = this.route.data.subscribe(data => {
            this.learningObjectiveList = data.learningobjectives;
            this.fillDomains();
        });
        this.parentRouteSubscription = this.route.parent.params.subscribe(params => {
            this.dashboardstate = params.state;
        });
    }
    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
        if (this.parentRouteSubscription) {
            this.parentRouteSubscription.unsubscribe();
        }
    }

    fillDomains() {
        const dlist = this.learningObjectiveList.map(lo => lo.domainTitle);
        const uniqueDomains = dlist
            .filter((v, i) => dlist.indexOf(v) === i);
        this.domainHeaders = uniqueDomains
            .map(v => {
                return { name: v, collapsed: this.determineCollapseValue(v, this.learningObjectiveList.length, uniqueDomains.length) };
            });
    }

    toggleCollapse(domainHeader: CollapsableHeader) {
        domainHeader.collapsed = !domainHeader.collapsed;
        localStorage.setItem(`C_DOMAIN_${domainHeader.name}`, domainHeader.collapsed ? 'c' : 'e');
    }

    determineCollapseValue(domainName: string, listCount: number, domainCount: number): boolean {
        if (localStorage.getItem(`C_DOMAIN_${domainName}`)) {
            return localStorage.getItem(`C_DOMAIN_${domainName}`) === 'c';
        }
        return domainCount > 1 && listCount > 10;
    }

    getDate(dateTimeCSharp: Date): Date {
        if (dateTimeCSharp) {
            return new Date(Date.parse(dateTimeCSharp.toString()));
        }
        return null;
    }

    createItem(learningobjectiveId: string, wishlistId: string) {
        // Als het lukt om een item te creeren van dit leerdoel, wacht dan op het id.
        // Zoja, dan route naar de create pagina.
        const that = this;
        this.itemService.createNew(learningobjectiveId, wishlistId).subscribe(item => {
            that.router.navigate(['/dashboard_co/draft/concept/' + item.id]);
        });
    }
}
