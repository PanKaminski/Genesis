import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { NotificationService } from "@shared/services/notification.service";
import { catchError, Observable, throwError } from "rxjs";
import { AccountService } from '../../authentication/services/account.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(
        private readonly accountService: AccountService,
        private readonly notifier: NotificationService,
        private readonly router: Router,
    ) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if ([401, 403].includes(err.status) && this.accountService.account) {
                this.accountService.logout();
            } else if (err.status === 404) {
                this.router.navigate(['/404']);
            }

            const error = (err && err.error && err.error.message) || err.statusText;
            if (error) this.notifier.notifyErrorMessage(error);
            return throwError(() => error);
        }))
    }
}
