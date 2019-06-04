import { Component, Input, OnInit, AfterViewChecked, Output, EventEmitter, Sanitizer, SecurityContext, OnDestroy } from '@angular/core';
import { ScreeningItem, Screening, Feedback, Item, ItemStatus } from '../../shared/model/model';
import { UserService } from '../../shared/services/user.service';
import { ScreeningService } from '../../shared/services/screening.service';
import { TranslateService } from '@ngx-translate/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
    selector: 'app-item-screening',
    styleUrls: ['screening.component.css'],
    templateUrl: 'screening.component.html'
})
export class ScreeningComponent implements OnInit, AfterViewChecked, OnDestroy {

    @Input() readonly = false;
    @Input() item: Item;
    @Input() screening: Screening;
    @Output() feedbackchanged = new EventEmitter();

    public screeningList: Array<ScreeningItem>;
    public screeningListWithFeedback: Array<ScreeningItem>;
    public screeningCategories: string[];
    public selectedscreening: string;
    public hasFeedback: boolean;
    public saveTime: string; // date time in seconds
    public selectedImage: '';
    screenItemForm: FormGroup;
    private formSubscription: Subscription;
    private listSubscription: Subscription;
    private translationSubscription: Subscription;
    constructor(private screeningService: ScreeningService, private formBuilder: FormBuilder,
        // tslint:disable-next-line:align
        private userService: UserService, private translateService: TranslateService) {

    }

    ngOnInit() {
        this.screenItemForm = this.formBuilder.group({
            feedback: ['']  // because we bind using ng-model and dont give specific validation errors this works.
            // it should be better to find a way to bind to an array/list
        });
        this.screenItemForm.valueChanges.subscribe(() => {
            this.translateService.get('SAVING').subscribe(t => {
                this.saveTime = t;
            });
        });

        this.formSubscription = this.screenItemForm.valueChanges
            .pipe(debounceTime(500))
            .subscribe(data => {
                this.save();
                this.updateSaveTime();
            });



        // Haal de screeningslijst op
        this.listSubscription = this.screeningService.getScreeningList(this.userService.user.selectedWishlist.id).subscribe(list => {
            this.screeningList = list;
            const categories = this.screeningList.map(m => m.category);

            // Filter the list only for screeningsitems with feedback
            // This is for the readonly mode
            this.screeningListWithFeedback = this.screeningList.filter((val, i) => {
                return (this.getFeedback(val.id).trim() !== '');
            });
            // Filter the screeningList for categories
            this.screeningCategories = categories.filter((v, i) => {
                return categories.indexOf(v) === i && ((!this.screening ||
                    this.item.itemStatus === ItemStatus.inReview || this.item.itemStatus === ItemStatus.readyForReview) ||
                    this.categoryContainsFeedback(v) > 0);
            });
        });
    }

    ngOnDestroy(): void {
        if (this.listSubscription) { this.listSubscription.unsubscribe(); }
        if (this.formSubscription) { this.formSubscription.unsubscribe(); }
        if (this.translationSubscription) { this.translationSubscription.unsubscribe(); }
    }

    categoryContainsFeedback = (category: string) =>
        this.screeningListWithFeedback.filter(screeningsList => screeningsList.category === category).length

    ngAfterViewChecked() {
        this.checkIfHasFeedback();
    }

    checkIfHasFeedback() {
        this.hasFeedback = false;
        if (this.screening && this.screening.feedbackList) {
            this.hasFeedback = this.screening.feedbackList.length > 0;
            this.feedbackchanged.emit({
                value: this.hasFeedback
            });
        }
    }

    // Checks if there is a feedback item for this screeningsitem
    getFeedback(screeningItemId: string): string {
        if (this.screening && this.screening.feedbackList) {
            const feedback = this.screening.feedbackList.find(fb => fb.screeningItemId === screeningItemId);
            if (feedback) {
                return feedback.value;
            }
        }
        return '';
    }

    setFeedback(screeningItemId: string, value: string) {
        // Als er een screening ( wat we mogen aannemen, hoe kom je anders op dit scherm )
        if (this.screening) {
            // Is er een lijst met feedback, zo nee maak er een aan.
            if (!this.screening.feedbackList) {
                this.screening.feedbackList = new Array<Feedback>();
            }
            // Zoek de feedback bij dit screenings item
            const feedback = this.screening.feedbackList.find(fb => fb.screeningItemId === screeningItemId);
            if (feedback) {
                // Als er feedback is, vul dit in.
                feedback.value = value;
            } else {
                // Als er nog geen feedback is
                const newFeedback = new Feedback();
                newFeedback.screeningItemId = screeningItemId;
                newFeedback.value = value;
                this.screening.feedbackList.push(newFeedback);
            }
            if (value.trim().length === 0) {
                // Als er geen feedback meer is .. length = 0 dan haal m uit de lijst
                this.screening.feedbackList.splice(this.screening.feedbackList.indexOf(feedback), 1);
            }
        }
    }

    selectScreeningsItem(s: any) {
        this.selectedscreening = s;
    }

    updateSaveTime = () => {
        this.translationSubscription = this.translateService.get('SCREENING_IS_SAVED').subscribe(text => {
            this.saveTime = text;
        });
    }

    save = () => this.screeningService.save(this.screening).subscribe();
    file = () => this.screeningService.file(this.screening);
    decline = () => this.screeningService.decline(this.screening);
}
