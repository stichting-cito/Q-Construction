import { Component, Input } from '@angular/core';
import { ProgressBarData } from '../../shared/model/frontendmodel';
@Component({
  selector: 'app-progress-overall',
  templateUrl: 'progress.html'
})

export class ProgressDataComponent {
  @Input() progress: Array<ProgressBarData>;
}
