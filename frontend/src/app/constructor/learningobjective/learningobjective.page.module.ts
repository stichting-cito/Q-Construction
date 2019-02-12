import { NgModule } from '@angular/core';
import { LearningObjectiveComponent } from './learningobjective.page.component';
import { LearningObjectiveRoutingModule } from './learningobjective.page.routing.module';
import { SharedModule } from './../../shared/shared.module';
import { LearningObjectivesResolver } from './../../shared/resolvers/learningobjectives.resolver';
@NgModule({
    imports: [SharedModule, LearningObjectiveRoutingModule],
    declarations: [LearningObjectiveComponent],
    providers: [LearningObjectivesResolver],
    exports: [LearningObjectiveComponent]
})
export class LearningObjectiveModule { }
