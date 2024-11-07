import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject, first, Subscription } from 'rxjs';
import { AccountModel } from '../../../models/account.model';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'app-profile-details',
  templateUrl: './profile-details.component.html'
})
export class ProfileDetailsComponent implements OnInit, OnDestroy {
  @Input() set account(value: AccountModel) {
    this._account.next(value);
  }

  get account() {
    return this._account.getValue();
  }

  private _account = new BehaviorSubject<AccountModel>(new AccountModel());
  private unsubscribe: Subscription[] = [];

  errorMessage?: string;
  hasError: boolean;
  isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoading: boolean;
  profileDetailsForm: FormGroup;

  constructor(
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef,
    private fb: FormBuilder
  ) {
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.profileDetailsForm.controls;
  }

  initForm() {
    this.profileDetailsForm = this.fb.group({
      allowCommercialMail: [
        this.account?.allowCommercialMail
      ],
      avatar: [
        this.account?.avatar
      ],
      communicationByMail: [
        this.account?.communicationByMail
      ],
      communicationByPhone: [
        this.account?.communicationByPhone
      ],
      companyName: [
        this.account?.companyName,
        Validators.compose([
          Validators.minLength(2),
          Validators.maxLength(256),
        ])
      ],
      countryAlpha2Code: [
        this.account?.countryAlpha2Code
      ],
      currencyCode: [
        this.account?.currencyCode
      ],
      firstname: [
        this.account?.firstname,
        Validators.compose([
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(256),
        ])
      ],
      languageAlpha2Code: [
        this.account?.languageAlpha2Code
      ],
      lastname: [
        this.account?.lastname,
        Validators.compose([
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(256),
        ])
      ],
      phoneNumber: [
        this.account?.phoneNumber,
        Validators.compose([
          Validators.minLength(4),
          Validators.maxLength(15),
        ])
      ]
    });
    const loadingSubscr = this.accountService.isLoading$
      .subscribe((res) => {
        this.isLoading = res;
        this.changeDetectorRef.detectChanges();
    });  
    this.unsubscribe.push(loadingSubscr); 
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }

  ngOnInit(): void { 
    this._account.subscribe((data: AccountModel) => {
      this.initForm();
    })
  }

  saveSettings() {
    this.hasError = false;
    const result: {
      [key: string]: string;
    } = {};
    Object.keys(this.f).forEach((key) => {
      result[key] = this.f[key].value;
    });
    const newAccount = new AccountModel();
    newAccount.set(result);
    const accountSubscr = this.accountService
      .updateProfileDetails(newAccount)
      .pipe(first())
      .subscribe((data: any) => {
        if (data instanceof Error) {
          this.errorMessage = data.message;
          this.hasError = true;
        }
      });
    this.unsubscribe.push(accountSubscr);
  }
}