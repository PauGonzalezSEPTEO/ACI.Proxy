import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { Observable } from 'rxjs';
import { ProjectService } from '../services/project-service';
import { SweetAlertOptions } from 'sweetalert2';
import { TranslateService } from '@ngx-translate/core';
import { Project } from '../models/project.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';
import { CompanyService } from '../../company/services/company-service';
import { Company } from '../../company/models/company.model';
import { AuthService } from 'src/app/modules/auth';

@Component({
  selector: 'app-project-listing',
  templateUrl: './project-listing.component.html',
  styleUrls: ['./project-listing.component.scss']
})
export class ProjectListingComponent implements OnInit, OnDestroy {

  isReadOnly = false;
  isCollapsed1 = false;
  isCollapsed2 = true;
  isLoading = false;
  projects: DataTablesResponse;
  datatableConfig: DataTables.Settings = {};
  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();
  // Single model
  aProject: Observable<Project>;
  projectModel: Project = new Project();
  @ViewChild('noticeSwal')
  noticeSwal!: SwalComponent;
  swalOptions: SweetAlertOptions = {};
  companiesList: Company[];

  constructor(private apiService: ProjectService, private companyService: CompanyService, public authService: AuthService, private cdr: ChangeDetectorRef, private translate: TranslateService) { }

  create() {
    this.projectModel = new Project();
  }

  delete(id: number) {
    this.apiService.delete(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  edit(event: { id: number, isReadOnly: boolean }) {
    this.isReadOnly = event.isReadOnly;
    this.aProject = this.apiService.get(event.id);
    this.aProject.subscribe((project: Project) => {
      this.projectModel = project;
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

  showAlert(swalOptions: SweetAlertOptions) {
    let style = swalOptions.icon?.toString() || 'success';
    if (swalOptions.icon === 'error') {
      style = 'danger';
    }
    this.swalOptions = Object.assign({
      buttonsStyling: false,
      confirmButtonText: this.translate.instant('PROJECTS.LIST.OK'),
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
    this.companyService.search('', '', this.translate.currentLang).subscribe((companies: Company[]) => {
      this.companiesList = companies;
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
          title: this.translate.instant('PROJECTS.LIST.NAME'), name: "PROJECTS.LIST.NAME", data: "name", render: function (data, _type, full) {
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
          title: this.translate.instant('PROJECTS.LIST.COMPANY'), name: "PROJECTS.LIST.COMPANY", data: 'companyName', type: 'string'
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
      title: this.translate.instant('PROJECTS.LIST.SUCCESS'),
      text: this.projectModel.id > 0 ? this.translate.instant('PROJECTS.LIST.PROJECT_UPDATED_SUCCESSFULLY') : this.translate.instant('PROJECTS.LIST.PROJECT_CREATED_SUCCESSFULLY'),
    };
    const errorAlert: SweetAlertOptions = {
      icon: 'error',
      title: this.translate.instant('PROJECTS.LIST.ERROR'),
      text: '',
    };
    const completeFn = () => {
      this.isLoading = false;
    };
    const updateFn = () => {
      this.apiService.update(this.projectModel.id, this.projectModel).subscribe({
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
      this.apiService.create(this.projectModel).subscribe({
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
    if (this.projectModel.id > 0) {
      updateFn();
    } else {
      createFn();
    }
  }
}
