import { ChangeDetectorRef, Component, EventEmitter, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import moment from 'moment';

@Component({
  selector: 'app-api-keys',
  templateUrl: './api-keys.component.html',
})
export class ApiKeysComponent implements OnInit {
  apiKey?: string;
  datatableConfig: DataTables.Settings = {};
  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();  
  isReadOnly = false;

  constructor(
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef,
    private translate: TranslateService
  )
  {
    moment.locale(this.translate.currentLang);
    this.translate.onLangChange
      .subscribe((event: LangChangeEvent) => {
        moment.locale(event.lang);
    });    
  }
  
  create(): void {
    this.accountService.generateUserApiKey().subscribe((apiKey: string) => {
      this.apiKey = apiKey;
      this.reloadEvent.emit(true);
      this.changeDetectorRef.detectChanges();
    });
  }

  delete(id: number) {
    this.accountService.deleteUserApiKey(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  ngOnInit(): void {
    var that = this;
    const capitalizeFirstLetter = (string: string) => {
      return string.charAt(0).toUpperCase() + string.slice(1);
    }
    this.datatableConfig = {
      serverSide: true,
      ajax: (dataTablesParameters: any, callback) => {
        this.accountService.readUserApiKeysDataTable(dataTablesParameters, this.translate.currentLang).subscribe(resp => {
          callback(resp);
        });
      },      
      columns: [
        {
          title: this.translate.instant('ACCOUNT.API_KEYS.API_KEY'), name: "ACCOUNT.API_KEYS.API_KEY", data: "apiKeyLast6", render: function (data, _type, full) {
            return `
              <div class="d-flex flex-column">
                <span>${'*'.repeat(5) + full.apiKeyLast6}</span>
              </div>
            `;
          }
        },
        {
          title: this.translate.instant('ACCOUNT.API_KEYS.EXPIRATION'), name: "ACCOUNT.API_KEYS.EXPIRATION", data: 'expiration', render: (data, _type, full) => {
            const date = data || full.expiration;
            let dateString = moment(date).fromNow();
            dateString = capitalizeFirstLetter(dateString);
            return `<div class="badge badge-light fw-bold">${dateString}</div>`;
          }
        },        
        {
          title: this.translate.instant('ACCOUNT.API_KEYS.IS_ACTIVE'), name: "ACCOUNT.API_KEYS.IS_ACTIVE", data: 'isActive', 
          render: function (_data, _type, full) {            
            if (_data) {
              return '<span class="badge badge-light-success fs-7 fw-semibold">' + that.translate.instant('ACCOUNT.API_KEYS.ACTIVE') + '</span>';
            } else {
              return '<span class="badge badge-light-danger fs-7 fw-semibold">' + that.translate.instant('ACCOUNT.API_KEYS.INACTIVE') + '</span>';
            }
          },
          type: 'boolean'
        }
      ],
      order: [[2, 'desc']]
    };       
  };
}