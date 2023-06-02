import { Routes } from "@angular/router";
import { AuthenticationComponent } from "./components/authentication/authentication.component";
import { LoginComponent } from "./components/login/login.component";
import { RegisterComponent } from "./components/register/register.component";
import { ResetPasswordComponent } from "./components/reset-password/reset-password.component";
import { RestorePasswordComponent } from "./components/restore-password/restore-password.component";
import { VerifyEmailComponent } from "./components/verify-email/verify-email.component";
import { ResetPasswordGuard } from "./guards/reset-password.guard";

export const AUTHENTICATION_ROUTES: Routes = [
    {
      path: '',
      component: AuthenticationComponent,
      children: [
        {
          path: 'login',
          component: LoginComponent,
        },
        {
          path: 'restore',
          component: RestorePasswordComponent,
        },
        {
          path: 'reset-password',
          component: ResetPasswordComponent,
          canActivate: [ResetPasswordGuard],
        },
        {
          path: 'register',
          component: RegisterComponent,
        },
        {
          path: 'verify-email',
          component: VerifyEmailComponent,
        },
      ]
    }
  ];