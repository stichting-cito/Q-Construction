import { NgModule } from '@angular/core';
import { DashboardSetupComponent } from './setup.dashboard.component';
import { ConfirmationService } from 'primeng/components/common/api';
import { ConfirmDialogModule } from 'primeng/components/confirmdialog/confirmdialog';
import { FileUploadModule } from 'ng2-file-upload/file-upload/file-upload.module';
import { SharedModule } from './../../../shared/shared.module';
import { ScreeninglistService } from '../../../shared/services/screeninglist.service';

@NgModule({
    imports: [SharedModule, FileUploadModule, ConfirmDialogModule],
    declarations: [DashboardSetupComponent],
    providers: [ScreeninglistService, ConfirmationService],
    exports: [DashboardSetupComponent]
})

export class SetupModule { }
