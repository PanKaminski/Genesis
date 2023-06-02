import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { StorageKey } from "@shared/enums/storage-key";
import { NotificationService } from "@shared/services/notification.service";
import { StorageManager } from "@shared/services/storage-manager.service";
import { catchError, map, Observable, of } from "rxjs";
import { AccountService } from "../services/account.service";

@Injectable({ providedIn: 'root' })
export class ResetPasswordGuard implements CanActivate {
  constructor(
    private readonly router: Router,
    private readonly loginService: AccountService,
    private readonly localStorage: StorageManager,
    private readonly notificationService: NotificationService,
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const token = route.queryParamMap.get('token');
    if (token) {
      return this.loginService.validateResetToken(token)
        .pipe(
          catchError(error => { 
            this.notificationService.notifyError(error)
            return of(false); 
          }),
          map(() => {
            this.localStorage.saveItem(StorageKey.ResetToken, token);
            return true;
          })
        );
    } else {
      this.notificationService.notifyErrorMessage('Reset token was not provided')
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return of(false);
    }
  }
}
