import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserListingComponent } from './user-listing/user-listing.component';
import { RouterModule } from '@angular/router';
import { CrudModule } from 'src/app/modules/crud/crud.module';
import { SharedModule } from 'src/app/_metronic/shared/shared.module';
import { NgbCollapseModule, NgbDropdownModule, NgbNavModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslationModule } from '../../modules/i18n/translation.module';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';

@NgModule({
  declarations: [UserListingComponent],
  imports: [
    CommonModule,
    TranslationModule,
    NgSelectModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild([
      {
        path: '',
        component: UserListingComponent,
      },
    ]),
    CrudModule,
    SharedModule,
    NgbNavModule,
    NgbDropdownModule,
    NgbCollapseModule,
    NgbTooltipModule,
    SweetAlert2Module.forChild(),
  ]
})
export class UserModule { }