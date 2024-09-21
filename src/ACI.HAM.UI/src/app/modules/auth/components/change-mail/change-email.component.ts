import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, Observable } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { first } from 'rxjs/operators';
import { ChangeEmailModel } from '../../models/change-email.model';

@Component({
  selector: 'app-registration',
  templateUrl: './change-email.component.html',
  styleUrls: ['./change-email.component.scss'],
})
export class ChangeEmailComponent implements OnInit, OnDestroy {
  email?: string;
  newEmail?: string;
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
    this.newEmail = this.route.snapshot.queryParams['newEmail'.toString()] || undefined;
    this.token = this.route.snapshot.queryParams['token'.toString()] || undefined;
    this.changeEmail();
  }

  changeEmail() {
    this.hasError = false;
    const changeEmail = new ChangeEmailModel();
    changeEmail.set({
      email: this.email,
      newEmail: this.newEmail,
      token: this.token
    });
    const changeEmailSubscr = this.authService
      .confirmNewEmail(changeEmail)
      .pipe(first())
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.router.navigate(['/']);
        } else {
          this.errorMessage = data.message;
          this.hasError = true;
        }
      });
    this.unsubscribe.push(changeEmailSubscr);
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
