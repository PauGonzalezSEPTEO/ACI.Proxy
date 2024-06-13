import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, Observable, first } from 'rxjs';
import { TwoFactorModel } from '../../models/two-factor.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-registration',
  templateUrl: './two-factor.component.html',
  styleUrls: ['./two-factor.component.scss'],
})
export class TwoFactorComponent implements OnInit, OnDestroy {
  errorMessage?: string;
  twoFactorForm: FormGroup;
  hasError: boolean;
  isLoading$: Observable<boolean>;
  email?: string;
  provider?: string;

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
    this.provider = this.route.snapshot.queryParams['provider'.toString()] || undefined;
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.twoFactorForm.controls;
  }

  initForm() {
    this.twoFactorForm = this.fb.group({
      token: [
        '',
        Validators.compose([
          Validators.required
        ]),
      ]
    });
  }

  submit() {
    this.hasError = false;
    const result: {
      [key: string]: string;
    } = {};
    Object.keys(this.f).forEach((key) => {
      result[key] = this.f[key].value;
    });
    const twoFactor = new TwoFactorModel();
    twoFactor.set({
      ...result,
      ...{
        email: this.email,
        provider: this.provider
      }
    });
    const twoFactorSubscr = this.authService
      .twoFactor(twoFactor)
      .pipe(first())
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.router.navigate(['/']);
        } else {
          this.errorMessage = data.message;
          this.hasError = true;
        }
      });
    this.unsubscribe.push(twoFactorSubscr);
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
