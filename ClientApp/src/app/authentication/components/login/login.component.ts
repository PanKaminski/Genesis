import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '@shared/services/notification.service';
import { first } from 'rxjs';
import { AccountService } from '../../services/account.service';
import { BaseAuthenticationComponent } from '../base-authentication.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent extends BaseAuthenticationComponent implements OnInit {
  form: FormGroup;

  constructor(
    accountService: AccountService,
    private readonly formBuilder: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly notificationService: NotificationService,
  ) { super(accountService) }

  get email(): AbstractControl { return this.form.controls['email']; }

  get password(): AbstractControl { return this.form.controls['password']; }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
    });
  }

  onSignIn() : void {
    if (this.form.invalid) {
        return;
    }

    this.accountService.login(this.email.value, this.password.value)
      .pipe(first())
      .subscribe({
        next: () => {
          const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard/trees';
          this.notificationService.notifySuccess('Login success');
          this.router.navigateByUrl(returnUrl);
        },
        error: error => {
          this.notificationService.notifyError(error);
        }
    });
  }
}
