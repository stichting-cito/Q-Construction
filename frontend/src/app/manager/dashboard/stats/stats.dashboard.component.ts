import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DateCount, UserType, UserStats, User } from '../../../shared/model/model';
import { DashboardData, ProgressBarData, ProgressGraphData } from '../../../shared/model/frontendmodel';
import { Subscription } from 'rxjs';
// import moment from 'moment/moment';
// import 'moment/locale/en-gb';
// import 'moment/locale/nl';

@Component({
    templateUrl: 'stats.dashboard.component.html'
})

export class DashboardStatsComponent implements OnInit, OnDestroy {
    dashboardData: DashboardData;
    overdeadline: number;
    oneday: number;
    sevendays: number;
    oversevendays: number;
    percentagePerDomainOptions: any = { legend: { display: false }, responsive: true };
    progressDataOptions: any = { responsive: false };
    progressData: any;
    displayTestExpertDialog = false;
    displayConstructorDialog = false;
    progressOverall: Array<ProgressBarData>;
    wishlistHasItems = false;
    percentagePerDomain: any;
    cs = new Array<UserStats>();
    ts = new Array<UserStats>();
    constructorPairs = new Array<Array<UserStats>>();
    testexpertPairs = new Array<Array<UserStats>>();
    private routeSubscription: Subscription;
    constructor(private translateService: TranslateService, private route: ActivatedRoute) {
    }

    ngOnInit() {
        this.routeSubscription = this.route.parent.data.subscribe(data => {
            this.dashboardData = data.data;
            if (this.dashboardData) {
                this.wishlistHasItems = this.dashboardData.wishlist.wishListItems.filter(w => w.numberOfItems !== w.todo).length > 0;
                this.getProgressData();
                this.getDeadlineDates();
                this.getProgressDataForGraph();
                this.getDomainData();
                this.getUsers();
            }
        });
    }
    ngOnDestroy(): void {
        if (this.routeSubscription) {
            this.routeSubscription.unsubscribe();
        }
    }
    getProgressData() {
        const tc = this.dashboardData.itemTargetCount; const ac = this.dashboardData.itemsAcceptedCount;
        const rc = this.dashboardData.itemsInReviewCount; const tdc = this.dashboardData.itemsTodoCount;
        const tAcc = `${ac} ${this.translateService.instant('MD_ACCEPTED_PROGRESS')}`;
        const tIr = `${rc} ${this.translateService.instant('MD_INREVIEW_PROGRESS')}`;
        const tTd = `${tdc} ${this.translateService.instant('MD_TODO_PROGRESS')}`;
        const pAcc = this.getPercentage(ac, tc);
        const pReview = this.getPercentage(rc, tc);
        const pTodo = 100 - (pAcc + pReview);
        this.progressOverall = [
            { label: tAcc, percentage: pAcc, value: pAcc, type: 'info' },
            { label: tIr, percentage: pReview, value: pReview, type: 'warning' },
            { label: tTd, percentage: pTodo, value: pTodo, type: 'default' }];
    }

    getDeadlineDates() {
        const today = this.getToday();
        const tomorrow = new Date(this.getToday().setDate(this.getToday().getDate() + 1));
        const sevendaysdate = new Date(this.getToday().setDate(this.getToday().getDate() + 7));
        this.overdeadline = this.dashboardData.itemDeadlinesWithCounts
            .filter(ic => new Date(ic.date.toString()).getTime() < new Date(today.toString()).getTime())
            .reduce((a, b) => a + b.count, 0);
        this.oneday = this.dashboardData.itemDeadlinesWithCounts
            .filter(ic => this.isInRange(ic.date, today, tomorrow))
            .reduce((a, b) => a + b.count, 0);
        this.sevendays = (this.dashboardData.itemDeadlinesWithCounts
            .filter(ic => this.isInRange(ic.date, today, sevendaysdate))
            .reduce((a, b) => a + b.count, 0) - this.oneday);
        this.oversevendays = (this.dashboardData.itemDeadlinesWithCounts
            .reduce((a, b) => a + b.count, 0) - (this.overdeadline + this.oneday + this.sevendays));
    }

    getProgressDataForGraph() {
        // TODO: could be moved to the backend
        if (this.dashboardData.itemDeadlinesWithCounts.length > 0) {
            const endDate = this.dashboardData.itemDeadlinesWithCounts.map(
                i => i.date).reduce((d1: Date, d2: Date) => d1 > d2 ? d1 : d2);
            const weeks = this.getWeeks(this.getFirstDayWeek(this.dashboardData.wishlist.created), endDate);
            const labels = weeks.map(w => this.formatDateForLabel(w.date));
            // add one label to have the last label
            if (labels && labels.length > 0) {
                labels.push(this.formatDateForLabel(this.addDays(weeks[weeks.length - 1].date, 7)));
            }
            let totalActual = 0;
            // sum the actuals over the weeks. if there is one created in wk2 its still there in all next weeks
            weeks.forEach(week => {
                totalActual += week.itemCountActual;
                week.itemCountActual = totalActual;
            });
            // divide the items this should be created equally over the weeks to go
            for (let index = weeks.length - 1; index >= 0; index--) {
                const totalExpected = weeks[index].itemCountDeadlineEnd;
                if (totalExpected > 0) {
                    for (let wIndex = 0; wIndex < weeks.length; wIndex++) {
                        const week = weeks[wIndex];
                        week.itemCountDeadlineExpected += (wIndex < index) ?
                            Math.round((totalExpected / (index + 1)) * (wIndex + 1)) : totalExpected;
                    }
                }
            }
            const gd = weeks.filter((w) => this.isInRange(this.getToday(), w.date, this.addDays(w.date, 7)));
            const currentweek = (gd && gd.length > 0) ? weeks.indexOf(gd[0]) : weeks.length;
            const progressActual = weeks.map((w) => w.itemCountActual);
            progressActual.splice(currentweek + 1); // remove the data after today.
            this.progressData = {
                labels,
                datasets: [{
                    label: 'Ideal', data: [0].concat(weeks.map((w) => w.itemCountDeadlineExpected)), // add zero to start with
                    fill: true, borderColor: '#aab2bd'
                },
                { label: 'Actual', data: [0].concat(progressActual), fill: true, borderColor: '#48cfad' }
                ]
            };
        }
    }

