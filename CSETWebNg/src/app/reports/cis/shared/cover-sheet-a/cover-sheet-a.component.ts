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
import { Component, Input, OnInit, OnChanges } from '@angular/core';

@Component({
    selector: 'app-cover-sheet-a',
    templateUrl: './cover-sheet-a.component.html',
    styleUrls: ['./cover-sheet-a.component.scss', '../../../../reports/reports.scss'],
    standalone: false
})
export class CoverSheetAComponent implements OnInit, OnChanges {

  @Input()
  title: string;

  @Input()
  assessmentName: string;

  @Input()
  assessorName: string;

  @Input()
  assessmentDate: Date;

  @Input()
  facilityName: string;

  infoParent: any;

  constructor() { }

  ngOnInit(): void {

  }

  ngOnChanges(): void {
    this.infoParent = {
      information: {
        assessment_Name: this.assessmentName,
        assessor_Name: this.assessorName,
        facility_Name: this.facilityName,
        assessment_Date: this.assessmentDate
      }
    }
  }

}
