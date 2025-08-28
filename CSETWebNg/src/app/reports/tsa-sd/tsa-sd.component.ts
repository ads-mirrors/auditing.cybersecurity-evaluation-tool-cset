import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AssessmentService } from '../../services/assessment.service';
import { MaturityService } from '../../services/maturity.service';

@Component({
    selector: 'app-tsa-sd',
    templateUrl: './tsa-sd.component.html',
    styleUrls: ['./tsa-sd.component.scss', '../reports.scss'],
    standalone: false
})
export class TsaSdComponent implements OnInit {

  domains: any[] = [];

  loading = false;

  responseU: any;
  responseS: any;
  assessmentName: string;
  assessmentDate: string;
  assessorName: string;
  facilityName: string;
  selfAssessment: boolean;

  constructor(
    public maturitySvc: MaturityService,
    public assessSvc: AssessmentService,
    public titleService: Title
  ) { }

  /**
   * 
   */
  ngOnInit(): void {
    this.titleService.setTitle("Deficiency Report - Pipeline SD02 Series");
    this.loading = true;

    this.assessSvc.getAssessmentDetail().subscribe((assessmentDetail: any) => {
      this.assessmentName = assessmentDetail.assessmentName;
      this.assessmentDate = assessmentDetail.assessmentDate;
      this.assessorName = assessmentDetail.facilitatorName;
      this.facilityName = assessmentDetail.facilityName;
      this.selfAssessment = assessmentDetail.selfAssessment;
    });

    this.maturitySvc.getMaturityDeficiencySd().subscribe(
      (r: any) => {
        this.loading = false;
        this.responseU = r.unanswered;
        this.responseS = r.no;
      });
  }

}
