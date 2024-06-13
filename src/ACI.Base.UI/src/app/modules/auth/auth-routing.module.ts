import { NgModule, inject } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthComponent } from './auth.component';
import { LoginComponent } from './components/login/login.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { LogoutComponent } from './components/logout/logout.component';
import { ConfirmEmailComponent } from './components/confirm-email/confirm-email.component';
import { ConfirmEmailAndSetPasswordComponent } from './components/confirm-email-and-set-password/confirm-email-and-set-password.component';
import { ChangeEmailComponent } from './components/change-mail/change-email.component';
import { TwoFactorComponent } from './components/two-factor/two-factor.component';
import { RegistrationGuard } from './guards/registration.guard';

const routes: Routes = [
  {
    path: '',
    component: AuthComponent,
    children: [
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full',
      },
      {
        path: 'change-email',
        component: ChangeEmailComponent,
      },
      {
        path: 'confirm-email',
        component: ConfirmEmailComponent,
      },
      {
        path: 'confirm-email-and-set-password',
        component: ConfirmEmailAndSetPasswordComponent,
      },
      {
        path: 'login',
        component: LoginComponent,
        data: { returnUrl: window.location.pathname },
      },
      {
        path: 'registration',
        component: RegistrationComponent,
        canActivate: [() => inject(RegistrationGuard).canActivate()],
      },
      {
        path: 'reset-password',
        component: ResetPasswordComponent,
      },
      {
        path: 'forgot-password',
        component: ForgotPasswordComponent,
      },
      {
        path: 'logout',
        component: LogoutComponent,
      },
      {
        path: 'two-factor',
        component: TwoFactorComponent,
      },
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: '**', redirectTo: 'login', pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
