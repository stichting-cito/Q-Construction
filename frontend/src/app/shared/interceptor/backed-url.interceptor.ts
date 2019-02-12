import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable()
export class BackendUrlInterceptor implements HttpInterceptor {
    private isIE = detectIE();
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (req.url && req.url.indexOf('assets') === -1 && req.url.indexOf('/player/') === -1) {
            req = req.clone({
                withCredentials: true,
                url: !this.isIE ? environment.api + req.url :
                `${environment.api}${req.url}?${new Date().getTime()}`
            });
        }
        return next.handle(req);
    }
}

function detectIE(): boolean {
    const ua = window.navigator.userAgent;
    return ua.indexOf('MSIE ') > 0 || ua.indexOf('Trident/') > 0 || ua.indexOf('Edge/') > 0;
}

