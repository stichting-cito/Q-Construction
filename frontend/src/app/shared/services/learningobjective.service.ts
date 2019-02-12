import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LearningObjective } from '../model/model';

@Injectable()
export class LearningObjectiveService {
    private endPoint = 'LearningObjectives';
    private wishlistEndPoint = 'Wishlists';

    constructor(private http: HttpClient) { }

    get = (id: string) => this.http.get<LearningObjective>(`${this.endPoint}/${id}`);
    open = (wishlistId: string) => this.http.get<LearningObjective[]>(`${this.wishlistEndPoint}/${wishlistId}/openlearningobjectives`);
}

