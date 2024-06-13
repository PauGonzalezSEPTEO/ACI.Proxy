import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LanguageComponent } from './language.component';
import { TranslationModule } from '../i18n/translation.module';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [LanguageComponent],
  imports: [
    FormsModule,
    NgSelectModule,
    TranslationModule
  ],
  exports: [LanguageComponent]
})
export class LanguageModule { }
