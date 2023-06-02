import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '@shared/services/notification.service';
import { ServerTextResponse } from '@shared/models/server-text-response';
import { first } from 'rxjs';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-verify-email',
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.scss']
})
export class VerifyEmailComponent implements OnInit {
  errorText: string;

  constructor(
      private route: ActivatedRoute,
      private router: Router,
      private accountService: AccountService,
      private notificationService: NotificationService
  ) { }

  ngOnInit() {
      const token = this.route.snapshot.queryParams['token'];

      this.router.navigate([], { relativeTo: this.route, replaceUrl: true });

      this.accountService.verifyEmail(token)
        .pipe(first())
        .subscribe({
          next: (result: ServerTextResponse) => {
            this.notificationService.notifySuccess(result.message);
            this.router.navigate(['/login'], { relativeTo: this.route });
          },
          error: (errorText: string) => {
            this.errorText = errorText;
          }
      });
  }
}
