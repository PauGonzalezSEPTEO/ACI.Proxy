import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AvatarComponent } from './avatar.component';
import { TranslationModule } from '../../modules/i18n/translation.module';
import { SharedModule } from 'src/app/_metronic/shared/shared.module';

@NgModule({
  declarations: [
    AvatarComponent
  ],
  imports: [
    CommonModule,
    TranslationModule,
    SharedModule
  ],
  exports: [
    AvatarComponent
  ]
})
export class AvatarModule { }