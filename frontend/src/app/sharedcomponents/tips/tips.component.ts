import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TipsType } from './../../shared/model/frontendmodel';
import { Subscription, forkJoin } from 'rxjs';

@Component({
    selector: 'app-construction-tips',
    templateUrl: 'tips.component.html',
    styles: [`
        host: {
            margin-top:15px;
        }
        .panel-heading {
            cursor: pointer;
        }
    `]
})

export class TipsComponent implements OnInit, OnDestroy {

    @Input() type: TipsType;
    public collapsed: boolean;
    public tiplist = new Array<string>();
    public header = '';
    private translationSubscription: Subscription;
    constructor(private translateService: TranslateService) { }

    ngOnInit(): void {
        this.collapsed = this.isCollapsed();
        let translationArray = [];
        switch (this.type) {
            case TipsType.do:
                translationArray = ['DO_1', 'DO_2', 'DO_3', 'DO_4', 'DO_5'];
                this.translateService.get('DOS').subscribe((v) => this.header = v);
                break;
            case TipsType.dont:
                translationArray = ['DONT_1', 'DONT_2', 'DONT_3', 'DONT_4', 'DONT_5'];
                this.translateService.get('DONTS').subscribe((v) => this.header = v);
                break;
        }
        this.translationSubscription = forkJoin
            (translationArray.map(transref => this.translateService.get(transref)))
            .subscribe(values => this.tiplist = values);
    }
    ngOnDestroy(): void {
       if (this.translationSubscription) { this.translationSubscription.unsubscribe(); }
    }
    public toggleCollapse() {
        this.collapsed = !this.collapsed;
        localStorage.setItem(`TIPS_${this.type.toString()}`, this.collapsed ? 'c' : 'e');
    }

    private isCollapsed(): boolean {
        if (localStorage.getItem(`TIPS_${this.type.toString()}`)) {
            return localStorage.getItem(`TIPS_${this.type.toString()}`) === 'c';
        }
        return false; // default expanded
    }
}




