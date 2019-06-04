import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { UserService } from './../services/user.service';
import { of, Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable()
export class LoginAuthGuard implements CanActivate {
    constructor(private userService: UserService) { }

    canActivate(): Observable<boolean> {
        return this.userService.loggedIn() ?
            this.userService.getCurrentUser()
                .pipe(
                    catchError(_ => of(true)),
                    map(u => {
                        if (u) {
                            this.userService.routeUser();
                        }
                        return true;
                    })) : of(true);
    }
}
