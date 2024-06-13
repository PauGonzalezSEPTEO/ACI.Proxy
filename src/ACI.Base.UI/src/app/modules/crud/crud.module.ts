import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrudComponent } from './crud.component';
import { DataTablesModule } from 'angular-datatables';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { TranslationModule } from '../../modules/i18n/translation.module';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';


@NgModule({
  declarations: [CrudComponent],
  imports: [
    CommonModule,
    DataTablesModule,
    TranslationModule,    
    SweetAlert2Module.forChild(),
    NgbModalModule,
  ],
  exports: [CrudComponent]
})
export class CrudModule { }
