import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { ThemeModeSwitcherComponent } from './theme-mode-switcher.component';
import { SharedModule } from "../../../shared/shared.module";
import { TranslationModule } from '../../../../modules/i18n/translation.module';

@NgModule({
  declarations: [ThemeModeSwitcherComponent],
  imports: [
    CommonModule,
    TranslationModule,    
    InlineSVGModule,
    SharedModule
  ],
  exports: [ThemeModeSwitcherComponent],
})
export class ThemeModeModule {}
