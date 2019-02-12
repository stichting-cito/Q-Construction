import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

import { Item } from '../model/model';
import { ItemService } from '../../shared/services/item.service';

@Injectable()
export class ItemResolver implements Resolve<Item> {
    constructor(private itemService: ItemService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Item> {
        return this.itemService.get(route.params['id']);
    }
}
