import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject, first, Subscription } from 'rxjs';
import { AuthService, ConfirmationPasswordValidator } from '../../../../auth';
import { AccountModel } from '../../../models/account.model';
import { ChangePasswordModel } from '../../../models/change-password.model';

enum ErrorStates {
  NotSubmitted,
  HasError,
  NoError
}

@Component({
  selector: 'app-sign-in-method',
  templateUrl: './sign-in-method.component.html',
})
export class SignInMethodComponent implements OnInit, OnDestroy {
  changeEmailForm: FormGroup;
  changePasswordForm: FormGroup;
  changeEmailErrorState: ErrorStates = ErrorStates.NotSubmitted;
  changePasswordErrorState: ErrorStates = ErrorStates.NotSubmitted;
  twoFactorEnabledErrorState: ErrorStates = ErrorStates.NotSubmitted;
  errorStates = ErrorStates;
  changeEmailErrorMessage?: string;
  changePasswordErrorMessage?: string;
  twoFactorEnabledErrorMessage?: string;
  changeEmailHasError: boolean;
  changePasswordHasError: boolean;
  twoFactorEnabledHasError: boolean;
  showChangeEmailForm: boolean = false;
  showChangePasswordForm: boolean = false;
  isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoading: boolean;
  private _account = new BehaviorSubject<AccountModel>(new AccountModel());
  private unsubscribe: Subscription[] = [];

  @Input() set account(value: AccountModel) {
    this._account.next(value);
  }

  get account() {
    return this._account.getValue();
  }

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    this._account.subscribe((data: AccountModel) => {
      this.initForm();
    })
  }

  // convenience getter for easy access to form fields
  get f1() {
    return this.changeEmailForm.controls;
  }

  get f2() {
    return this.changePasswordForm.controls;
  }

  initForm() {
    this.changeEmailForm = this.fb.group(
      {
        newEmail: [
          '',
          Validators.compose([
            Validators.required,
            Validators.email,
            Validators.minLength(3),
            Validators.maxLength(256), // https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
          ]),
        ]
      }
    );
    this.changePasswordForm = this.fb.group(
      {
        currentPassword: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(6),
            Validators.maxLength(100),
          ]),
        ],
        newPassword: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(6),
            Validators.maxLength(100),
          ]),
        ],
        newConfirmationPassword: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(100),
          ]),
        ]
      },
      {
        validator: ConfirmationPasswordValidator.MatchNewPassword
      }
    );
    const loadingSubscr = this.isLoading$
      .asObservable()
      .subscribe((res) => {
        this.isLoading = res;
        this.changeDetectorRef.detectChanges();
      });
    this.unsubscribe.push(loadingSubscr);
  }

  toggleEmailForm(show: boolean) {
    this.showChangeEmailForm = show;
  }

  changeEmail() {
    this.changeEmailErrorState = ErrorStates.NotSubmitted;
    this.changeEmailHasError = false;
    const changeEmailSubscr = this.authService
      .changeEmail(this.f1.newEmail.value)
      .pipe(first())
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.changeEmailErrorState = ErrorStates.NoError;
          this.changeEmailHasError = false;
        } else {
          this.changeEmailErrorState = ErrorStates.HasError;
          this.changeEmailErrorMessage = data.message;
          this.changeEmailHasError = true;
        }
      });
    this.unsubscribe.push(changeEmailSubscr);
  }

  togglePasswordForm(show: boolean) {
    this.showChangePasswordForm = show;
  }

  changePassword() {
    this.changePasswordErrorState = ErrorStates.NotSubmitted;
    this.changePasswordHasError = false;
    const result: {
      [key: string]: string;
    } = {};
    Object.keys(this.f2).forEach((key) => {
      result[key] = this.f2[key].value;
    });
    const newPassword = new ChangePasswordModel();
    newPassword.set(result);
    const changePasswordSubscr = this.authService
      .changePassword(newPassword)
      .pipe(first())
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.changePasswordErrorState = ErrorStates.NoError;
          this.changePasswordHasError = false;
        } else {
          this.changePasswordErrorState = ErrorStates.HasError;
          this.changePasswordErrorMessage = data.message;
          this.changePasswordHasError = true;
        }
      });
    this.unsubscribe.push(changePasswordSubscr);
  }

  setTwoFactorEnabled(enabled: boolean) {
    this.twoFactorEnabledErrorState = ErrorStates.NotSubmitted;
    this.twoFactorEnabledHasError = false;
    this.authService.setTwoFactorEnabled(enabled)
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.account.twoFactorEnabled = enabled;
          this.twoFactorEnabledErrorState = ErrorStates.NoError;
          this.twoFactorEnabledHasError = false;
        } else {
          this.twoFactorEnabledErrorState = ErrorStates.HasError;
          this.twoFactorEnabledErrorMessage = data.message;
          this.twoFactorEnabledHasError = true;
        }
      });
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}