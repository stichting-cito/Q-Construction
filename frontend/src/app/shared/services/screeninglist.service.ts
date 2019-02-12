import { ScreeningList } from '../model/model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ScreeninglistService {
    public endPoint = 'ScreeningList';

    constructor(private http: HttpClient) { }

    getAll = () => this.http.get<ScreeningList[]>(`${this.endPoint}`);
    addNew = (screeningList: ScreeningList) => this.http.post<ScreeningList>(this.endPoint, screeningList);
}
