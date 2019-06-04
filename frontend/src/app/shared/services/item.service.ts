import { Item, ItemStatus, ItemSummary, ItemStatusCount } from '../model/model';
import { Injectable } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
@Injectable()
export class ItemService {
    public endPoint = 'Items';
    private userEndPoint = 'Users';
    private summaryEndPoint = 'ItemSummaries';

    constructor(private http: HttpClient, private domSanitizer: DomSanitizer) { }

    get = (id: string) => this.http.get<Item>(`${this.endPoint}/${id}`);
    save = (item: Item) => this.http.put(`${this.endPoint}/${item.id}`, item);
    delete = (item: Item) => this.save({ ...item, itemStatus: ItemStatus.deleted });
    file = (item: Item) => this.save({ ...item, itemStatus: ItemStatus.readyForReview });

    createNew = (learningObjectiveId: string, wishlistId: string) =>
        this.http.post<Item>(this.endPoint, ({
            learningObjectiveId,
            wishListId: wishlistId,
            itemStatus: ItemStatus.draft,
            itemType: null
        }) as Item)


    getMyListByState = (state: ItemStatus, userId: string, wishlistId: string) =>
        this.http.get<ItemSummary[]>(`${this.userEndPoint}/${userId}/${wishlistId}/itemsummariesbystatus/${state}`)

    getMyListByStates = (states: Array<ItemStatus>, userId: string, wishlistId: string) => {
        const calls: Array<Observable<Array<ItemSummary>>> = states.map(s => this.getMyListByState(s, userId, wishlistId));
        return forkJoin(calls).pipe(map((data: Array<Array<ItemSummary>>) => [].concat.apply([], data)));
    }

    getMyListByLatestScreeningAndStates = (userId: string, wishlistId: string, states: ItemStatus[]): Observable<Array<ItemSummary>> =>
        this.http.get<Array<ItemSummary>>
            (`${this.userEndPoint}/${userId}/${wishlistId}/itemsummariesbylatestscreeningandstates/${states.map(o => o).join('|')}`)

    getItemSummariesToDoForTestExpert = (userId: string, wishlistId: string) =>
        this.http.get<Array<ItemSummary>>(`${this.userEndPoint}/${userId}/${wishlistId}/itemsummariestodofortestexpert`)

    getStateCount = (userId: string, wishlistId: string) =>
        this.http.get<ItemStatusCount[]>(`${this.userEndPoint}/${userId}/${wishlistId}/itemstatuscounts`)

    getAll = () => this.http.get<ItemSummary>(this.summaryEndPoint);

    getAttachmentUrl = (itemId: string, attachmentId: string, thumbnail: boolean = false) =>
        `${environment.api}/${this.endPoint}/${itemId}/attachment/${attachmentId}${thumbnail ? '/thumbnail' : ''}`

    getAttachment = (itemId: string, attachmentId: string, thumbnail: boolean = false): Observable<string> => this.http
        .get(`${this.endPoint}/${itemId}/attachment/${attachmentId}${thumbnail ? '/thumbnail' : ''}`, { responseType: 'text' })

}
