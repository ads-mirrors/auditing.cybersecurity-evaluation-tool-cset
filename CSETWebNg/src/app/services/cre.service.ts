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
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';
import { Observable } from 'rxjs';

const headers = {
  headers: new HttpHeaders().set('Content-Type', 'application/json'),
  params: new HttpParams()
};

@Injectable({
  providedIn: 'root'
})
export class CreService {

  public colorScheme = {
    domain: ['#5AA454', '#367190', '#b17300', '#DC3545', '#EEEEEE']
  }

  /**
   * 
   */
  constructor(
    private configSvc: ConfigService,
    private http: HttpClient
  ) { }

  /*************
  Label and tooltip formatting functions 
  ***************/

  fmt1 = (value) => {
    return `${Math.round(value)}%`;
  };

  fmt3 = (obj) => {
    return `${obj.data.name}<br>${Math.round(obj.data.value)}%`;
  }



  /**
   * 
   */
  getAllAnswerDistrib(modelIds: number[]): Observable<any[]> {
    return this.http.get<any[]>(this.configSvc.apiUrl + `chart/maturity/answerdistribs/all?modelIds=${modelIds.join('|')}`);
  }

  /**
   * 
   */
  getDomainAnswerDistrib(modelIds: number[]): Observable<any[]> {
    return this.http.get<any[]>(this.configSvc.apiUrl + `chart/maturity/answerdistrib/domain?modelIds=${modelIds.join('|')}`);
  }

  /**
   * 
   */
  getDistribForModel(modelId: number): Observable<any[]> {
    return this.http.get<any[]>(this.configSvc.apiUrl + `chart/maturity/answerdistrib/model?modelId=${modelId}`);
  }
}
