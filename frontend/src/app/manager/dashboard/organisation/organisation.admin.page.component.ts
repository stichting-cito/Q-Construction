import { Component } from '@angular/core';
import { OrganisationService } from 'src/app/shared/services/organisation.services';
import { Observable } from 'rxjs';
import { ConfirmationService } from 'primeng/components/common/confirmationservice';

@Component({
    styleUrls: ['organisation.admin.page.component.scss'],
    templateUrl: 'organisation.admin.page.component.html'
})

export class OrganisationAdministrationComponent {
    organisations$: Observable<string[]>;
    addedOrganisationResult = '';
    errorMessage = '';
    constructor(private organisationService: OrganisationService, private confirmationService: ConfirmationService) {
        this.organisations$ = this.organisationService.get();
    }

    delete = (organisation: string) => this.confirmationService.confirm({
        message: `Are you sure you want to delete ${organisation}?`,
        accept: () => {
            this.organisationService.delete(organisation).subscribe(_ => {
                this.organisations$ = this.organisationService.get();
            });
        }
    })

    add = (organisation: string, language: string) => {
        if (!organisation || !language) {
            this.errorMessage = '';
            if (!organisation) {
                this.errorMessage = 'Organisation name cannot be empty.';
            }
            if (!language) {
                this.errorMessage = this.errorMessage + ' Language name cannot be empty.';
            }

        } else {
            this.errorMessage = '';
            this.organisationService.add(organisation, language).subscribe(result => {
                this.addedOrganisationResult = result.replace(/(?:\r\n|\r|\n)/g, '<br>');
                this.organisations$ = this.organisationService.get();
            });
        }
    }
}
