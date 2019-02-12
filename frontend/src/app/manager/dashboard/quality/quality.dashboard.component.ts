import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { UserType, ScreeningItemStats } from '../../../shared/model/model';
import { DashboardData } from '../../../shared/model/frontendmodel';
import { Subscription } from 'rxjs';

@Component({
    moduleId: module.id,
    templateUrl: 'quality.dashboard.component.html'
})

export class DashboardQualityComponent implements OnInit, OnDestroy {
    dashboardData: DashboardData;
    wishlistHasItems = false;
    testExpertFeedbackData: any;
    selectedConstructorForTopFeedback: any;
    removedData: any;
    removedDataOptions: any = { legend: { display: false } };
    roundsPerDomain = new Array<any>();
    topFeedback = new Array<any>();
    constructorsFeedback = new Array<any>();
    AllId = 'ae4bc035-317e-4a30-bb36-64462cc8cb79';
    routeSubscription: Subscription;

    constructor(private translateService: TranslateService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.routeSubscription = this.route.parent.data.subscribe(data => {
            this.dashboardData = data['data'];
            if (this.dashboardData) {
                this.wishlistHasItems = this.dashboardData &&
                    this.dashboardData.wishlist.wishListItems.filter(w => w.numberOfItems !== w.todo).length > 0;
                if (this.wishlistHasItems) {
                    this.setRemovedDataOptions();
                    this.getRemovedItemData();
                    this.getRoundsPerDomain();
                    this.getFeedbackDataConstuctor();
                    this.getFeedbackDataTestExpert();
                }
            }
        });
    }

    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
    }

    setRemovedDataOptions() {
        this.removedDataOptions = {
            legend: { display: false }, responsive: false, elements: {
                center: {
                    maxText: '100%', text: String.fromCharCode(0xf1f8), fontFamily: 'FontAwesome', fontStyle: 'normal',
                    fontColor: '#000', minFontSize: 1, maxFontSize: 256,
                }
            }
        };
    }

    getRoundsPerDomain() {
        this.roundsPerDomain = this.dashboardData.statsPerDomain.map(d => {
            return { name: d.domainName, count: d.meanReviewIterations };
        });
    }

    getFeedbackDataConstuctor() {
        this.constructorsFeedback =
            [{
                label: 'Alle', value:
                {
                    image: 'http://freeflaticons.com/wp-content/uploads/2014/11/crowd-copy-1415616695ngk48.png',
                    id: this.AllId
                }
            }]
                .concat(this.dashboardData.statsPerUser.filter(u => u.userType === UserType.constructeur).map(u => {
                    return { label: u.userName, value: { image: u.picture, id: u.userId } };
                }));
        this.selectedConstructorForTopFeedback = this.constructorsFeedback[0];
        this.topFeedback = this.getTopFiveFeedback(this.getTotalListOfFeedback(UserType.constructeur));
    }

    getFeedbackDataTestExpert() {
        const total = this.getTotalListOfFeedback(UserType.toetsdeskundige);
        const feedbacks = this.dashboardData.screeningsList.map(si => {
            return (si.value !== '') ?
                { name: si.value, label: si.code } :
                { name: si.value, label: '..' };
        });

        const newDataset = new Array<any>();
        newDataset.push({
            label: 'all', values: feedbacks.map(f => {
                const tItem = total.filter(t => t.screeningItemName === f.name);
                return (tItem && tItem.length > 0) ? tItem[0].useCount : 0;
            })
        });
        this.dashboardData.statsPerUser.filter(u => u.userType === UserType.toetsdeskundige).forEach(
            u => {
                newDataset.push({
                    label: u.userName, values: feedbacks.map(f => {
                        if (u.screeningItemStatsList) {
                            const si = u.screeningItemStatsList.filter(s => s.screeningItemName === f.name);
                            return (si && si.length === 1) ? si[0].useCount : 0;
                        } else { return 0; }
                    })
                });
            }
        );
        this.testExpertFeedbackData = {
            labels: feedbacks.map(f => f.label),
            datasets: newDataset.map((d, index) => {
                return {
                    label: d.label, data: d.values,
                    fill: false, borderColor: this.getRandomColor(index), hidden: !(d.label === 'all')
                };
            })
        };
    }

    getTotalListOfFeedback(type: UserType) {
        const screeningsItemStats = new Array<ScreeningItemStats>();
        this.dashboardData.statsPerUser.filter(us => us.userType === type)
            .forEach((cs, index) => {
                cs.screeningItemStatsList.forEach(sic => {
                    const f = screeningsItemStats.filter(s => s.screeningItemId === sic.screeningItemId);
                    if (f && f.length > 0) {
                        f[0].useCount += sic.useCount;
                    } else {
                        screeningsItemStats.push(JSON.parse(JSON.stringify(sic)));
                    }
                });
            });
        return screeningsItemStats;
    }

    getTopFiveFeedback(stats: Array<ScreeningItemStats>) {
        const siIds = stats.map(si => si.screeningItemId).filter((item, i, ar) => ar.indexOf(item) === i);
        const top = siIds.map(s => {
            const items = stats.filter(si => si.screeningItemId === s);
            const label = items[0].screeningItemName;
            return (items && items.length > 0) ? {
                name: label, count: items.map(i => i.useCount)
                    .reduce((a, b) => a + b, 0)
            } : null;
        }).sort((a, b) => b.count - a.count);
        top.splice(5);
        return top;
    }
    onConstructorChange(newValue: any) {
        if (newValue.id === this.AllId) {
            this.topFeedback = this.getTopFiveFeedback(
                this.getTotalListOfFeedback(UserType.constructeur));
        } else {
            const cdata = this.dashboardData.statsPerUser.filter(us => us.userType === UserType.constructeur &&
                us.userId === newValue.id);
            if (cdata && cdata.length === 1) {
                this.topFeedback = this.getTopFiveFeedback(cdata[0].screeningItemStatsList);
            }
        }
    }

    getRandomColor(index: number) {
        const definedColors = ['#737373', '#F15A60', '#7AC36A', '#5A9BD4', '#FAA75B', '#9E67AB', '#CE7058', '#D77FB4'];
        if (definedColors.length > index) {
            return (definedColors[index]);
        } else {
            const letters = '0123456789ABCDEF';
            let color = '#';
            for (let i = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            return color;
        }
    }

    getRemovedItemData() {
        const l1 = this.translateService.instant('MD_DELETED_NO_ROUNDS');
        const l2 = this.translateService.instant('MD_DELETED_ONE_ROUND');
        const l3 = this.translateService.instant('MD_DELETED_MORE_ROUNDS_TE');
        const l4 = this.translateService.instant('MD_DELETED_MORE_ROUNDS_TD');
        const direct = this.dashboardData.screeningRoundsOfWastedItems.filter(i => i.rounds === 0).length;
        const one = this.dashboardData.screeningRoundsOfWastedItems.filter(i => i.rounds === 1).length;
        const moreThanOneC = this.dashboardData.screeningRoundsOfWastedItems.filter(i => i.rounds > 1 && i.removedByAuthor === true).length;
        const moreThanOneT =
            this.dashboardData.screeningRoundsOfWastedItems.filter(i => i.rounds > 1 && i.removedByAuthor === false).length;
        const factor = (direct + one + moreThanOneC + moreThanOneT) / 100;
        const d1 = factor !== 0 ? Math.round(direct / factor) : 0;
        const d2 = factor !== 0 ? Math.round(one / factor) : 0;
        const d3 = factor !== 0 ? Math.round(moreThanOneC / factor) : 0;
        const d4 = factor !== 0 ? 100 - (d1 + d2 + d3) : 0;
        this.removedData = {
            labels: [l1, l2, l3, l4],
            datasets: [{ data: [d1, d2, d3, d4], backgroundColor: ['#d24d57', '#c91737', '#9d2933', '#8f1d21'] }]
        };
    }
}
