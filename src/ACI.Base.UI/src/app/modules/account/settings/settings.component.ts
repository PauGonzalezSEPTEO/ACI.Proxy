import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AccountModel } from '../models/account.model';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
})
export class SettingsComponent implements OnInit {
  account: AccountModel | null = null;
  errorMessage?: string;
  hasError: boolean;  

  constructor(
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.hasError = false;
    this.accountService.account.subscribe(data => {
        if (!(data instanceof Error)) {
          this.account = data;
        } else {
          this.errorMessage = data.message;
          this.hasError = true;
        }
        this.changeDetectorRef.detectChanges();
    });
  };
}