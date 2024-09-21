import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject, Subscription } from 'rxjs';
import { AuthService } from '../../../../auth';

@Component({
  selector: 'app-deactivate-account',
  templateUrl: './deactivate-account.component.html',
})
export class DeactivateAccountComponent implements OnInit, OnDestroy {
  deactivateAccountForm: FormGroup;
  errorMessage?: string;
  hasError: boolean;
  isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoading: boolean;

  // private fields
  private unsubscribe: Subscription[] = [];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.deactivateAccountForm = this.fb.group(
      {
        confirm: [false, Validators.compose([Validators.required])],
      }
    );
    const loadingSubscr = this.authService.isLoading$
      .subscribe((res) => {
        this.isLoading = res;
        this.changeDetectorRef.detectChanges();
      });
    this.unsubscribe.push(loadingSubscr);
  }

  submit() {
    this.authService.deactivate()
      .subscribe((data: any) => {
        if (!(data instanceof Error)) {
          this.authService.logout();
        } else {
          this.errorMessage = data.message;
          this.hasError = true;
        }
      });
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}