import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AccountModel } from '../models/account.model';
import { AccountService } from '../services/account.service';
import { TranslationService } from '../../i18n/translation.service';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
})
export class OverviewComponent implements OnInit {
  account?: AccountModel | null = null;

  constructor(
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef,
    private translationService: TranslationService
  ) {}

  getCountryTranslation(): string | undefined {
    return this.translationService.getCountryTranslation(this.account?.countryAlpha2Code);
  }

  ngOnInit(): void {
    this.accountService.account.subscribe(account => {
        this.account = account;
        this.changeDetectorRef.detectChanges();
    });
  }
}
