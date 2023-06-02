import { Component } from '@angular/core';
import { FormGroup, FormBuilder, AbstractControl, Validators } from '@angular/forms';
import { ServerTextResponse } from '@shared/models/server-text-response';
import { NotificationService } from '@shared/services/notification.service';
import { first } from 'rxjs';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-restore-password',
  templateUrl: './restore-password.component.html',
  styleUrls: ['./restore-password.component.scss']
})
export class RestorePasswordComponent {
  form: FormGroup;
  isLinkSent: boolean = false;
  messageText: string = null;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly accountService: AccountService,
    private readonly notificationService: NotificationService,
  ) { }

  get email(): AbstractControl { return this.form.controls['email']; }

  get isLoading(): boolean { return this.accountService.isLoading; }

  get hasError(): boolean { return this.accountService.hasError; }

  get errorText(): string { return this.accountService.errorText; }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    });
  }

  onRestorePassword(): void {
    if (this.form.invalid) {
      return;
    }

    this.accountService.forgotPassword(this.email.value)
      .pipe(first())
      .subscribe({
        next: (result: ServerTextResponse) => {
          this.messageText = result.message;
          this.isLinkSent = true;
        },
        error: error => {
          this.notificationService.notifyError(error);
        }
    });
  }
}
