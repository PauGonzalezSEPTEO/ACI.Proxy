import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AccountModel } from './models/account.model';
import { AccountService } from './services/account.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
})
export class AccountComponent implements OnInit {
  account?: AccountModel | null = null;

  constructor(
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.accountService.getAccount()
      .subscribe((data: AccountModel) => {
        this.account = data;
        this.changeDetectorRef.detectChanges();
    });    
    this.accountService.account.subscribe(account => {
      this.account = account;
      this.changeDetectorRef.detectChanges();
    });
  }
}
