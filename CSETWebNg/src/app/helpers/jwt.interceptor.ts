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
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '../../../node_modules/@angular/router';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private router: Router) { }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {

    console.log('intercept');
    console.log(request.headers);

    const origHeadersObj = request.headers.keys().reduce((acc, key) => {
      acc[key] = request.headers.getAll(key);
      return acc;
    }, {});


    // add authorization header with jwt token if available 
    // and the requestor did not provide one
    if (!request.headers.has('authorization')) {
      console.log(1);
      if (
        localStorage.getItem('userToken') &&
        localStorage.getItem('userToken').length > 1
      ) {
        console.log(2);
        request.headers.append('Content-Type', 'application/json');
        request = request.clone({
          setHeaders: {
            Authorization: localStorage.getItem('userToken')
          }
        });
      }
    }

    // include any headers in the original request
    for (const key in origHeadersObj) {
      if (origHeadersObj.hasOwnProperty(key)) {
        console.log(`${key}: ${origHeadersObj[key]}`);
        request.headers.append(key, origHeadersObj[key]);
      }
    }

    console.log('after restore');
    console.log(request.headers);


    return next.handle(request)
      .pipe(
        catchError((event: any, caught: Observable<any>) => {

          if (event instanceof HttpErrorResponse) {
            const e = <HttpErrorResponse>event;

            // continue if this is a "business exception"
            if (e.status !== 500 && e.status !== 401) {
              throw event;
            }


            if (e.status === 401) {
              console.log('Error 401! Ejecting to login page!');
            }

            if (e.status === 500 || (e.error && e.error.ExceptionMessage === 'JWT invalid')) {
              console.log('JWT Invalid. logging out.');
            }

            const userToken = localStorage.getItem('userToken')
            localStorage.clear();
            this.router.navigate(['/home/login/eject'], { queryParams: { token: userToken } });

            return of({});
          }
        })
      );
  }
}
