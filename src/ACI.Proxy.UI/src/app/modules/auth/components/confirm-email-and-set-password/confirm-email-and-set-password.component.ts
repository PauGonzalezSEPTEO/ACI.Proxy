import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, Observable } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { first } from 'rxjs/operators';
import { ConfirmEmailAndSetPasswordModel } from '../../models/confirm-email-and-set-password.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationPasswordValidator } from '../registration/confirmation-password.validator';

@Component({
  selector: 'app-registration',
  templateUrl: './confirm-email-and-set-password.component.html',
  styleUrls: ['./confirm-email-and-set-password.component.scss'],
})
export class ConfirmEmailAndSetPasswordComponent implements OnInit, OnDestroy {
  confirmEmailAndSetPasswordForm: FormGroup;
  email?: string;
  errorMessage?: string;
  hasError: boolean;
  isLoading$: Observable<boolean>;
  emailToken?: string;
  passwordToken?: string;

  // private fields
  private unsubscribe: Subscription[] = [];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.isLoading$ = this.authService.isLoading$;
  }

  ngOnInit(): void {
    this.initForm();
    this.email = this.route.snapshot.queryParams['email'.toString()] || undefined;
    this.emailToken = this.route.snapshot.queryParams['email-token'.toString()] || undefined;
    this.passwordToken = this.route.snapshot.queryParams['password-token'.toString()] || undefined;
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.confirmEmailAndSetPasswordForm.controls;
  }

  initForm() {
    this.confirmEmailAndSetPasswordForm = this.fb.group(
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
    const confirmEmailAndSetPassword = new ConfirmEmailAndSetPasswordModel();
    confirmEmailAndSetPassword.set({
      ...result,
      ...{
            email: this.email,
            emailToken: this.emailToken,
            passwordToken: this.passwordToken
      }
    });
    const confirmEmailAndSetPasswordSubscr = this.authService
      .confirmEmailAndSetPassword(confirmEmailAndSetPassword)
      .pipe(first())
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.router.navigate(['/']);
        } else {
          this.errorMessage = data.message;
          this.hasError = true;
        }
      });
    this.unsubscribe.push(confirmEmailAndSetPasswordSubscr);
  }
}