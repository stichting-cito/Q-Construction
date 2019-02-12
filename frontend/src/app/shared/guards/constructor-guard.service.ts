import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivate } from '@angular/router';
import { UserService } from './../services/user.service';
import { UserType } from './../model/model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class ConstructorAuthGuard implements CanActivate {

  constructor(private userService: UserService, private router: Router) { }

  canActivate(): Observable<boolean> | boolean {
    if (!this.userService.loggedIn()) {
      this.router.navigate(['/login']);
      return false;
    }
    return this.userService.getCurrentUser().pipe(map(u =>
      u.userType === UserType.constructeur));
  }
}
