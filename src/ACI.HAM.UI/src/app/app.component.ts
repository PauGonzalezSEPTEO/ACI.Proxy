import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { TranslationService } from './modules/i18n';
// language list
import { locale as enLang } from './modules/i18n/vocabs/en';
import { locale as esLang } from './modules/i18n/vocabs/es';
import * as enCountryLocale from 'i18n-iso-countries/langs/en.json';
import * as esCountryLocale from 'i18n-iso-countries/langs/es.json';
import { ThemeModeService } from './_metronic/partials/layout/theme-mode-switcher/theme-mode.service';

@Component({
  // tslint:disable-next-line:component-selector
  // eslint-disable-next-line @angular-eslint/component-selector
  selector: 'body[root]',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent implements OnInit {
  constructor(
    private translationService: TranslationService,
    private modeService: ThemeModeService
  ) {
    // register translations
    this.translationService.loadTranslations(
      enLang,
      esLang
    );
    this.translationService.loadCountryTranslations(
      enCountryLocale,
      esCountryLocale
    );
  }

  ngOnInit() {
    this.modeService.init();    
  }
}
