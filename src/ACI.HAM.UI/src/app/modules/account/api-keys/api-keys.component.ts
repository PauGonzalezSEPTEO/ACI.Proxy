import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AccountModel } from '../models/account.model';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-api-keys',
  templateUrl: './api-keys.component.html',
})
export class ApiKeysComponent implements OnInit {
  account: AccountModel | null = null;
  errorMessage?: string;
  hasError: boolean;  

  constructor(
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef
  ) { }

  generateApiKey(): void {
    this.accountService.generateApiKey().subscribe((apiKey: string) => {

      //ToDo Pau
      console.log(apiKey);
      //

    });
  }

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