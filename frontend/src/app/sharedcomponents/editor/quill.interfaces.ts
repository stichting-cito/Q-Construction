import { Observable } from 'rxjs';

export interface IImageHandler {
    handle(file: File): Observable<string>;
}
