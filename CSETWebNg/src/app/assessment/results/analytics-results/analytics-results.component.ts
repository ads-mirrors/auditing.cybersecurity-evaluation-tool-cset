import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AnalyticsService } from '../../../services/analytics.service';
import { NavigationService } from '../../../services/navigation/navigation.service';
import Chart, { ChartConfiguration, ChartType, registerables } from 'chart.js/auto';
import { AssessmentService } from '../../../services/assessment.service';
import { AggregationService } from '../../../services/aggregation.service';
import { AssessmentDetail } from '../../../models/assessment-info.model';
import { DemographicService } from '../../../services/demographic.service';
import { DemographicIodService } from '../../../services/demographic-iod.service';

Chart.register(...registerables);

interface Sector {
  sectorId: number;
  sectorName: string;
}

interface DemographicsIod {
  listSectors: listSectors[];
}

interface listSectors {
  optionValue: number;
  optionText: string;
}
@Component({
  selector: 'app-analytics-results',
  templateUrl: './analytics-results.component.html',
  styleUrls: ['./analytics-results.component.scss']
})
export class AnalyticsResultsComponent implements OnInit {

  sectorId: number;
  assessmentId: any;
  modelId: any; 
  minData: number[] = [];
  medianData: number[] = [];
  maxData: number[] = [];
  currentUserData: number[] = [];
  labels: string[] = [];
  sectorsList: Sector[];
  sectorTitle: string;
  showSector: boolean = true;
  sampleSize: number;
  allSectors: string = 'All Sectors';


  @ViewChild('barCanvas') private barCanvas!: ElementRef<HTMLCanvasElement>;
  private barChart!: Chart;

  // result from API call
  scoreBarData: any;

  // Toggle state
  dataType: "mySector" | "allSectors" = "mySector";

  constructor(
    public navSvc: NavigationService,
    public analyticsSvc: AnalyticsService,
    public assessSvc: AssessmentService,
    public aggregSvc: AggregationService, 
    public demoSvc: DemographicService,
    public demoIodSvc: DemographicIodService
  ) { }

  ngOnInit(): void {
    this.assessSvc.getAssessmentDetail().subscribe((resp: AssessmentDetail) => {
      this.assessmentId = resp.id;
      this.sectorId = resp.sectorId;
      this.modelId = resp.maturityModel.modelId;
      if (this.sectorId == null){
        this.showSector = false; 
        this.dataType = "allSectors"
      }
      let isCISA = this.analyticsSvc.isCisaAssessorMode()
      if (isCISA){
        this.demoIodSvc.getDemographics().subscribe((resp: DemographicsIod) => {
          resp.listSectors.forEach(sector => {
            if (sector.optionValue == this.sectorId){
              this.sectorTitle = sector.optionText
            }
          });
        })
      } else {
        this.demoSvc.getAllSectors().subscribe(
          (data: Sector[]) => {
              this.sectorsList = data;
              this.sectorsList.forEach(sector => {
                if (sector.sectorId == this.sectorId){
                  this.sectorTitle = sector.sectorName
                }
              });
          }
        )
      }
      

      // Fetch initial data after getting assessment details
      this.getAnalyticsResults();
    });

   
  }

  ngAfterViewInit(): void {
    // Initialize the chart after the view is initialized
    this.initializeChart();
  }

  // Get analytics results for specified sector 
  private async getAnalyticsResults(allSectors?: boolean): Promise<void> {
    try {
      let result = null;
      if (allSectors){
        result = await this.analyticsSvc.getAnalyticResults(this.assessmentId, this.modelId).toPromise();
      } else {
        result = await this.analyticsSvc.getAnalyticResults(this.assessmentId, this.modelId, this.sectorId).toPromise();
      }
      //this.setData(result);


      this.scoreBarData = result;
      this.sampleSize = result.sampleSize;



    } catch (error) {
      console.error('Error fetching analytics results', error);
    }
  }

  private setData(result: any): void {
    this.minData = result.min || [];
    this.medianData = result.median || [];
    this.maxData = result.max || [];
    this.currentUserData = result.barData?.values || [];
    this.labels = result.barData?.labels || [];
    this.sampleSize = result.sampleSize;
    if (this.barChart) {
      this.updateChart();
    }
  }

  private initializeChart(): void {
    const data: ChartConfiguration<'bar'>['data'] = {
      labels: this.labels,
      datasets: [
        {
          label: 'Min',
          data: this.minData,
          backgroundColor: 'rgba(178, 29, 45, 1)',
          borderWidth: 1
        },
        {
          label: 'Median',
          data: this.medianData,
          backgroundColor: 'rgba(216, 173, 30, 1)',
          borderWidth: 1
        },
        {
          label: 'Max',
          data: this.maxData,
          backgroundColor: 'rgba(16, 145, 71, 1)',
          borderWidth: 1
        },
        {
          label: 'My Assessment',
          data: this.currentUserData,
          backgroundColor: 'rgba(29, 136, 230, 1)',
          borderWidth: 1
        }
      ]
    };

    const options: ChartConfiguration<'bar'>['options'] = {
      indexAxis: 'y',
      responsive: true,
      scales: {
        x: {
          beginAtZero: true,
          min: 0,
          max: 100
        },
        y: {
          beginAtZero: true,
        }
      }
    };

    this.barChart = new Chart(this.barCanvas.nativeElement, {
      type: 'bar' as ChartType,
      data: data,
      options: options
    });
  }

  private updateChart(): void {
    this.barChart.data.labels = this.labels;
    this.barChart.data.datasets[0].data = this.minData;
    this.barChart.data.datasets[1].data = this.medianData;
    this.barChart.data.datasets[2].data = this.maxData;
    this.barChart.data.datasets[3].data = this.currentUserData;
    this.barChart.update();
  }

  toggleData(event: any): void {
    this.dataType = event.value;
    if (this.dataType === "allSectors") {
      this.getAnalyticsResults(true);
    } else {
      this.getAnalyticsResults();
    }
  }

}
