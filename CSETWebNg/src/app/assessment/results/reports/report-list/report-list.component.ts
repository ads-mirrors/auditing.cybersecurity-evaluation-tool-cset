import { Component, Input, OnInit } from '@angular/core';
import { ReportService } from '../../../../services/report.service';
import { TranslocoService } from '@jsverse/transloco';
import { AssessmentService } from '../../../../services/assessment.service';
import { ObservationsService } from '../../../../services/observations.service';
import { MaturityService } from '../../../../services/maturity.service';

/**
 * This component displays a list of report launching links.  
 * The links defined for each assessment type is defined in report-list.json
 * Two types of links are supported.  Most launch an HTML report in a new tab, 
 * defined with a 'linkUrl' property.
 * The other will launch an exported Excel spreadsheet, defined with an 'exportUrl' 
 * property.
 */
@Component({
    selector: 'app-report-list',
    templateUrl: './report-list.component.html',
    styleUrls: ['./report-list.component.scss'],
    standalone: false
})
export class ReportListComponent implements OnInit {


  @Input()
  confidentiality: any;

  @Input()
  sectionId: string;

  sectionTitle: string;

  @Input()
  list: any[];


  /**
   * 
   */
  constructor(
    public reportSvc: ReportService,
    public tSvc: TranslocoService,
    public assessSvc: AssessmentService,
    public maturitySvc: MaturityService,
    public observationsSvc: ObservationsService
  ) { }

  ngOnInit(): void {
    if (!this.sectionId) {
      return;
    }

    const key = 'reports.launch.' + this.sectionId.toLowerCase() + '.sectionTitle';
    this.sectionTitle = this.tSvc.translate(key);
  }

  /**
   * 
   */
  onSelectSecurity(val) {
    this.confidentiality = val;
    this.reportSvc.confidentiality = val;
  }

  /**
   * Returns the translation, or an empty string
   */
  translateDesc(section: string, index: number): string {
    const key = 'reports.launch.' + section.toLowerCase() + '.' + (index + 1) + '.desc';
    const val = this.tSvc.translate(key);
    return val === key ? '' : val;
  }

  /**
   * Evaluates certain conditions to indicate if a report link
   * should be disabled.
   */
  isDisabled(condition: string) {
    return false;
  }
}
