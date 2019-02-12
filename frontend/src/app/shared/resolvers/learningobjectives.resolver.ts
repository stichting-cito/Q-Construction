import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { LearningObjective } from '../model/model';
import { UserService } from '../../shared/services/user.service';
import { LearningObjectiveService } from '../../shared/services/learningobjective.service';

@Injectable()
export class LearningObjectivesResolver implements Resolve<Array<LearningObjective>> {
    constructor(private learningObjectiveService: LearningObjectiveService, private userService: UserService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Array<LearningObjective>> {
        return this.userService.user.selectedWishlist ?
            this.learningObjectiveService.open(this.userService.user.selectedWishlist.id) : null;
    }
}
