import { Injectable, EventEmitter, Inject } from '@angular/core';
import { APP_BASE_HREF } from '@angular/common';
import { ItemStatus, User, UserType, KeyValue, Auth0Settings } from '../model/model';
import { MenuItemTestExpert } from '../model/frontendmodel';
import { Router } from '@angular/router';
import { Auth0DecodedHash } from 'auth0-js';
import { JwtHelperService } from '@auth0/angular-jwt';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import Auth0Lock from 'auth0-lock';
import { environment } from '../../../environments/environment';
// Avoid name not found warnings

@Injectable()
export class UserService {
    user: User;
    userRetrieved = new EventEmitter();
    private endPoint = 'Users';

    constructor(private http: HttpClient, private router: Router,
        @Inject(APP_BASE_HREF) private baseHref: string, private jwtHelperService: JwtHelperService) { }

    routeUser(force: boolean = false) {
        if (force) {
            window.location.href = this.baseHref;
        } else {
            if (!this.user) {
                this.router.navigate(['/login']);
            } else {
                switch (this.user.userType) {
                    case UserType.constructeur:
                        this.router.navigate(['/dashboard_co/', ItemStatus[ItemStatus.needsWork]]);
                        break;
                    case UserType.toetsdeskundige:
                        this.router.navigate(['/dashboard_te/', MenuItemTestExpert[MenuItemTestExpert.todo]]);
                        break;
                    case UserType.restrictedManager:
                    case UserType.manager:
                    case UserType.admin:
                        this.router.navigate(['/dashboard_ma/']);
                        break;
                }
            }
        }
    }

    getCurrentUser(): Observable<User> {
        if (this.user) {
            return of(this.user);
        }
        return this.http.get<User>(`${this.endPoint}/loggedinuser`).pipe(
            map(u => {
                this.user = u;
                this.userRetrieved.emit(u);
                return u;
            }), catchError(_ => of(null)));
    }

    login() {
        this.http.get<Auth0Settings>(`Settings`).subscribe(setting => {
            const options: Auth0LockConstructorOptions = {
                oidcConformant: true,
                languageDictionary: {
                    title: 'Log me in'
                },
                allowForgotPassword: false,
                autoclose: true,
                avatar: null,
                auth: {
                    responseType: 'id_token',
                    sso: false,
                    redirect: false,
                    params: { scope: 'openid' },
                },
                theme: {
                    logo: '/assets/CitolabLogo256.png',
                    primaryColor: '#00B6DE'
                }
            };
            const lock = new Auth0Lock(setting.auth0ClientId, setting.auth0TenantUrl, options);
            lock.on('authenticated', (authResult: Auth0DecodedHash) => {
                localStorage.setItem((environment.production + '_id_token_qconst'), authResult.idToken);
                this.getCurrentUser().subscribe(() =>
                    this.routeUser());
            });
            // Call the show method to display the widget.
            lock.show();
        });
    }

    UpdateSelectedWishlist(wishlist: KeyValue): Observable<any> {
        this.user.selectedWishlist = wishlist;
        return this.http
            .post(`${this.endPoint}/${this.user.id}/${wishlist.id}/updateselectedwishlist`, {});
    }

    UpdatePermissions = (userId: string, wishlistIds: string[]) =>
        this.http.put(`${this.endPoint}/${userId}/updatepermissions`, wishlistIds)

    AddNew = (user: User): Observable<User> => this.http.post<User>(this.endPoint, user);
    All = () => this.http.get<User[]>(`${this.endPoint}`);
    loggedIn() {
        try {
            return !this.jwtHelperService.isTokenExpired();
        } catch (error) {
            return false;
        }
    }
    logout() {
        // Remove token and profile from localStorage
        localStorage.removeItem((environment.production + '_id_token_qconst'));
        sessionStorage.removeItem('User');
        this.user = null;
        this.router.navigate(['/login']);
    }
}
