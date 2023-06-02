import { NgModule } from '@angular/core';
import { AuthenticationComponent } from './components/authentication/authentication.component';
import { LoginComponent } from './components/login/login.component';
import { AUTHENTICATION_ROUTES } from './authentication.routes';
import { RouterModule } from '@angular/router';
import { DefaultSetModule } from '@shared/default.module';
import { RegisterComponent } from './components/register/register.component';
import { RestorePasswordComponent } from './components/restore-password/restore-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { VerifyEmailComponent } from './components/verify-email/verify-email.component';


@NgModule({
  declarations: [
    AuthenticationComponent,
    LoginComponent,
    RegisterComponent,
    RestorePasswordComponent,
    ResetPasswordComponent,
    VerifyEmailComponent
  ],
  imports: [
    DefaultSetModule,
    RouterModule.forChild(AUTHENTICATION_ROUTES),
  ],
  exports: [
    AuthenticationComponent
  ]
})
export class AuthenticationModule { }
