import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

import { Screening } from '../model/model';
import { ScreeningService } from '../../shared/services/screening.service';

@Injectable()
export class ScreeningResolver implements Resolve<Screening> {
    constructor(private screeningService: ScreeningService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Screening> {
        return this.screeningService.getByItem(route.params['id']);
    }
}
