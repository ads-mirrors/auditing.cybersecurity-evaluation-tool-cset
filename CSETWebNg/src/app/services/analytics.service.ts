import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ConfigService } from './config.service';


@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private apiUrl: string;
  private enterpriseUrl: string;
  public headers = {
    headers: new HttpHeaders().set('Content-Type', 'application/json'),
    params: new HttpParams()
  };



  constructor(private http: HttpClient, private configSvc: ConfigService) {
    this.apiUrl = this.configSvc.apiUrl + "";
    this.enterpriseUrl = this.configSvc.enterpriseUrl + "api/";

  }

  getAnalytics(): any {
    return this.http.get(this.apiUrl + 'getAggregation');
  }

  getAnalyticsToken(username, password): any {
    let header: HttpHeaders = new HttpHeaders();
    header = header.append('Content-Type', 'application/json');
    return this.http.post(
      this.apiUrl + 'auth/login', { "email":username, password }
    );
  }

  getTokenFromEnterprise(): any {
    let header: HttpHeaders = new HttpHeaders();
    header = header.append('Content-Type', 'application/json');
    let params: HttpParams = new HttpParams();
    return this.http.get(this.apiUrl + 'auth/GetToken');
  }

  postAnalyticsWithLogin(analytics, token): any {
    //let header: HttpHeaders = new HttpHeaders();
    //header = header.append('Content-Type', 'application/json');
    //header = header.append("Authorization", "Bearer " + token);
    //console.log(token);
    //console.log(analytics);
    //let params: HttpParams = new HttpParams();

    return this.http.get(
      this.apiUrl + 'assessment/exportandsend?token=' + token
    );
  }

  // pingAnalyticsService(): any {
    // return this.http.get(this.analyticsUrl + 'ping/GetPing');
  // }
}