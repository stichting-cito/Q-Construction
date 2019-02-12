import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { DateTimeCSharpPipe } from './datetimecsharp';
import { DaysLeftPipe } from './daysleft';
import { EmptyStatePipe } from './emptystate';
import { FilterPipe } from './filter-pipe';
import { ItemStatePipe } from './itemstate';
import { ItemStatusCountPipe } from './itemstatuscount';
import { NewlinePipe } from './newline';
import { TruncatePipe } from './truncate';


@NgModule({
    imports: [TranslateModule],
    declarations: [DateTimeCSharpPipe, DaysLeftPipe, EmptyStatePipe, FilterPipe, ItemStatePipe,
        ItemStatusCountPipe, NewlinePipe, TruncatePipe],
    exports: [DateTimeCSharpPipe, DaysLeftPipe, EmptyStatePipe, FilterPipe, ItemStatePipe,
        ItemStatusCountPipe, NewlinePipe, TruncatePipe]
})
export class PipesModule {

}