    getDomainData() {
        this.percentagePerDomain = this.dashboardData.statsPerDomain.map(ds => {
            return {
                labels: [ds.domainName, 'Todo'], datasets: [{
                    data: [ds.percentageAccepted, 100 - ds.percentageAccepted],
                    backgroundColor: ['#37bc9b', 'lightgrey'], center: this.getTextOptions(` ${ds.percentageAccepted}%`)
                }]
            };
        });
    }

    getUsers() {
        this.cs = this.dashboardData.statsPerUser.filter(u => u.userType === UserType.constructeur);
        this.ts = this.dashboardData.statsPerUser.filter(u => u.userType === UserType.toetsdeskundige);
        this.constructorPairs = this.getPairs(this.cs);
        this.testexpertPairs = this.getPairs(this.ts);
    }

    getWeeks(firstDay: Date, endDate: Date): Array<ProgressGraphData> {
        const weeks = new Array<ProgressGraphData>();
        let dayinweek = firstDay;
        // get actual and wished starting from the start of week where the wishlist is created to
        // the last deadline.
        while (new Date(dayinweek.toString()).getTime() < new Date(endDate.toString()).getTime()) {
            const endWeek = this.addDays(dayinweek, 7);
            const actualCount = this.getTotalCountForDateRange(this.dashboardData.itemsAcceptedPerDayCumulative, dayinweek, endWeek);
            const deadlineCount = this.getTotalCountForDateRange(this.dashboardData.deadlines, dayinweek, endWeek);
            weeks.push({
                date: dayinweek, itemCountDeadlineEnd: deadlineCount,
                itemCountDeadlineExpected: 0, itemCountActual: actualCount
            });
            dayinweek = endWeek;
        }
        return weeks;
    }

    getPairs(list: Array<any>): Array<Array<any>> {
        const pairs = new Array<Array<any>>();
        for (let index = 0; index < (list.length / 2); index++) {
            const secondUser = !(list.length === ((index * 2) + 1)) ? list[((index * 2) + 1)] : null;
            pairs.push([list[(index * 2)], secondUser]);
        }
        return pairs;
    }

    getToday(): Date {
        const today = new Date();
        let dd = today.getDate();
        let mm = today.getMonth() + 1; // January is 0!
        const yyyy = today.getFullYear();
        if (dd < 10) { dd = +('0' + dd); }
        if (mm < 10) { mm = +('0' + mm); }
        return new Date(mm + '/' + dd + '/' + yyyy);
    }

    getFirstDayWeek(d: Date) {
        d = new Date(d);
        // tslint:disable-next-line:one-variable-per-declaration
        const day = d.getDay(),
            diff = d.getDate() - day + (day === 0 ? -6 : 1); // adjust when day is sunday
        return new Date(d.setDate(diff));
    }

    getPercentage(value: number, total: number) {
        return (total === 0) ? 0 : Math.round((value / total) * 100);
    }

    formatDateForLabel(input: Date) {
        const d = input.getDate(); const m = input.getMonth() + 1;
        return [(d < 10) ? '0' + d : d.toString(), (m < 10) ? '0' + m : m.toString()].join('/');
    }

    addDays(date: Date, days: number): Date {
        return new Date(new Date().setTime(date.getTime() + days * 86400000));
    }

    isInRange(dateToCheck: Date, beginDate: Date, endDate: Date): boolean {
        return (dateToCheck && beginDate && endDate) &&
            new Date(dateToCheck.toString()).getTime() >= new Date(beginDate.toString()).getTime()
            && new Date(dateToCheck.toString()).getTime() < new Date(endDate.toString()).getTime();
    }

    getTotalCountForDateRange(deadlines: Array<DateCount>, beginDate: Date, endDate: Date): number {
        let count = 0;
        const counts = deadlines.
            filter(d => this.isInRange(d.date, beginDate, endDate)).map(d => d.count);
        if (counts && counts.length > 0) {
            count = counts.reduce((a, b) => a + b);
        }
        return count;
    }

    getTextOptions(text: string) {
        return {
            // the longest text this could appear in the center
            maxText: '100%',
            text,
            fontFamily: 'Arial',
            fontStyle: 'normal',
            fontColor: '#000',
            minFontSize: 1,
            maxFontSize: 256,
        };
    }
}
