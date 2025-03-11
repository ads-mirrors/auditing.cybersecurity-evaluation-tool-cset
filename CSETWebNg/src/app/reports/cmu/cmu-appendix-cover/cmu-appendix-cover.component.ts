import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-cmu-appendix-cover',
    templateUrl: './cmu-appendix-cover.component.html',
    styleUrls: ['../../reports.scss'],
    standalone: false
})
export class CmuAppendixCoverComponent {

  @Input() moduleName: string;
  @Input() appendixName: string;
}
