import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { ServerTextResponse } from "@shared/models/server-text-response";
import { NotificationService } from "@shared/services/notification.service";
import { Observable, catchError, of, map } from "rxjs";
import { AccountService } from "../services/account.service";

@Injectable({ providedIn: 'root' })
export class VerifyEmailGuard implements CanActivate {
  constructor(
    private readonly router: Router,
    private readonly loginService: AccountService,
    private readonly notificationService: NotificationService,
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const token = route.queryParamMap.get('token');
    if (token) {
      return this.loginService.verifyEmail(token)
        .pipe(
          catchError(error => { 
            this.notificationService.notifyError(error)
            return of(false); 
        }),
          map((result: ServerTextResponse) => {
            this.notificationService.notifySuccess(result.message);
            return true;
          })
        );
    } else {
      this.notificationService.notifyErrorMessage("Verification token wasn't provided");  
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return of(false);
    }
  }
}