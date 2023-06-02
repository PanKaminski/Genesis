import { Component } from '@angular/core';
import { FormGroup, FormBuilder, AbstractControl, Validators } from '@angular/forms';
import { StorageKey } from '@shared/enums/storage-key';
import { ServerTextResponse } from '@shared/models/server-text-response';
import { NotificationService } from '@shared/services/notification.service';
import { StorageManager } from '@shared/services/storage-manager.service';
import { first } from 'rxjs';
import { ResetPasswordRequest } from '../../models/reset-password-request';
import { AccountService } from '../../services/account.service';
import { BaseAuthenticationComponent } from '../base-authentication.component';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent extends BaseAuthenticationComponent {
  form: FormGroup;
  isLinkSent: boolean = false;
  messageText: string = null;
  private token: string = null;

  constructor(
    accountService: AccountService,
    private readonly formBuilder: FormBuilder,
    private readonly storage: StorageManager,
    private readonly notificationService: NotificationService,
  ) { super(accountService); }

  get password(): AbstractControl { return this.form.controls['password']; }

  get confirmPassword(): AbstractControl { return this.form.controls['confirmPassword']; }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
    password: ['', [Validators.required]],
    confirmPassword: ['', [Validators.required]],
    });

    this.token = this.storage.getItem(StorageKey.ResetToken) as string;
  }

  onResetPassword(): void {
    if (this.form.invalid || !this.token) {
      return;
    }

    this.accountService.resetPassword({ 
      token: this.token, 
      password: this.password.value, 
      confirmPassword: this.confirmPassword.value, 
    } as ResetPasswordRequest)
      .pipe(first())
      .subscribe({
        next: (result: ServerTextResponse) => {
          this.messageText = result.message;
        },
        error: error => {
          this.notificationService.notifyError(error);
        }
    });
  }
}
