import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription, Observable } from 'rxjs';
import { first } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserModel } from '../../models/user.model';
import { LockedOutModel } from '../../models/locked-out.model';
import { environment } from 'src/environments/environment';

enum ErrorStates {
  AccessFailedCount,
  LockedOut,
  HasError,
  NoError
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit, OnDestroy {
  register = environment.register;
  accessFailedCount: number = 0;
  errorMessage?: string;
  loginForm: FormGroup;
  errorStates = ErrorStates;
  errorState: ErrorStates = ErrorStates.NoError;
  hasError: boolean;
  returnUrl: string;
  isLoading$: Observable<boolean>;

  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

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
    // get return url from route parameters or default to '/'
    this.returnUrl =
      this.route.snapshot.queryParams['returnUrl'.toString()] || '/';
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.loginForm.controls;
  }

  initForm() {
    this.loginForm = this.fb.group({
      email: [
        '',
        Validators.compose([
          Validators.required,
          Validators.email,
          Validators.minLength(3),
          Validators.maxLength(256), // https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
        ]),
      ],
      password: [
        '',
        Validators.compose([
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(100),
        ]),
      ],
    });
  }

  submit() {
    this.hasError = false;
    const loginSubscr = this.authService
      .login(this.f.email.value, this.f.password.value)
      .pipe(first())
      .subscribe((data: UserModel | any) => {
        if ((data !== undefined) && (data instanceof LockedOutModel)) {
          if (data.isLockedOut) {
            this.errorState = ErrorStates.LockedOut;
          } else {
            this.errorState = ErrorStates.AccessFailedCount;
            this.accessFailedCount = data.accessFailedCount;
            this.hasError = true;
          }
        }
        else if ((data !== undefined) && !(data instanceof Error)) {
          this.errorState = ErrorStates.NoError;
          this.hasError = false;
          this.router.navigate([this.returnUrl]);
        }
        else if (data instanceof Error) {
          this.errorState = ErrorStates.HasError;
          this.errorMessage = data.message;
          this.hasError = true;
        }
      });
    this.unsubscribe.push(loginSubscr);
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
