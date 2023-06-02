import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading-panel',
  templateUrl: './loading-panel.component.html',
  styleUrls: ['./loading-panel.component.scss']
})
export class LoadingPanelComponent {
  @Input() hasAbsolutePosition: boolean = false;

  get viewClass(): string {
    return this.hasAbsolutePosition ? 'absolute-container' : 'loading-container';
  }
}
