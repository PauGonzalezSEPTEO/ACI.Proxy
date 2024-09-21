import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription, Observable } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ConfirmationPasswordValidator } from '../registration/confirmation-password.validator';
import { first } from 'rxjs/operators';
import { ResetPasswordModel } from '../../models/reset-password.model';

@Component({
  selector: 'app-registration',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
  resetPasswordForm: FormGroup;
  email?: string;
  errorMessage?: string;
  hasError: boolean;
  isLoading$: Observable<boolean>;
  token?: string;

  // private fields
  private unsubscribe: Subscription[] = [];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.isLoading$ = this.authService.isLoading$;
    // redirect to home if already logged in
    if (this.authService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    this.initForm();
    this.email = this.route.snapshot.queryParams['email'.toString()] || undefined;
    this.token = this.route.snapshot.queryParams['token'.toString()] || undefined;
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.resetPasswordForm.controls;
  }

  initForm() {
    this.resetPasswordForm = this.fb.group(
      {
        password: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(6),
            Validators.maxLength(100),
          ]),
        ],
        confirmationPassword: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(100),
          ]),
        ]
      },
      {
        validator: ConfirmationPasswordValidator.MatchPassword,
      }
    );
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }  

  submit() {
    this.hasError = false;
    const result: {
      [key: string]: string;
    } = {};
    Object.keys(this.f).forEach((key) => {
      result[key] = this.f[key].value;
    });
    const newPassword = new ResetPasswordModel();
    newPassword.set({
      ...result,
      ...{
            email: this.email,
            token: this.token
      }
    });
    const resetPasswordSubscr = this.authService
      .resetPassword(newPassword)
      .pipe(first())
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.router.navigate(['/']);
        } else {
          this.errorMessage = data.message;
          this.hasError = true;
        }
      });
    this.unsubscribe.push(resetPasswordSubscr);
  }
}