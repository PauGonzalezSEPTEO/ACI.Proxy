import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormArray, NgForm } from '@angular/forms';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { Observable } from 'rxjs';
import { UserService } from '../services/user-service';
import { SweetAlertOptions } from 'sweetalert2';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { User } from '../models/user.model';
import { RoleService } from '../../role/services/role-service';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';
import { Role } from '../../role/models/role.model';
import { AuthService } from 'src/app/modules/auth';
import { Company } from '../../company/models/company.model';
import { CompanyService } from '../../company/services/company-service';
import { HotelService } from '../../hotel/services/hotel-service';
import moment from 'moment';

@Component({
  selector: 'app-user-listing',
  templateUrl: './user-listing.component.html',
  styleUrls: ['./user-listing.component.scss']
})
export class UserListingComponent implements OnInit, OnDestroy {

  isReadOnly = false;
  isCollapsed1 = false;
  isCollapsed2 = true;
  isLoading = false;
  users: DataTablesResponse;
  datatableConfig: DataTables.Settings = {};
  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();
  // Single model
  aUser: Observable<User>;
  userModel: User = new User();
  @ViewChild('noticeSwal')
  noticeSwal!: SwalComponent;
  swalOptions: SweetAlertOptions = {};
  roles$: Observable<Role[]>;
  companiesList: Company[] | null = null;

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private authService: AuthService,
    private companyService: CompanyService,        
    private hotelService: HotelService,        
    private cdr: ChangeDetectorRef,
    private translate: TranslateService
  )
  {
    moment.locale(this.translate.currentLang);
    this.translate.onLangChange
      .subscribe((event: LangChangeEvent) => {
        moment.locale(event.lang);
    });
  }

  create() {
    this.userModel = new User();
  }

  delete(id: string) {
    this.userService.delete(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  edit(event: { id: string, isReadOnly: boolean }) {
    this.isReadOnly = event.isReadOnly;
    this.aUser = this.userService.get(event.id);
    this.aUser.subscribe((user: User) => {
      this.userModel = new User(user);
      this.loadHotelsForSelectedCompanies();
    });
  }

  email(email: string) {
    this.authService.resendVerifyEmail(email).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  extractText(obj: any): string {
    var textArray: string[] = [];
    for (var key in obj) {
      if (typeof obj[key] === 'string') {
        // If the value is a string, add it to the 'textArray'
        textArray.push(obj[key]);
      } else if (typeof obj[key] === 'object') {
        // If the value is an object, recursively call the function and concatenate the results
        textArray = textArray.concat(this.extractText(obj[key]));
      }
    }
    // Use a Set to remove duplicates and convert back to an array
    var uniqueTextArray = Array.from(new Set(textArray));
    // Convert the uniqueTextArray to a single string with line breaks
    var text = uniqueTextArray.join('\n');
    return text;
  }

  isAtLeastOneRoleSelected() {
    return this.userModel?.isAtLeastOneRoleSelected();
  }

  loadHotelsForSelectedCompanies() {
    const selectedCompanyIds = this.userModel.companies;
    this.hotelService.getByCompanyIds(selectedCompanyIds).subscribe(hotels => {
      const selectedHotelIds = this.userModel.hotels;
      this.userModel.hotelsList = hotels
      this.userModel.hotels = hotels
        .filter(hotel => selectedHotelIds.includes(hotel.id))
        .map(hotel => hotel.id);
    });
  }

  ngOnDestroy(): void {
    this.reloadEvent.unsubscribe();
  }

  ngOnInit(): void {
    const getRolesFn = () => {
      this.roles$ = this.roleService.search('', '', this.translate.currentLang);
    };
    this.translate.onLangChange.subscribe((event: LangChangeEvent) => {
      getRolesFn();
    });
    getRolesFn();
    this.companyService.search('', '', this.translate.currentLang).subscribe((companies: Company[]) => {
      this.companiesList = companies;
    });
    var that = this;
    const capitalizeFirstLetter = (string: string) => {
      return string.charAt(0).toUpperCase() + string.slice(1);
    }
    this.datatableConfig = {
      serverSide: true,
      ajax: (dataTablesParameters: any, callback) => {
        this.userService.readDataTable(dataTablesParameters, this.translate.currentLang).subscribe(resp => {
          callback(resp);
        });
      },
      columns: [
        {
          title: this.translate.instant('USERS.LIST.FIRSTNAME'), name: "USERS.LIST.FIRSTNAME", data: 'firstname', render: function (data, _type, full) {
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
          title: this.translate.instant('USERS.LIST.LASTNAME'), name: "USERS.LIST.LASTNAME", data: 'lastname', type: 'string'
        },
        {
          title: this.translate.instant('USERS.LIST.EMAIL'), name: "USERS.LIST.EMAIL", data: 'email', type: 'string'
        },
        {
          title: this.translate.instant('USERS.LIST.EMAIL_CONFIRMED'), name: "USERS.LIST.EMAIL_CONFIRMED", data: 'emailConfirmed', 
          render: function (_data, _type, full) {            
            if (_data) {
              return '<span class="badge badge-light-success fs-7 fw-semibold">' + that.translate.instant('USERS.LIST.VALIDATED') + '</span>';
            } else {
              return '<span class="badge badge-light-danger fs-7 fw-semibold">' + that.translate.instant('USERS.LIST.NOT_VALIDATED') + '</span>';
            }
          },
          type: 'boolean'
        },                
        {
          title: this.translate.instant('USERS.LIST.COMPANIES'), name: "USERS.LIST.COMPANIES", data: 'companyNames', render: function (_data, _type, full) {
            return full.companyNames?.join(', ') || '';
          },
          type: 'string'
        },                
        {
          title: this.translate.instant('USERS.LIST.ROLES'), name: "USERS.LIST.ROLES", data: 'roleNames', render: function (_data, _type, full) {
            return full.roleNames?.join(', ') || '';
          },
          type: 'string'
        },
        {
          title: this.translate.instant('USERS.LIST.LAST_LOGIN'), name: "USERS.LIST.LAST_LOGIN", data: 'lastLoginDate', render: (data, _type, full) => {
            const date = data || full.lastLoginDate;
            let dateString;
            if (date === null) {
                dateString = that.translate.instant('USERS.LIST.NEVER');
            } else {
              dateString = moment(date).fromNow();
            }
            dateString = capitalizeFirstLetter(dateString);
            return `<div class="badge badge-light fw-bold">${dateString}</div>`;
          }
        },
        {
          title: this.translate.instant('USERS.LIST.JOINED_DATE'), name: "USERS.LIST.JOINED_DATE", data: 'createDate', render: (data, _type, full) => {
            const date = data || full.createDate;
            let dateString;
            if (date === null) {
                dateString = that.translate.instant('USERS.LIST.NEVER');
            } else {
              dateString = moment(date).fromNow();
            }
            dateString = capitalizeFirstLetter(dateString);
            return `<div class="badge badge-light fw-bold">${dateString}</div>`;
          }
        }
      ],
      createdRow: function (row, _data, _dataIndex) {
        $('td:eq(0)', row).addClass('d-flex align-items-center');
      }
    };
  }

  onChange($event: any, role: string) {
    this.userModel.updateRole(role, $event.target.checked);
  }

  onCompaniesChange() {
    this.loadHotelsForSelectedCompanies();
  }

  onSubmit(_event: Event, myForm: NgForm) {
    if (myForm && myForm.invalid) {
      return;
    }
    this.isLoading = true;
    const successAlert: SweetAlertOptions = {
      icon: 'success',
      title: this.translate.instant('USERS.LIST.SUCCESS'),
      text: this.userModel.id ? this.translate.instant('USERS.LIST.USER_UPDATED_SUCCESSFULLY') : this.translate.instant('USERS.LIST.USER_CREATED_SUCCESSFULLY'),
    };
    const errorAlert: SweetAlertOptions = {
      icon: 'error',
      title: 'Error!',
      text: '',
    };
    const completeFn = () => {
      this.isLoading = false;
    };
    const updateFn = () => {
      this.userService.update(this.userModel.id, this.userModel).subscribe({
        next: () => {
          this.showAlert(successAlert);
          this.reloadEvent.emit(true);
        },
        error: (error) => {
          errorAlert.text = this.extractText(error.error);
          this.showAlert(errorAlert);
          this.isLoading = false;
        },
        complete: completeFn,
      });
    };
    const createFn = () => {
      this.userService.createUser(this.userModel).subscribe({
        next: () => {
          this.showAlert(successAlert);
          this.reloadEvent.emit(true);
        },
        error: (error) => {
          errorAlert.text = this.extractText(error.error);
          this.showAlert(errorAlert);
          this.isLoading = false;
        },
        complete: completeFn,
      });      
    };
    if (this.userModel.id) {
      updateFn();
    } else {
      createFn();
    }    
  }

  rolesValidator(min = 1) {
    const validator = (formArray: FormArray) => {
      const totalSelected = formArray.controls
        .map(control => control.value)
        .reduce((prev, next) => next ? prev + next : prev, 0);
  
      return totalSelected >= min ? null : { required: true };
    };  
    return validator;
  }

  showAlert(swalOptions: SweetAlertOptions) {
    let style = swalOptions.icon?.toString() || 'success';
    if (swalOptions.icon === 'error') {
      style = 'danger';
    }
    this.swalOptions = Object.assign({
      buttonsStyling: false,
      confirmButtonText: this.translate.instant('USERS.LIST.OK'),
      customClass: {
        confirmButton: "btn btn-" + style
      }
    }, swalOptions);
    this.cdr.detectChanges();
    this.noticeSwal.fire();
  }
}
