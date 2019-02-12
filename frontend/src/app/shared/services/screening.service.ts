import { Screening, ScreeningStatus, ItemStatus, ScreeningList, ScreeningItem } from '../model/model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';


@Injectable()
export class ScreeningService {
    private endPointWishlist = 'Wishlists';
    private endPoint = 'Screenings';
    private itemEndPoint = 'Items';

    constructor(private http: HttpClient) { }

    getScreeningList = (wishlistId: string): Observable<ScreeningItem[]> => this.http
        .get<ScreeningList>(`${this.endPointWishlist}/${wishlistId}/screeninglist`)
        .pipe(map((l: ScreeningList) => (l) ? l.items : null))

    createNew = (itemId: string) => this.http
        .post<Screening>(this.endPoint, ({ itemId: itemId, status: ScreeningStatus.draft }) as Screening)

    get = (id: string) => this.http.get<Screening>(`${this.endPoint}/${id}`);

    getByItem = (id: string) =>
        this.http.get<Screening>(`${this.itemEndPoint}/${id}/screenings/latest`)

    file = (screening: Screening) => this.save({
        ...screening,
        nextItemStatus: (!screening.feedbackList || screening.feedbackList.length === 0) ?
            ItemStatus.accepted : ItemStatus.needsWork,
        status: ScreeningStatus.final
    })
    decline = (screening: Screening) => this.save({
        ...screening,
        nextItemStatus: ItemStatus.rejected,
        status: ScreeningStatus.final
    })
    save = (screening: Screening) => this.http.put(`${this.endPoint}/${screening.id}`, screening);
}
