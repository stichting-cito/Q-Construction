import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LearningObjectiveComponent } from './learningobjective.page.component';
import { ConstructorAuthGuard } from './../../shared/guards/constructor-guard.service';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: 'selectLearningObjective',
                component: LearningObjectiveComponent,
                canActivate: [ConstructorAuthGuard]
            }
        ])
    ],
    exports: [RouterModule]
})
export class LearningObjectiveRoutingModule { }
