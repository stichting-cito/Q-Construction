import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Route } from '@angular/router';
import { EditComponent } from './edititem.page.component';
import { ConstructorAuthGuard } from './../../shared/guards/constructor-guard.service';
import { ItemResolver } from './../../shared/resolvers/item.resolver';
import { DisabledItemTypeResolver } from './../../shared/resolvers/disableditemtypes.resolver';
@NgModule({
  imports: [
    RouterModule.forChild([
  {
    path: 'edit/:id',
    component: EditComponent,
    canActivate: [ConstructorAuthGuard],
    resolve: { item: ItemResolver, disabledItemTypes: DisabledItemTypeResolver }
  }
])
  ],
  exports: [RouterModule]
})

export class EditItemRoutingModule { }
