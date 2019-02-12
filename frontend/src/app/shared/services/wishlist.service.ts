import { Wishlist, ItemSummary, ItemType } from '../model/model';
import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable()
export class WishlistService {
    public endPoint = 'Wishlists';
    constructor(private http: HttpClient) { }

    all = () => this.http.get<Array<Wishlist>>(this.endPoint);
    get = (id: string) => this.http.get<Wishlist>(`${this.endPoint}/${id}`);
    getDisabledItemTypes = (id: string) => this.http.get<Array<ItemType>>(`${this.endPoint}/${id}/disabledItemTypes`);
    setScreeningList = (wishlistId: string, screeningListId: string) =>
        this.get(wishlistId).pipe(map(wi => {
            wi.screeningsListId = screeningListId;
            return this.http.put(`${this.endPoint}/${wishlistId}`, wi).subscribe();
        }))
    delete = (id: string) => this.http.delete(`${this.endPoint}/${id}`);
    addNew = (wishlist: Wishlist) => this.http.post(this.endPoint, wishlist);
    getItems = (id: string, learningObjectiveId: string) => this.http
        .get<Array<ItemSummary>>
        (`${this.endPoint}/${id}/itemsummary/bylearningobjective/${learningObjectiveId}`)

    getWordExport = (id: string, itemList: string[]) => this.http
        .post(`${this.endPoint}/${id}/WordExportByItemCodes`, itemList, { responseType: 'blob' })
        .subscribe((blob: any) => FileSaver.saveAs(blob, 'report.docx'))
}
