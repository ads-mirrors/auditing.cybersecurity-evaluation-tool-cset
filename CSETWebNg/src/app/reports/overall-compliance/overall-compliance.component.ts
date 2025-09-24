////////////////////////////////
//
//   Copyright 2025 Battelle Energy Alliance, LLC
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
////////////////////////////////
import { Component, OnInit } from '@angular/core';
import { ReportAnalysisService } from '../../services/report-analysis.service';
import Chart from 'chart.js/auto';

@Component({
    selector: 'app-overall-compliance',
    templateUrl: './overall-compliance.component.html',
    styleUrls: ['../reports.scss'],
    standalone: false
})
export class OverallComplianceComponent implements OnInit {

  chartPercentCompliance: Chart;

  loading = true;

  /**
   * 
   */
  constructor(
    public analysisSvc: ReportAnalysisService
  ) { }

  /**
   * 
   */
  ngOnInit(): void {
    this.analysisSvc.getDashboard().subscribe(x => {
      this.loading = false;
      setTimeout(() => {
        this.chartPercentCompliance = this.analysisSvc.buildPercentComplianceChart('canvasCompliance', x);
      }, 0);
    });
  }

}
