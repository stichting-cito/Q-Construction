import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-stats-tile',
  styleUrls: ['../../shared/css/sb-admin-2.css'],
  templateUrl: 'stats.html'
})

export class StatsComponent  {
  @Input() number: string;
  @Input() comments: string;
  @Input() colour: string;
  @Input() type: string;
  @Input() detailUrl: string;
}
