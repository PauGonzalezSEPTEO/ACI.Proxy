import {NgModule} from '@angular/core';
import {KeeniconComponent} from './keenicon/keenicon.component';
import {CommonModule} from "@angular/common";
import { TranslationsValidatorDirective } from '../../modules/language/language.validator';

@NgModule({
  declarations: [
    KeeniconComponent,
    TranslationsValidatorDirective
  ],
  imports: [
    CommonModule,
  ],
  exports: [
    KeeniconComponent,
    TranslationsValidatorDirective
  ]
})
export class SharedModule {
}
