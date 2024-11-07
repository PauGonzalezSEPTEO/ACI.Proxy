import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RoleListingComponent } from './role-listing/role-listing.component';
import { RoleDetailsComponent } from './role-details/role-details.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbNavModule, NgbDropdownModule, NgbCollapseModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslationModule } from '../../modules/i18n/translation.module';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { SharedModule } from 'src/app/_metronic/shared/shared.module';
import { CrudModule } from 'src/app/modules/crud/crud.module';
import { LanguageModule } from 'src/app/modules/language/language.module';
import { RoleEditComponent } from './role-edit/role-edit.component';

@NgModule({
  declarations: [RoleDetailsComponent, RoleListingComponent, RoleEditComponent],
  imports: [
    CommonModule,
    TranslationModule,    
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild([
      {
        path: '',
        component: RoleListingComponent,
      },
      {
        path: ':id',
        component: RoleDetailsComponent,
      },
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
export class RoleModule { }
