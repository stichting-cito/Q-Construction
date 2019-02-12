import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class OrganisationService {
    private endPoint = 'Organisation';

    constructor(private http: HttpClient) { }

    get = () => this.http.get<string[]>(`${this.endPoint}`);
    delete = (name: string) => this.http.delete(`${this.endPoint}/${name}`, { responseType: 'text'});
    add = (name: string, language: string) =>
        this.http.post<string>(`${this.endPoint}`, {
            name: name,
            language: language
        }, {responseType: 'text' as 'json'})

}

