import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AnalyticsService } from '../../../services/analytics.service';
import { NavigationService } from '../../../services/navigation/navigation.service';
import { strict } from 'assert';
import { MatDialog } from '@angular/material/dialog';
import { ConfigService } from '../../../services/config.service';
import { AnalyticsloginComponent } from '../analysis/analytics-login/analytics-login.component';
import { AuthenticationService } from '../../../services/authentication.service';
import { isNotFound } from '@angular/core/primitives/di';

@Component({
    selector: 'app-analytics',
    templateUrl: './analytics.component.html',
    styleUrls: ['./analytics.component.scss'],
    standalone: false
})
export class AnalyticsComponent implements OnInit {

    analytics: any = {
        demographics: {
            sectorId: 0,
            industryId: 0,
            sectorName: '',
            industryName: '',
            assets: '',
            size: '',
            alias: ''
        },
        questionAnswers: []
    };

    username: string = '';
    password: string = '';

    uploadInProgress = false;
    successMessage = '';
    errorMessage = '';

    /**
     *
     */
    constructor(private router: Router,
        public navSvc: NavigationService,
        private authSvc: AuthenticationService,
        public analyticsSvc: AnalyticsService,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private dialog: MatDialog,
        private config: ConfigService
    ) { }

    /**
     *
     */
    ngOnInit() {
        this.navSvc.navItemSelected.asObservable().subscribe((value: string) => {
            this.router.navigate([value], { relativeTo: this.route.parent });
        });
        this.route.params.subscribe(params => {
            this.getAnalytics();
        });
    }

    /**
     *
     */
    getAnalytics() {
        this.analyticsSvc.getAnalytics().subscribe(
            (data: any) => {
                this.analytics = data;
            },
            error => {
                console.log('Error getting all documents: ' + (<Error>error).name + (<Error>error).message);
                console.log('Error getting all documents: ' + (<Error>error).stack);
            });
    }

    /**
     * Submit With Login
     */
    startUpload() {
        this.successMessage = '';
        this.errorMessage = '';
        this.uploadInProgress = true;

        // if we already signed in and have the remote token, submit without re-asking for credentials
        var remoteToken = localStorage.getItem('remoteToken') ?? '';

        if (remoteToken.trim().length == 0) {
            this.showLoginDialog();
            return;
        }

        this.analyticsSvc.isRemoteTokenValid(remoteToken).subscribe((isValidRemoteToken) => {
            if (isValidRemoteToken.toString().toLowerCase() == 'true') {

                this.analyticsSvc.postAnalytics(remoteToken).subscribe(x => {
                    this.uploadInProgress = false;
                    this.successMessage = 'The assessment has been uploaded to the enterprise server';
                },
                    error => {
                        this.uploadInProgress = false;
                        this.errorMessage = '<i class="fa fa-triangle-exclamation me-2"></i>' + error.error;
                    });
                return;
            } else {
                this.showLoginDialog();
            }
        },
            (error) => {
                this.uploadInProgress = false;
                console.warn('call to validate remote token error');
                console.warn(<Error>error.message);
                this.errorMessage = error.message;
            }
        );
    }

    /**
     *
     */
    showLoginDialog() {
        this.uploadInProgress = false;

        // if we dont have a valid remote token, prompt for credentials
        const dialogRef = this.dialog.open(AnalyticsloginComponent, {
            width: '300px',
            disableClose: true,
            data: this.analytics
        }).afterClosed().subscribe(info => {
            if (!!info) {
                if (info.cancel) {
                    // user canceled, do nothing
                } else {
                    this.successMessage = info;
                }
            }
        });
    }

    /**
     *
     */
    logoutRemote() {
        localStorage.removeItem('remoteToken');
        this.successMessage = 'Logout complete';
        this.errorMessage = '';
    }

    /**
     *
     */
    getRawData() {
        return JSON.stringify(this.analytics);
    }

    /**
     *
     * @param val
     */
    isNullOrEmpty(val) {
        if (!val || val.length == 0) {
            return true;
        }
        return false;
    }

    /**
     *
     */
    openSnackBar(message) {
        this.snackBar.open(message, "", {
            duration: 4000,
            verticalPosition: 'top',
            panelClass: ['green-snackbar']
        });
    }
}
