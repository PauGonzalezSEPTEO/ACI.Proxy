import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AuthRoutingModule } from './auth-routing.module';
import { LoginComponent } from './components/login/login.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { LogoutComponent } from './components/logout/logout.component';
import { AuthComponent } from './auth.component';
import { TranslationModule } from '../i18n/translation.module';
import { ConfirmEmailComponent } from './components/confirm-email/confirm-email.component';
import { ConfirmEmailAndSetPasswordComponent } from './components/confirm-email-and-set-password/confirm-email-and-set-password.component';
import { ChangeEmailComponent } from './components/change-mail/change-email.component';
import { TwoFactorComponent } from './components/two-factor/two-factor.component';

@NgModule({
  declarations: [
    LoginComponent,
    ConfirmEmailComponent,
    ConfirmEmailAndSetPasswordComponent,
    ChangeEmailComponent,
    TwoFactorComponent,
    RegistrationComponent,
    ResetPasswordComponent,
    ForgotPasswordComponent,
    LogoutComponent,
    AuthComponent,
  ],
  imports: [
    CommonModule,
    TranslationModule,
    AuthRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
})
export class AuthModule {}
