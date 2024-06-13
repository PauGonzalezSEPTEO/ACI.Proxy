import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, Observable } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { first } from 'rxjs/operators';
import { ConfirmEmailModel } from '../../models/confirm-email.model';

@Component({
  selector: 'app-registration',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss'],
})
export class ConfirmEmailComponent implements OnInit, OnDestroy {
  email?: string;
  errorMessage?: string;
  hasError: boolean;
  isLoading$: Observable<boolean>;
  token?: string;

  // private fields
  private unsubscribe: Subscription[] = [];

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.isLoading$ = this.authService.isLoading$;
  }

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParams['email'.toString()] || undefined;
    this.token = this.route.snapshot.queryParams['token'.toString()] || undefined;
    this.confirmEmail();
  }

  confirmEmail() {
    this.hasError = false;
    const confirmEmail = new ConfirmEmailModel();
    confirmEmail.set({
      email: this.email,
      token: this.token
    });
    const confirmEmailSubscr = this.authService
      .confirmEmail(confirmEmail)
      .pipe(first())
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.router.navigate(['/']);
        } else {
          this.errorMessage = data.message;
          this.hasError = true;
        }
      });
    this.unsubscribe.push(confirmEmailSubscr);
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
