import { NgModule } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { OrganisationAdministrationComponent } from './organisation.admin.page.component';
import { OrganisationService } from 'src/app/shared/services/organisation.services';
import {DataViewModule} from 'primeng/dataview';
import { ConfirmDialogModule } from 'primeng/components/confirmdialog/confirmdialog';
@NgModule({
    imports: [SharedModule, ConfirmDialogModule, DataViewModule],
    declarations: [OrganisationAdministrationComponent],
    providers: [OrganisationService],
    exports: [OrganisationAdministrationComponent]
})

export class OrganisationAdministrationModule { }
