import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BoardListingComponent } from './board-listing/board-listing.component';
import { RouterModule } from '@angular/router';
import { CrudModule } from 'src/app/modules/crud/crud.module';
import { SharedModule } from 'src/app/_metronic/shared/shared.module';
import { NgbCollapseModule, NgbDropdownModule, NgbNavModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslationModule } from '../../modules/i18n/translation.module';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { LanguageModule } from 'src/app/modules/language/language.module';

@NgModule({
  declarations: [
    BoardListingComponent
  ],
  imports: [
    CommonModule,
    TranslationModule,
    NgSelectModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild([
      {
        path: '',
        component: BoardListingComponent,
      }
    ]),
    CrudModule,
    LanguageModule,
    SharedModule,
    NgbNavModule,
    NgbDropdownModule,
    NgbCollapseModule,
    NgbTooltipModule,
    SweetAlert2Module.forChild(),
  ]
})
export class BoardModule { }