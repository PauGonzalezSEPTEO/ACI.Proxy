import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { RoleService } from '../../role/services/role-service';
import moment from 'moment';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { Role } from '../models/role.model';
import { RoleEditComponent } from '../role-edit/role-edit.component';

@Component({
  selector: 'app-role-details',
  templateUrl: './role-details.component.html',
  styleUrls: ['./role-details.component.scss']
})
export class RoleDetailsComponent implements OnInit {

  role$: Observable<Role>;
  datatableConfig: DataTables.Settings = {};
  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();
  @ViewChild(RoleEditComponent) roleEditComponent!: RoleEditComponent;

  constructor(private route: ActivatedRoute, private apiService: RoleService, private translate: TranslateService) {
    moment.locale(this.translate.currentLang);
    this.translate.onLangChange
      .subscribe((event: LangChangeEvent) => {
        moment.locale(event.lang);
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.role$ = this.apiService.get(id);
      var that = this;
      const capitalizeFirstLetter = (string: string) => {
        return string.charAt(0).toUpperCase() + string.slice(1);
      }
      this.datatableConfig = {
        serverSide: true,
        ajax: (dataTablesParameters: any, callback) => {
          this.apiService.readUsersDataTable(id, dataTablesParameters, this.translate.currentLang).subscribe(resp => {
            callback(resp);
          });
        },
        columns: [
          {
            title: this.translate.instant('ROLES.DETAILS.FIRSTNAME'), name: "ROLES.DETAILS.FIRSTNAME", data: 'firstname', render: function (data, _type, full) {
              const colorClasses = ['success', 'info', 'warning', 'danger'];
              const randomColorClass = colorClasses[Math.floor(Math.random() * colorClasses.length)];
              const initials = data[0].toUpperCase();
              const symbolLabel = `
                <div class="symbol-label fs-3 bg-light-${randomColorClass} text-${randomColorClass}">
                  ${initials}
                </div>
              `;
              const firstname = `
                <div class="d-flex flex-column" data-action="view" data-id="${full.id}">
                  <a href="javascript:;" class="text-gray-800 text-hover-primary mb-1">${data}</a>
                  <span>${full.firstname}</span>
                </div>
              `;
              return `
                <div class="symbol symbol-circle symbol-50px overflow-hidden me-3" data-action="view" data-id="${full.id}">
                  <a href="javascript:;">
                    ${symbolLabel}
                  </a>
                </div>
                ${firstname}
              `;
            }
          },
          {
            title: this.translate.instant('ROLES.DETAILS.LASTNAME'), name: "ROLES.DETAILS.LASTNAME", data: 'lastname', type: 'string'
          },
          {
            title: this.translate.instant('ROLES.DETAILS.EMAIL'), name: "ROLES.DETAILS.EMAIL", data: 'email', type: 'string'
          },
          {
            title: this.translate.instant('ROLES.DETAILS.ROLES'), name: "ROLES.DETAILS.ROLES", data: 'roleNames', render: function (_data, _type, full) {
              return full.roleNames?.join(', ') || '';
            },
            type: 'string'
          },
          {
            title: this.translate.instant('ROLES.DETAILS.LAST_LOGIN'), name: "ROLES.DETAILS.LAST_LOGIN", data: 'lastLoginDate', render: (data, _type, full) => {
              const date = data || full.lastLoginDate;
              let dateString;
              if (date === null) {
                  dateString = that.translate.instant('ROLES.DETAILS.NEVER');
              } else {
                dateString = moment(date).fromNow();
              }
              dateString = capitalizeFirstLetter(dateString);
              return `<div class="badge badge-light fw-bold">${dateString}</div>`;
            }
          },
          {
            title: this.translate.instant('ROLES.DETAILS.JOINED_DATE'), name: "ROLES.DETAILS.JOINED_DATE", data: 'createDate', render: (data, _type, full) => {
              const date = data || full.createDate;
              let dateString;
              if (date === null) {
                  dateString = that.translate.instant('ROLES.DETAILS.NEVER');
              } else {
                dateString = moment(date).fromNow();
              }
              dateString = capitalizeFirstLetter(dateString);
              return `<div class="badge badge-light fw-bold">${dateString}</div>`;
            }
          }
        ],
        createdRow: function (row, data, dataIndex) {
          $('td:eq(0)', row).addClass('d-flex align-items-center');
        },
      };
    });
  }

  onEditRoleClick(roleId: string) {
    if (this.roleEditComponent) {
      this.roleEditComponent.edit(roleId);
    }
  }

  unlinkUser(userId: string) {
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.apiService.deleteUser(id, userId).subscribe(() => {
        this.reloadEvent.emit(true);
      });
    });
  }
}
