import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StatsPerWishlist } from '../model/model';
@Injectable()
export class StatsService {
    private endPoint = 'StatsPerWishlist';
    constructor(private http: HttpClient) { }

    get = (id: string) => this.http.get<StatsPerWishlist>(`${this.endPoint}/${id}`);
}
