import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { Observable } from 'rxjs';
import { IntegrationService } from '../services/integration-service';
import { SweetAlertOptions } from 'sweetalert2';
import { TranslateService } from '@ngx-translate/core';
import { Integration } from '../models/integration.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';
import { ProjectService } from '../../project/services/project-service';
import { Project } from '../../project/models/project.model';
import { AuthService } from 'src/app/modules/auth';

@Component({
  selector: 'app-integration-listing',
  templateUrl: './integration-listing.component.html',
  styleUrls: ['./integration-listing.component.scss']
})
export class IntegrationListingComponent implements OnInit, OnDestroy {

  isReadOnly = false;
  isCollapsed1 = false;
  isCollapsed2 = true;
  isLoading = false;
  integrations: DataTablesResponse;
  datatableConfig: DataTables.Settings = {};
  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();
  // Single model
  aIntegration: Observable<Integration>;
  integrationModel: Integration = new Integration();
  @ViewChild('noticeSwal')
  noticeSwal!: SwalComponent;
  swalOptions: SweetAlertOptions = {};
  projectsList: Project[];

  constructor(private apiService: IntegrationService, private projectService: ProjectService, public authService: AuthService, private cdr: ChangeDetectorRef, private translate: TranslateService) { }

  changeLanguage(languageCode: string) {
    this.integrationModel.changeLanguage(languageCode);
    this.cdr.markForCheck();
  }

  create() {
    this.integrationModel = new Integration();
  }

  delete(id: number) {
    this.apiService.delete(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  edit(event: { id: number, isReadOnly: boolean }) {
    this.isReadOnly = event.isReadOnly;
    this.aIntegration = this.apiService.get(event.id);
    this.aIntegration.subscribe((integration: Integration) => {
      this.integrationModel = integration;
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

  removeTranslations() {
    this.integrationModel.removeTranslations();
  }

  showAlert(swalOptions: SweetAlertOptions) {
    let style = swalOptions.icon?.toString() || 'success';
    if (swalOptions.icon === 'error') {
      style = 'danger';
    }
    this.swalOptions = Object.assign({
      buttonsStyling: false,
      confirmButtonText: this.translate.instant('INTEGRATIONS.LIST.OK'),
      customClass: {
        confirmButton: "btn btn-" + style
      }
    }, swalOptions);
    this.cdr.detectChanges();
    this.noticeSwal.fire();
  }

  ngOnDestroy(): void {
    this.reloadEvent.unsubscribe();
  }

  ngOnInit(): void {
    this.projectService.search('', '', this.translate.currentLang).subscribe((projects: Project[]) => {
      this.projectsList = projects;
    });
    this.datatableConfig = {
      serverSide: true,
      ajax: (dataTablesParameters: any, callback) => {
        this.apiService.readDataTable(dataTablesParameters, this.translate.currentLang).subscribe(resp => {
          callback(resp);
        });
      },
      columns: [
        {
          title: this.translate.instant('INTEGRATIONS.LIST.NAME'), name: "INTEGRATIONS.LIST.NAME", data: "name", render: function (data, _type, full) {
            const colorClasses = ['success', 'info', 'warning', 'danger'];
            const randomColorClass = colorClasses[Math.floor(Math.random() * colorClasses.length)];

            const initials = data[0].toUpperCase();
            const symbolLabel = `
              <div class="symbol-label fs-3 bg-light-${randomColorClass} text-${randomColorClass}">
                ${initials}
              </div>
            `;
            const name = `
              <div class="d-flex flex-column" data-action="view" data-id="${full.id}">
                <a href="javascript:;" class="text-gray-800 text-hover-primary mb-1">${data}</a>
                <span>${full.name}</span>
              </div>
            `;
            return `
              <div class="symbol symbol-circle symbol-50px overflow-hidden me-3" data-action="view" data-id="${full.id}">
                <a href="javascript:;">
                  ${symbolLabel}
                </a>
              </div>
              ${name}
            `;
          }
        },
        {
          title: this.translate.instant('INTEGRATIONS.LIST.PROJECT'), name: "INTEGRATIONS.LIST.PROJECT", data: 'projectName', type: 'string'
        }
      ],
      createdRow: function (row, _data) {
        $('td:eq(0)', row).addClass('d-flex align-items-center');
      }
    };
  }

  onSubmit(_event: Event, myForm: NgForm) {
    if (myForm && myForm.invalid) {
      return;
    }
    this.isLoading = true;
    const successAlert: SweetAlertOptions = {
      icon: 'success',
      title: this.translate.instant('INTEGRATIONS.LIST.SUCCESS'),
      text: this.integrationModel.id > 0 ? this.translate.instant('INTEGRATIONS.LIST.INTEGRATION_UPDATED_SUCCESSFULLY') : this.translate.instant('INTEGRATIONS.LIST.INTEGRATION_CREATED_SUCCESSFULLY'),
    };
    const errorAlert: SweetAlertOptions = {
      icon: 'error',
      title: this.translate.instant('INTEGRATIONS.LIST.ERROR'),
      text: '',
    };
    const completeFn = () => {
      this.isLoading = false;
    };
    const updateFn = () => {
      this.apiService.update(this.integrationModel.id, this.integrationModel).subscribe({
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
      this.apiService.create(this.integrationModel).subscribe({
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
    if (this.integrationModel.id > 0) {
      updateFn();
    } else {
      createFn();
    }
  }
}