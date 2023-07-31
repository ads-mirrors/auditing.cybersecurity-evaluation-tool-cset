import { Component, Input } from '@angular/core';
import { CrrService } from '../../../services/crr.service';
import { CrrReportModel } from '../../../models/reports.model';

@Component({
  selector: 'app-cmu-goal-perf-stacked-bar',
  templateUrl: './cmu-goal-perf-stacked-bar.component.html',
  styleUrls: ['./cmu-goal-perf-stacked-bar.component.scss']
})
export class CmuGoalPerfStackedBarComponent {
  @Input() modelName: string;
  @Input() model: CrrReportModel;

  mil1FullAnswerDistribChart: string = '';
  legend: string = '';
  scoreBarCharts: string[] = [];
  stackedBarCharts: any[] = [];

  constructor(private crrSvc: CrrService) { }

  ngOnInit(): void {
    this.crrSvc.getMil1FullAnswerDistribWidget().subscribe((resp: string) => {
      this.mil1FullAnswerDistribChart = resp;
    })

    this.crrSvc.getMil1PerformanceSummaryLegendWidget().subscribe((resp: string) => {
      this.legend = resp;
    })

    this.crrSvc.getMil1PerformanceSummaryBodyCharts().subscribe((resp: any) => {
      this.scoreBarCharts = resp.scoreBarCharts;
      this.stackedBarCharts = resp.stackedBarCharts;
    })
  }

  // This function splits strings like
  // "Goal 6 - Post-incident lessons learned are translated into improvement strategies."
  // and
  // "Goal 3-Risks are identified."
  stringSplitter(str: string) {
    return str.split(" - ")[1] ?? str.split("-")[1];
  }

  getStackedBarChart(goalTitle: string) {
    return this.stackedBarCharts.find(c => c.title === goalTitle)?.chart;
  }

  filterMilDomainGoals(goals) {
    return goals.filter(g => !g.title.startsWith('MIL'));
  }
}
